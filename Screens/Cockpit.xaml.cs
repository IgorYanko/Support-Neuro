using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;
using NeuroApp.Api;
using NeuroApp.Classes;

namespace NeuroApp
{
    public partial class Cockpit : UserControl
    {
        public ObservableCollection<Sales> SalesData { get; set; } = new ObservableCollection<Sales>();
        private TimerService _apiTimer;
        private ObservableCollection<Sales> _cachedSalesData;

        private DataGridRow _draggedRow;
        private System.Windows.Point _startPoint;
        private bool _isScrolling = false;

        private Sales _sales;

        public Cockpit()
        {
            InitializeComponent();
            InitializeTimer();
            DataContext = new MainViewModel();
        }

        public async Task ProcessSalesDataAsync()
        {
            try
            {
                if (_cachedSalesData == null)
                {
                    _cachedSalesData = await FetchSalesDataAsync();
                }

                var salesData = _cachedSalesData;

                foreach (var sales in salesData)
                {
                    DatabaseActions database = new();
                    var existingSales = await database.GetSalesFromDatabaseAsync();

                    var existingSale = existingSales.FirstOrDefault(es => es.code == sales.code);

                    if (existingSale != null)
                    {
                        if (existingSale.ApprovedAt == null && sales.approved)
                        {
                            sales.ApprovedAt = DateTime.UtcNow;
                        }
                    }
                    await database.VerifyAndSave(sales);

                    foreach (var tag in sales.Tags)
                    {
                        await database.SaveTagForOSAsync(sales.code, tag.TagId);
                        await database.LinkTagToSaleAsync(sales.code, tag.Id);
                    }
                }

                await LoadSalesDataFromDatabaseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar dados: {ex.Message}");
            }
        }

        public async Task<ObservableCollection<Sales>> FetchSalesDataAsync()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

                var apiService = new SensioApiService(configuration);
                var endpoint = "sales/list/1";

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };

                var response = await apiService.GetDataAsync(endpoint);
                var apiResponse = JsonSerializer.Deserialize<ApiResponseSales>(response, options);

                if (apiResponse == null || apiResponse.Response == null)
                {
                    throw new InvalidOperationException("API response is null or invalid.");
                }

                DateTime maxDate = DateTime.Today.AddDays(-30);

                var filteredSales = apiResponse.Response
                    .Where(sale => sale.DateCreated >= maxDate && sale.DateCreated <= DateTime.Today)
                    .ToList();

                return new ObservableCollection<Sales>(filteredSales ?? new List<Sales>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao preencher DataGrid: {ex.Message}");
                return new ObservableCollection<Sales>();
            }
        }

        private void InitializeTimer()
        {
            _apiTimer = new TimerService(ProcessSalesDataAsync, TimeSpan.FromMinutes(5));
            _apiTimer.OnError += (s, ex) =>
            {
                MessageBox.Show($"Erro no timer: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _apiTimer.Start();
            await LoadSalesDataFromDatabaseAsync();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _apiTimer.Stop();
        }

        private async Task LoadSalesDataFromDatabaseAsync()
        {
            try
            {
                DatabaseActions database = new();
                var salesFromDb = await database.GetSalesFromDatabaseAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        SalesData.Clear();
                        foreach (var sale in salesFromDb)
                        {
                            SalesData.Add(sale);
                        }

                        ApplyPrioritySorting();

                        DataContext = this;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erro ao atualizar a interface do usuário: {ex.Message}");
                    }
                });

                var tagMapper = new Tags();

                //await Application.Current.Dispatcher.InvokeAsync(() =>
                //{
                //    SalesData.Clear();
                //    foreach (var sale in salesFromDb)
                //    {
                //        var mappedTags = sale.Tags
                //            .Select(tag => tagMapper.GetCustomTagById(tag.TagId))
                //            .Where(tag => tag != null)
                //            .ToList();

                //        sale.MappedTags = mappedTags;

                //        SalesData.Add(sale);
                //    }

                //    DataContext = this;
                //});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar dados do banco: {ex.Message}");
            }
        }

        private void DataGridSales_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedElement = e.EditingElement as TextBox;

            if (editedElement != null)
            {
                string cellText = editedElement.Text;

                var selectedRow = DataGridSales.SelectedItem as Sales;

                if (selectedRow != null)
                {
                    var numeroOs = selectedRow.code;

                    DatabaseActions databaseActions = new();
                    databaseActions.AddObservations(cellText, numeroOs);

                    selectedRow.Observation = cellText;
                }
            }
        }

        //private void InitializeDataGrid()
        //{
        //    DataGridSales.AllowDrop = true;
        //    DataGridSales.SelectionUnit = DataGridSelectionUnit.FullRow;
        //    DataGridSales.SelectionMode = DataGridSelectionMode.Single;

        //    DataGridSales.MouseMove += DataGridSales_MouseMove;
        //    DataGridSales.PreviewMouseLeftButtonDown += DataGridSales_PreviewMouseLeftButtonDown;
        //    DataGridSales.Drop += DataGridSales_Drop;
        //}

        private void DataGridSales_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);

            var position = e.GetPosition(DataGridSales);
            var row = GetDataGridRowUnderMouse(position);
            if (row != null)
            {
                _draggedRow = row;
            }
        }

        private void DataGridSales_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedRow != null)
            {
                DragDrop.DoDragDrop(DataGridSales, _draggedRow.Item, DragDropEffects.Move);
            }
        }

        private void DataGridSales_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Sales)))
            {
                var draggedItem = e.Data.GetData(typeof(Sales)) as Sales;

                var position = e.GetPosition((IInputElement)sender);
                var row = GetDataGridRowUnderMouse(position);

                if (row != null)
                {
                    var targetItem = row.Item as Sales;

                    var draggedIndex = SalesData.IndexOf(draggedItem);
                    var targetIndex = SalesData.IndexOf(targetItem);

                    if (draggedIndex >= 0 && targetIndex >= 0 && draggedIndex != targetIndex)
                    {
                        if (draggedIndex >= 0 && draggedIndex < SalesData.Count &&
                            targetIndex >= 0 && targetIndex < SalesData.Count)
                        {
                            SalesData.Move(draggedIndex, targetIndex);

                            DatabaseActions databaseActions = new();
                            for (int i = 0; i < SalesData.Count; i++)
                            {
                                SalesData[i].Priority = i;
                                SalesData[i].IsManual = true;

                                databaseActions.UpdatePriority(SalesData[i].code, i);
                            }

                            ApplyPrioritySorting();
                        }
                    }
                }
            }
        }

        private DataGridRow GetDataGridRowUnderMouse(System.Windows.Point position)
        {
            var element = DataGridSales.InputHitTest(position) as DependencyObject;

            while (element != null && !(element is DataGridRow))
            {
                element = VisualTreeHelper.GetParent(element);
            }

            return element as DataGridRow;
        }

        private void DataGridSales_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Sales)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects |= DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void DataGridSales_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var row = (DataGridRow)((DependencyObject)e.OriginalSource).GetParentOfType<DataGridRow>();
            if (row != null)
            {
                _sales = (Sales)row.Item;
            }
        }

        private void RemoveOs_Click(object sender, RoutedEventArgs e)
        {
            if (_sales != null)
            {
                var result = MessageBox.Show($"Deseja remover a OS nº {_sales.code}?",
                                              "Confirmação",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DatabaseActions databaseActions = new();
                    databaseActions.RemoveOs(_sales.code);

                    SalesData.Remove(_sales);
                    MessageBox.Show("OS removida com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void PauseOs_Click()
        {
            if (_sales != null)
            {
                var result = MessageBox.Show($"Deseja pausar a OS nº {_sales.code}?",
                                              "Confirmação",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DatabaseActions databaseActions = new();
                    databaseActions.PauseOs(_sales.code);

                    MessageBox.Show("OS pausada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ApplyPrioritySorting()
        {
            var sortedSales = SalesData
                .OrderByDescending(sale => sale.IsManual)
                .ThenBy(sale => sale.Priority)
                .ThenByDescending(sale => sale.ApprovedAt ?? DateTime.MinValue)
                .ToList();

            SalesData.Clear();
            foreach (var sale in sortedSales)
            {
                SalesData.Add(sale);
            }
        }

        private void DataGridSales_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedRow != null)
            {
                System.Windows.Point currentPoint = e.GetPosition(null);

                double horizontalDistance = Math.Abs(currentPoint.X - _startPoint.X);
                double verticalDistance = Math.Abs(currentPoint.Y - _startPoint.Y);

                double minScrollDistance = SystemParameters.MinimumHorizontalDragDistance;

                if (horizontalDistance > minScrollDistance || verticalDistance > minScrollDistance)
                {
                    _isScrolling = true;
                }

                if (!_isScrolling)
                {
                    DragDrop.DoDragDrop(DataGridSales, _draggedRow.Item, DragDropEffects.Move);
                }
            }
        }

        private void DataGridSales_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isScrolling = false;
            _draggedRow = null;
        }

        private void PaintRowByPriority()
        {
            var sales = _cachedSalesData;
            TimeSpan interval = new(7,)

            foreach (var sale in sales)
            {
                DateTime? finalDate = sale.ApprovedAt + TimeSpan;
            }
        }
    }

    public class RowColors
    {
        public System.Drawing.Color backgroundColor {  get; set; }
        public System.Drawing.Color foregroundColor { get; set; }
    }

    public static class DependencyObjectExtensions
    {
        public static T GetParentOfType<T>(this DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if (parentObject is T parent) return parent;

            return GetParentOfType<T>(parentObject);
        }
    }
}
