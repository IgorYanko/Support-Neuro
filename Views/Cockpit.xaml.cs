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
using NeuroApp.Interfaces;
using NeuroApp.Views;
using System.Windows.Threading;

namespace NeuroApp
{
    public partial class Cockpit : UserControl
    {
        private readonly IConfiguration _configuration;
        private readonly IMainViewModel _mainViewModel;

        public ObservableCollection<Sales> SalesData { get; set; } = new ObservableCollection<Sales>();
        private ObservableCollection<Sales> _cachedSalesData = new();
        private TimerService? _apiTimer;

        private DataGridRow? _draggedRow;
        private Point _startPoint;
        private bool _isScrolling = false;
        private bool _isLoading = false;

        public Cockpit(IMainViewModel mainViewModel, IConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration;
            _mainViewModel = mainViewModel;
            _apiTimer = null;
            _draggedRow = null;
            InitializeTimer();
            DataContext = mainViewModel;
        }

        public async Task ProcessSalesDataAsync()
        {
            try
            {
                DatabaseActions database = new(_configuration);
                var salesFromDb = await database.GetSalesFromDatabaseAsync();
                
                _cachedSalesData = new ObservableCollection<Sales>(salesFromDb);

                var apiSales = await FetchSalesDataAsync();

                foreach (var apiSale in apiSales)
                {
                    var cachedSale = _cachedSalesData.FirstOrDefault(s => s.Code == apiSale.Code);

                    if (cachedSale != null && cachedSale.Excluded)
                    {
                        continue;
                    }

                    if (cachedSale == null && apiSale.Status.ToString() != "Faturado" && !apiSale.Excluded)
                    {
                        await database.VerifyAndSave(apiSale);
                        _cachedSalesData.Add(apiSale);
                    }
                    else if (cachedSale == null && apiSale.Status.ToString() == "Faturado")
                    {
                        continue;
                    }
                    else if (cachedSale != null && cachedSale.Status != apiSale.Status)
                    {
                        cachedSale.Status = apiSale.Status;
                        cachedSale.IsStatusModified = false;
                        await database.VerifyAndSave(apiSale);
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
                DatabaseActions database = new(_configuration);
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

                List<string> deletedOsCodes = await database.GetDeletedOsCodesAsync();

                DateTime maxDate = DateTime.Today.AddDays(-30);

                var filteredSales = apiResponse.Response
                    .Where(sale =>
                        sale.DateCreated >= maxDate &&
                        sale.DateCreated <= DateTime.Today &&
                        !deletedOsCodes.Contains(sale.Code)
                    )
                    .ToList();

                return new ObservableCollection<Sales>(filteredSales);
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

            if (success && _apiTimer != null)
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
                ShowLoading(true);
                DatabaseActions database = new(_configuration);

                var updatedSalesData = new List<Sales>();

                foreach (var sale in _cachedSalesData)
                {
                    sale.Tags = await database.GetTagsForOsAsync(sale.Code);
                    sale.Priority = database.CalculatePriority(sale.Status.ToString());
                    updatedSalesData.Add(sale);
                }

                updatedSalesData = updatedSalesData
                    .OrderByDescending(s => s.IsManual)
                    .ThenBy(s => s.Priority)
                    .ToList();

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
            finally
            {
                ShowLoading(false);
            }
        }

        private void ShowLoading(bool show)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
                _isLoading = show;
            });
        }

        private bool _isSyncingScroll = false;

        private void DataGridSales_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Column.Header?.ToString() == "Status" && e.Row.Item is Sales selectedRow)
            {
                var currentStatus = selectedRow.Status;
                bool isEditable = SalesUtils.IsLocalStatus(currentStatus.ToString()) || currentStatus == Status.Aprovado; // Ajuste para enum

                if (!isEditable)
                {
                    e.Cancel = true;
                    return;
                }

                var allowedStatuses = GetStatusToComboBox.GetStatusToComboBoxList<Status>()
                    .Where(s => SalesUtils.IsLocalStatus(s) || s == Status.Aprovado.ToString())
                    .ToList();

                selectedRow.StatusList.Clear();
                selectedRow.StatusList.AddRange(allowedStatuses);
            }
        }

        private void DataGridSales_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is DependencyObject originalSource)
            {
                DependencyObject current = originalSource;
                while (current != null && !(current is DataGridCell))
                {
                    current = VisualTreeHelper.GetParent(current);
                }

                if (current is DataGridCell cell && cell.Column?.Header?.ToString() == "Obs")
                {
                    Button button = FindButtonInVisualTree(cell);
                    if (button != null)
                    {
                        if (cell.DataContext is Sales sale && sale != null)
                        {
                            _mainViewModel.SelectedSale = sale;

                            if (!string.IsNullOrWhiteSpace(_mainViewModel.SelectedSale.Observation))
                            {
                                ObservationPopUp.DataContext = sale;
                                ObservationPopUp.PlacementTarget = button;
                                ObservationPopUp.Placement = PlacementMode.Top;
                                ObservationPopUp.VerticalOffset = -10;
                                ObservationPopUp.IsOpen = true;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Erro!");
                        }
                    }
                }

                var source = originalSource;
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

        private Button FindButtonInVisualTree(DependencyObject parent)
        {
            if (parent == null)
                return null;

            if (parent is Button button)
                return button;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var result = FindButtonInVisualTree(child);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void DataGridSales_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _draggedRow != null)
            {
                DragDrop.DoDragDrop(DataGridSales, _draggedRow.Item, DragDropEffects.Move);
            }
        }

        private async void DataGridSales_Drop(object sender, DragEventArgs e)
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

                            DatabaseActions databaseActions = new(_configuration);

                            var updates = new List<Task>();

                            for (int i = 0; i < SalesData.Count; i++)
                            {
                                SalesData[i].Priority = i;

                                //bool isManualValue = (SalesData[i] == draggedItem);
                                //updates.Add(databaseActions.UpdatePriorityAsync(SalesData[i].Code, isManualValue));
                                bool isManualValue = SalesData[i].IsManual || (SalesData[i] == draggedItem);
                                databaseActions.UpdatePriorityAsync(SalesData[i].Code, isManualValue);
                            }

                            //await Task.WhenAll(updates);
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

        private async void RemoveOs_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataGridSales.SelectedItem is Sales selectedSale))
            {
                MessageBox.Show("Nenhuma OS selecionada.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var warrantyResult = MessageBox.Show(
                    "Deseja criar uma garantia para esta OS?",
                    "Remover OS",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (warrantyResult == MessageBoxResult.Yes)
                {
                    await HandleWarrantyCreation(selectedSale);
                }
                else
                {
                    await ConfirmAndRemoveOs(selectedSale);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar a solicitação: {ex.Message}",
                               "Erro",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }                                 
        }

        private async Task HandleWarrantyCreation(Sales selectedSale)
        {
            var warranty = new Warranty
            {
                OsCode = selectedSale.Code,
                Customer = selectedSale.PersonName,
                ServiceDate = DateTime.Now,
                WarrantyMonths = 3
            };

            var dialog = new WarrantyDetailsDialog(warranty, _configuration);

            if (dialog.ShowDialog() == true)
            {
                await RemoveOsAndUpdateUI(selectedSale.Code, selectedSale);
                MessageBox.Show("OS removida e garantia registrada com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task ConfirmAndRemoveOs(Sales selectedSale)
        {
            var confirmResult = MessageBox.Show(
                "Tem certeza que deseja remover a OS sem criar garantia?",
                "Confirmação Remoção",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmResult == MessageBoxResult.Yes)
            {
                await RemoveOsAndUpdateUI(selectedSale.Code, selectedSale);
                MessageBox.Show("OS removida com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task RemoveOsAndUpdateUI(string osCode, Sales saleToRemove)
        {
            var databaseActions = new DatabaseActions(_configuration);
            var success = await databaseActions.RemoveOsAsync(osCode);

            if (!success)
            {
                throw new InvalidOperationException("Falha ao remover a OS. Registro não encontrado");
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                SalesData.Remove(saleToRemove);
            });
        }

        private async void PauseOs_Click(object sender, RoutedEventArgs e)
        {
            DatabaseActions database = new(_configuration);
            var selectedSale = DataGridSales.SelectedItem as Sales;

            if (selectedSale != null && !selectedSale.IsPaused)
            {
                var result = MessageBox.Show($"Deseja pausar a OS nº {selectedSale.Code}?",
                                              "Confirmação",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool sucess = await database.PauseOsAsync(selectedSale.Code, selectedSale.Status, selectedSale.DisplayType);

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

        private async void AddObservation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is MenuItem menuItem && menuItem.Parent is ContextMenu contextMenu)
                {
                    DataGridRow row = null;
                    var target = contextMenu.PlacementTarget as DependencyObject ?? (e.OriginalSource as DependencyObject);
                    if (target != null)
                    {
                        row = DependencyObjectExtensions.FindAncestor<DataGridRow>(target);
                    }

                    if (row == null)
                    {
                        var dataGrid = DependencyObjectExtensions.FindAncestor<DataGrid>(target);
                        if (dataGrid != null && dataGrid.SelectedItem is Sales selectedRow)
                        {
                            row = dataGrid.ItemContainerGenerator.ContainerFromItem(selectedRow) as DataGridRow;
                        }
                    }

                    if (row != null && row.DataContext is Sales selectedSale)
                    {
                        string osCode = selectedSale.Code;

                        if (string.IsNullOrEmpty(osCode))
                        {
                            MessageBox.Show("O código da OS não está disponível.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        var dialog = new ObservationsDialog(
                            _configuration,
                            osCode,
                            !string.IsNullOrEmpty(selectedSale.Observation) ? selectedSale.Observation : null);

                        dialog.ShowDialog();

                        if (dialog.IsConfirmed)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                ProcessSalesDataAsync();
                            });
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Selecione uma linha válida no DataGrid.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar observação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is string selectedStatus)
            {
                if (DataGridSales.SelectedItem is Sales selectedSale)
                {
                    if (SalesUtils.IsLocalStatus(selectedStatus) || selectedStatus == "Aprovado")
                    {
                        selectedSale.IsStatusModified = true;
                        Status? enumStatus = GetStatusToComboBox.ConvertDisplayToEnum<Status>(selectedStatus);

                        if (!enumStatus.HasValue || selectedSale.Status.ToString() == enumStatus.ToString()) return;

                        selectedSale.DisplayStatus = selectedStatus;

                        DatabaseActions databaseActions = new(_configuration);
                        await databaseActions.UpdateStatusOnDatabaseAsync(selectedSale.Code, enumStatus.ToString());

                        MessageBox.Show($"Status atualizado para {selectedStatus} na OS {selectedSale.Code}", "Status atualizado!", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        comboBox.IsEnabled = false;
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ShowHomeScreen();
        }

        public void OpenPopup(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Sales sale)
            {
                if (!string.IsNullOrEmpty(sale.Observation))
                {
                    ObservationPopUp.DataContext = sale;
                    ObservationPopUp.PlacementTarget = button;
                    ObservationPopUp.Placement = PlacementMode.Top;
                    ObservationPopUp.VerticalOffset = -10;
                    ObservationPopUp.IsOpen = true;

                }
                else
                {
                    MessageBox.Show("Erro teste!");
                }
            }
            else
            {
                MessageBox.Show("Erro!");
            }
        }

        private void ClosePopupButton_Click(object sender, RoutedEventArgs e)
        {
            ObservationPopUp.IsOpen = false;
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

    public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);
            if (child is T result)
                return result;
            var childResult = FindVisualChild<T>(child);
            if (childResult != null)
                return childResult;
        }
        return null;
    }

    public static T FindAncestor<T>(this DependencyObject element) where T : DependencyObject
    {
        while (element != null)
        {
            if (element is T ancestor)
                return ancestor;
            element = VisualTreeHelper.GetParent(element);
        }
        return null;
    }
}

public static class DataGridExtensions
{
    public static DataGridRow GetRow(this DataGrid grid, int index)
    {
        return grid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
    }

    public static int GetFirstVisibleRowIndex(this DataGrid grid)
    {
        var scrollViewer = DependencyObjectExtensions.FindVisualChild<ScrollViewer>(grid);
        if (scrollViewer == null) return 0;

        return (int)(scrollViewer.VerticalOffset / grid.RowHeight);
    }

    public static int GetLastVisibleRowIndex(this DataGrid grid)
    {
        var scrollViewer = DependencyObjectExtensions.FindVisualChild<ScrollViewer>(grid);
        if (scrollViewer == null) return grid.Items.Count - 1;

        return (int)((scrollViewer.VerticalOffset + scrollViewer.ViewportHeight) / grid.RowHeight);
    }
}
