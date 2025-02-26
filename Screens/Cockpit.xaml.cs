using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private ObservableCollection<Sales> _cachedSalesData = new();
        private TimerService _apiTimer;

        private DataGridRow _draggedRow;
        private Point _startPoint;
        private bool _isScrolling = false;

        public Cockpit(MainViewModel mainViewModel)
        {
            InitializeComponent();
            InitializeTimer();
            DataContext = mainViewModel;
            Console.WriteLine($"Cockpit inicializado. ViewModel: {mainViewModel}");
        }

        public async Task ProcessSalesDataAsync()
        {
            try
            {
                DatabaseActions database = new();
                var salesFromDb = await database.GetSalesFromDatabaseAsync();
                
                _cachedSalesData = new ObservableCollection<Sales>(salesFromDb);

                var apiSales = await FetchSalesDataAsync();

                foreach (var apiSale in apiSales)
                {
                    var cachedSale = _cachedSalesData?.FirstOrDefault(s => s.Code == apiSale.Code);

                    if (cachedSale != null && cachedSale.Excluded)
                    {
                        continue;
                    }

                    if (cachedSale == null && apiSale.Status.ToString() != "Faturado" && !apiSale.Excluded)
                    {
                        await database.VerifyAndSave(apiSale);
                        apiSale.Tags = await database.GetTagsForOsAsync(apiSale.Code);
                        _cachedSalesData?.Add(apiSale);
                    }
                    else if (cachedSale == null && apiSale.Status.ToString() == "Faturado")
                    {
                        continue;
                    }
                    else if (cachedSale.Status != apiSale.Status)
                    {
                        cachedSale.Status = apiSale.Status;
                        cachedSale.IsStatusModified = false;
                    }
                }

                await LoadSalesDataFromDatabaseAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar dados: {ex.Message}");
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

                if (apiResponse?.Response == null)
                {
                    throw new InvalidOperationException("API response is null or invalid.");
                }

                DateTime maxDate = DateTime.Today.AddDays(-30);

                return new ObservableCollection<Sales>(
                    apiResponse.Response.Where(sale => sale.DateCreated >= maxDate && sale.DateCreated <= DateTime.Today)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao preencher DataGrid: {ex.Message}");
                return new ObservableCollection<Sales>();
            }
        }

        public async Task<Sales?> FetchSpecificSaleAsync(string saleCode)
        {
            var endpoint = $"sales/{Uri.EscapeDataString(saleCode)}";

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json").Build();

                var apiService = new SensioApiService(configuration);

                var response = await apiService.GetDataAsync(endpoint);

                if (string.IsNullOrWhiteSpace(response))
                {
                    Console.WriteLine($"Erro: resposta vazia ao buscar pedido {saleCode}");
                    return null;
                }

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


        private void InitializeTimer()
        {
            _apiTimer = new TimerService(ProcessSalesDataAsync, TimeSpan.FromMinutes(2), Application.Current.Dispatcher);
            _apiTimer.OnError += (s, ex) =>
            {
                MessageBox.Show($"Erro no timer: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            bool success = await LoadSalesDataFromDatabaseAsync();

            if (success)
            {
                _apiTimer.Start();
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_apiTimer != null)
            {
                _apiTimer.Stop();
                _apiTimer = null;
            }
        }

        private async Task<bool> LoadSalesDataFromDatabaseAsync()
        {
            try
            {
                DatabaseActions database = new();

                var updatedSalesData = new List<Sales>();

                foreach (var sale in _cachedSalesData)
                {
                    sale.Tags = await database.GetTagsForOsAsync(sale.Code);
                    updatedSalesData.Add(sale);
                }

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    SalesData.Clear();
                    foreach (var sale in updatedSalesData.Where(s => !s.Excluded))
                    {
                        SalesData.Add(sale);
                    }

                    DataContext = this;
                });


                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar dados do banco: {ex.Message}");
                return false;
            }
        }

        private void DataGridSales_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.Header.ToString() == "Status")
            {
                if (e.Row.Item is Sales selectedRow)
                {
                    var currentStatus = selectedRow.Status.ToString();
                    var localStatuses = GetStatusToComboBox.GetStatusToComboBoxList<Status>()
                        .Where(Sales.IsLocalStatus)
                        .ToList();

                    if (!localStatuses.Contains(currentStatus))
                    {
                        localStatuses.Add(currentStatus);
                    }

                    selectedRow.StatusList.Clear();
                    selectedRow.StatusList.AddRange(localStatuses);
                }
            }
        }

        private async void DataGridSales_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (e.Column.Header.ToString() != "Observações")
                {
                    return;
                }

                if (e.EditingElement is TextBox editedElement)
                {
                    string cellText = editedElement.Text;

                    if (e.Row.Item is Sales selectedRow)
                    {
                        var numeroOs = selectedRow.Code;

                        DatabaseActions databaseActions = new();
                        await databaseActions.AddObservationsAsync(cellText, numeroOs);

                        selectedRow.Observation = cellText;

                        selectedRow.StatusList.Clear();
                        selectedRow.StatusList.AddRange(GetStatusToComboBox.GetStatusToComboBoxList<Status>()
                            .Where(Sales.IsLocalStatus)); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar observação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var selectedSale = (Sales)row.Item;

                Console.WriteLine($"[MouseRightButtonDown] OS selecionada: {selectedSale.Code} - Status: {selectedSale.Status}");
            }
        }


        private void RemoveOs_Click(object sender, RoutedEventArgs e)
        {
            var selectedSale = DataGridSales.SelectedItem as Sales;

            if (selectedSale != null)
            {
                var result = MessageBox.Show($"Deseja remover a OS nº {selectedSale.Code}?",
                                              "Confirmação",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DatabaseActions databaseActions = new();
                    databaseActions.RemoveOs(selectedSale.Code);

                    SalesData.Remove(selectedSale);
                    MessageBox.Show("OS removida com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private async void PauseOs_Click(object sender, RoutedEventArgs e)
        {
            DatabaseActions database = new();
            var selectedSale = DataGridSales.SelectedItem as Sales;

            if (selectedSale != null && !selectedSale.IsPaused)
            {
                var result = MessageBox.Show($"Deseja pausar a OS nº {selectedSale.Code}?",
                                              "Confirmação",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool sucess = await database.PauseOsAsync(selectedSale.Code, selectedSale.Status);

                    if (sucess)
                    {
                        selectedSale.IsPaused = true;
                        SalesData.First(s => s.Code == selectedSale.Code).IsPaused = true;
                        await LoadSalesDataFromDatabaseAsync();
                    }
                }
            }
            else
            {
                bool sucess = await database.UnpauseOsAsync(selectedSale.Code);

                if (sucess)
                {
                    selectedSale.IsPaused = false;
                    SalesData.First(s => s.Code == selectedSale.Code).IsPaused = false;
                    await LoadSalesDataFromDatabaseAsync();
                }
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

                        if (!enumStatus.HasValue || selectedSale.Status.ToString() == enumStatus.ToString()) return;

                        DatabaseActions databaseActions = new();
                        databaseActions.UpdateStatusOnDatabaseAsync(selectedSale.Code, enumStatus.ToString());

                        Console.WriteLine($"Status atualizado para {selectedStatus} na OS {selectedSale.Code}");
                    }
                }
            }
        }

        //private void HelpButton_Click(object sender, RoutedEventArgs e)
        //{
        //    DialogHost.OpenDialogCommand.Execute(null, HelpDialogHost);
        //}
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
