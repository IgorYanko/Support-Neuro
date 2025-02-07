using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
        private Point _startPoint;
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
                var apiSales = await FetchSalesDataAsync();

                foreach (var apiSale in apiSales)
                {
                    var cachedSale = _cachedSalesData?.FirstOrDefault(s => s.Code == apiSale.Code);

                    if (cachedSale != null)
                    {
                        if (cachedSale.IsStatusModified && Sales.IsLocalStatus(cachedSale.Status.ToString()))
                        {
                            continue;
                        }

                        if (!Sales.IsLocalStatus(apiSale.Status.ToString()))
                        {
                            cachedSale.Status = apiSale.Status;
                            cachedSale.IsStatusModified = false;
                        }
                    }
                    else
                    {
                        DatabaseActions database = new();
                        await database.VerifyAndSave(apiSale);

                        _cachedSalesData?.Add(apiSale);
                    }
                }

                await LoadSalesDataFromDatabaseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar dados: {ex.Message}");
            }
        }

        public async Task<Sales> FetchSpecificSaleAsync(string saleCode)
        {
            var endpoint = $"sales/{saleCode}";

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json").Build();

                var apiService = new SensioApiService(configuration);

                var response = await apiService.GetDataAsync(endpoint);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };

                var sale = JsonSerializer.Deserialize<Sales>(response, options);
                return sale;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar pedido {saleCode}: {ex.Message}");
                return null;
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
            _apiTimer = new TimerService(ProcessSalesDataAsync, TimeSpan.FromMinutes(5), Application.Current.Dispatcher);
            _apiTimer.OnError += (s, ex) =>
            {
                MessageBox.Show($"Erro no timer: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadSalesDataFromDatabaseAsync();

            _apiTimer.Start();
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
                    var numeroOs = selectedRow.Code;

                    DatabaseActions databaseActions = new();
                    databaseActions.AddObservationsAsync(cellText, numeroOs);

                    selectedRow.Observation = cellText;
                }
            }
        }

        private void DataGridSales_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject source)
            {
                while (source != null)
                {
                    if (source is ScrollBar)
                    {
                        _isScrolling = true;
                        _draggedRow = null;
                        return;
                    }
                    source = VisualTreeHelper.GetParent(source);
                }
            }

            _startPoint = e.GetPosition(null);

            var position = e.GetPosition(DataGridSales);
            var row = GetDataGridRowUnderMouse(position);
            if (row != null)
            {
                _draggedRow = row;
                _isScrolling = false;
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

                                bool isManualValue = SalesData[i].IsManual || (SalesData[i] == draggedItem);

                                databaseActions.UpdatePriorityAsync(SalesData[i].Code, i, isManualValue);
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

            while (element != null)
            {
                if (element is ScrollBar)
                {
                    return null;
                }

                if (element is DataGridRow row)
                {
                    return row;
                }

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }

        private void DataGridSales_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Sales)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
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
                var result = MessageBox.Show($"Deseja remover a OS nº {_sales.Code}?",
                                              "Confirmação",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DatabaseActions databaseActions = new();
                    databaseActions.RemoveOs(_sales.Code);

                    SalesData.Remove(_sales);
                    MessageBox.Show("OS removida com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private async void PauseOs_Click(object sender, RoutedEventArgs e)
        {
            DatabaseActions databaseActions = new();

            if (_sales != null && !_sales.IsPaused)
            {
                var result = MessageBox.Show($"Deseja pausar a OS nº {_sales.Code}?",
                                              "Confirmação",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool sucess = await databaseActions.PauseOsAsync(_sales.Code);

                    if (sucess)
                    {
                        _sales.IsPaused = true;
                        SalesData.First(s => s.Code == _sales.Code).IsPaused = true;
                        await LoadSalesDataFromDatabaseAsync();
                    }
                }
            }
            else
            {
                bool sucess = await databaseActions.UnpauseOsAsync(_sales.Code);

                if (sucess)
                {
                    await LoadSalesDataFromDatabaseAsync();
                }
                //MessageBox.Show("Esta OS já pausada.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is string selectedStatus)
            {
                if (DataGridSales.SelectedItem is Sales selectedSale)
                {
                    if (Sales.IsLocalStatus(selectedStatus))
                    {
                        selectedSale.IsStatusModified = true;
                        Status? enumStatus = GetStatusToComboBox.ConvertDisplayToEnum<Status>(selectedStatus);

                        DatabaseActions databaseActions = new();
                        databaseActions.UpdateStatusOnDatabaseAsync(selectedSale.Code, enumStatus.ToString());

                        Console.WriteLine($"Status atualizado para {selectedStatus} na OS {selectedSale.Code}");
                    }
                }
            }
        }
    }
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
