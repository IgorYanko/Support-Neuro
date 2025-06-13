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
using NeuroApp.Services;

namespace NeuroApp
{
    public partial class Cockpit : UserControl
    {
        private readonly IConfiguration _configuration;
        private readonly IMainViewModel _mainViewModel;
        private readonly IApiService _apiService;
        private readonly ICacheService _cacheService;
        private readonly IDatabaseActions _database;

        public ObservableCollection<Sales> _cachedSalesData;

        private List<string> deletedOsCodes = new();

        private DataGridRow? _draggedRow;
        private Point _startPoint;
        private bool _isScrolling = false;
        private bool _isLoading = false;

        public Cockpit(IMainViewModel mainViewModel, IConfiguration configuration, IApiService apiService, ICacheService cacheService, IDatabaseActions database)
        {
            InitializeComponent();
            _configuration = configuration;
            _mainViewModel = mainViewModel;
            _apiService = apiService;
            _cacheService = cacheService;
            _database = database;

            _draggedRow = null;

            DataContext = mainViewModel;
        }

        public async Task ProcessSalesDataAsync()
        {
            try
            {
                var salesFromDb = await _database.GetSalesFromDatabaseAsync();
                _cachedSalesData = new ObservableCollection<Sales>(salesFromDb);

                var deletedOsCodes = await _database.GetDeletedOsCodesAsync();

                var apiSales = await FetchSalesDataAsync(deletedOsCodes);

                foreach (var apiSale in apiSales)
                {
                    var cachedSale = _cachedSalesData.FirstOrDefault(s => s.Code == apiSale.Code);

                    if (cachedSale != null && cachedSale.Excluded)
                        continue;

                    if (cachedSale == null && apiSale.Status.ToString() != "Faturado" && !apiSale.Excluded)
                    {
                        await _database.VerifyAndSave(apiSale);
                        _cachedSalesData.Add(apiSale);
                    }
                    else if (cachedSale != null && cachedSale.Status != apiSale.Status)
                    {
                        cachedSale.Status = apiSale.Status;
                        cachedSale.IsStatusModified = false;
                        await _database.VerifyAndSave(apiSale);
                    }
                }

                await LoadSalesDataFromDatabaseAsync(deletedOsCodes);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar dados: {ex.Message}");
            }
        }

        public async Task<ObservableCollection<Sales>> FetchSalesDataAsync(HashSet<string> deletedOsCodes)
        {
            try
            {
                var endpoint = "sales/list/1";
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };

                var response = await _apiService.GetDataAsync(endpoint);
                var apiResponse = JsonSerializer.Deserialize<ApiResponseSales>(response, options);

                if (apiResponse?.Response == null)
                {
                    throw new InvalidOperationException("API response is null or invalid.");
                }

                DateTime maxDate = DateTime.UtcNow.AddDays(-30);

                var filteredSales = apiResponse.Response
                    .Where(sale =>
                        sale.DateCreated >= maxDate &&
                        sale.DateCreated <= DateTime.UtcNow &&
                        !deletedOsCodes.Contains(sale.Code)
                    )
                    .ToList();

                return new ObservableCollection<Sales>(filteredSales);
            }
            catch (Exception ex)
            {
                return new ObservableCollection<Sales>();
            }
        }

        public async Task UpdateDataAsync()
        {
            try
            {
                var response = await _apiService.GetDataAsync("sales/list/1");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };

                var apiResponse = JsonSerializer.Deserialize<ApiResponseSales>(response, options);
                if (apiResponse?.Response == null)
                {
                    throw new InvalidOperationException("API response is null or invalid.");
                }

                var deletedOsCodesSet = await _database.GetDeletedOsCodesAsync();

                var apiSales = apiResponse.Response
                    .Where(sale =>
                        sale.DateCreated >= DateTime.Today.AddDays(-30) &&
                        sale.DateCreated <= DateTime.Today &&
                        !deletedOsCodes.Contains(sale.Code) &&
                        sale.Status.ToString() != "Faturado" &&
                        !sale.Excluded
                    )
                    .ToList();


                var existingSales = await _database.GetSalesFromDatabaseAsync();
                var existingSalesDict = existingSales.ToDictionary(s => s.Code);

                var salesToSave = new List<Sales>();

                foreach (var apiSale in apiSales)
                {
                    if (existingSalesDict.TryGetValue(apiSale.Code, out var existingSale))
                    {
                        if (existingSale.Excluded) continue;

                        if (existingSale.Status != apiSale.Status)
                        {
                            existingSale.Status = apiSale.Status;
                            existingSale.IsStatusModified = false;
                            salesToSave.Add(existingSale);
                        }
                    }
                    else
                    {
                        salesToSave.Add(apiSale);
                    }
                }

                foreach (var sale in salesToSave)
                {
                    await _database.VerifyAndSave(sale);
                }

                var updatedSales = await _database.GetSalesFromDatabaseAsync();

                _cachedSalesData = new(updatedSales);

                await SalesCacheManager.SaveCacheAsync(new SalesCacheData
                {
                    Sales = updatedSales,
                    DeletedOsCodes = deletedOsCodes.ToHashSet()
                });

                await LoadSalesDataFromDatabaseAsync(deletedOsCodesSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar dados: {ex.Message}");
            }
        }


        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var cacheData = await SalesCacheManager.LoadCacheAsync();
            var deletedOsCodes = cacheData.DeletedOsCodes;
            _cachedSalesData = new(cacheData.Sales);

            await LoadSalesDataFromDatabaseAsync(deletedOsCodes);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private async Task<ObservableCollection<Sales>> LoadSalesDataFromDatabaseAsync(HashSet<string> deletedOsCodes)
        {
            try
            {
                ShowLoading(true);
                MessageBox.Show("Funcionando");

                var tasks = _cachedSalesData.Select(async sale =>
                {
                    sale.Tags = await _database.GetTagsForOsAsync(sale.Code);
                    sale.Priority = _database.CalculatePriority(sale.Status.ToString());
                    return sale;    
                });

                var updatedSalesData = (await Task.WhenAll(tasks))
                    .OrderBy(s => s.Priority)
                    .ThenBy(s => !s.IsManual)
                    .ThenBy(s => s.DateCreated)
                    .ToList();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _mainViewModel.SalesData.Clear();
                    foreach (var sale in updatedSalesData.Where(s => !s.Excluded && !deletedOsCodes.Contains(s.Code)))
                    {
                        _mainViewModel.SalesData.Add(sale);
                    }
                });

                return new ObservableCollection<Sales>(updatedSalesData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do banco: {ex.Message}");
                return new ObservableCollection<Sales>();
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

                    var draggedIndex = _mainViewModel.SalesData.IndexOf(draggedItem);
                    var targetIndex = _mainViewModel.SalesData.IndexOf(targetItem);

                    if (draggedIndex >= 0 && targetIndex >= 0 && draggedIndex != targetIndex)
                    {
                        if (draggedIndex >= 0 && draggedIndex < _mainViewModel.SalesData.Count &&
                            targetIndex >= 0 && targetIndex < _mainViewModel.SalesData.Count)
                        {
                            _mainViewModel.SalesData.Move(draggedIndex, targetIndex);

                            var updates = new List<Task>();

                            for (int i = 0; i < _mainViewModel.SalesData.Count; i++)
                            {
                                _mainViewModel.SalesData[i].Priority = i;

                                //bool isManualValue = (SalesData[i] == draggedItem);
                                //updates.Add(databaseActions.UpdatePriorityAsync(SalesData[i].Code, isManualValue));
                                bool isManualValue = _mainViewModel.SalesData[i].IsManual || (_mainViewModel.SalesData[i] == draggedItem);
                                _database.UpdatePriorityAsync(_mainViewModel.SalesData[i].Code, isManualValue);
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
            var success = await _database.RemoveOsAsync(osCode);

            if (!success)
            {
                throw new InvalidOperationException("Falha ao remover a OS. Registro não encontrado");
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _mainViewModel.SalesData.Remove(saleToRemove);
            });
        }

        private async void PauseOs_Click(object sender, RoutedEventArgs e)
        {
            var selectedSale = DataGridSales.SelectedItem as Sales;

            if (selectedSale != null && !selectedSale.IsPaused)
            {
                var result = MessageBox.Show($"Deseja pausar a OS nº {selectedSale.Code}?",
                                              "Confirmação",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    bool sucess = await _database.PauseOsAsync(selectedSale.Code, selectedSale.Status, selectedSale.DisplayType);

                    if (sucess)
                    {
                        //Preciso adicionar uma atualização forçada de UI
                        selectedSale.IsPaused = true;
                        _mainViewModel.SalesData.First(s => s.Code == selectedSale.Code).IsPaused = true;
                    }
                }
            }
            else
            {
                bool sucess = await _database.UnpauseOsAsync(selectedSale.Code);

                if (sucess)
                {
                    //Preciso adicionar uma atualização forçada de UI
                    selectedSale.IsPaused = false;
                    _mainViewModel.SalesData.First(s => s.Code == selectedSale.Code).IsPaused = false;
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

        //private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (sender is ComboBox comboBox && comboBox.SelectedItem is string selectedStatus)
        //    {
        //        if (DataGridSales.SelectedItem is Sales selectedSale)
        //        {
        //            if (SalesUtils.IsLocalStatus(selectedStatus) || selectedStatus == "Aprovado")
        //            {
        //                selectedSale.IsStatusModified = true;
        //                Status? enumStatus = GetStatusToComboBox.ConvertDisplayToEnum<Status>(selectedStatus);

        //                if (!enumStatus.HasValue || selectedSale.Status.ToString() == enumStatus.ToString()) return;

        //                selectedSale.DisplayStatus = selectedStatus;

        //                await _database.UpdateStatusOnDatabaseAsync(selectedSale.Code, enumStatus.ToString());

        //                MessageBox.Show($"Status atualizado para {selectedStatus} na OS {selectedSale.Code}", "Status atualizado!", MessageBoxButton.OK, MessageBoxImage.Information);
        //            }
        //            else
        //            {
        //                comboBox.IsEnabled = false;
        //            }
        //        }
        //    }
        //}

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
