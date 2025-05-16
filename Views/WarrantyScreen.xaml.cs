using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using NeuroApp.Classes;
using NeuroApp.Interfaces;
using NeuroApp.Services;
using Serilog;

namespace NeuroApp
{
    public partial class WarrantyScreen : UserControl
    {
        private readonly IConfiguration _configuration;
        private readonly IMainViewModel _mainViewModel;
        private readonly DispatcherTimer _searchTimer;
        private DatabaseActions _actions;
        public ObservableCollection<Warranty> Warranties { get; set; }
        private bool _isLoading = false;
        private readonly Serilog.ILogger _logger;

        public WarrantyScreen(IMainViewModel mainViewModel, IConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration;
            _mainViewModel = mainViewModel;

            _logger = Log.ForContext<WarrantyScreen>();
            _logger.Information("WarrantyScreen inicializado");

            _actions = new DatabaseActions(_configuration);
            Warranties = new ObservableCollection<Warranty>();
            DataContext = this;

            _searchTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _searchTimer.Tick += DebouncerTimer_Tick;

            LoadWarrantiesAsync();
        }

        private ObservableCollection<Warranty> oldWarranties = new ObservableCollection<Warranty>();

        private async void LoadWarrantiesAsync()
        {
            try
            {
                ShowLoading(true);
                var warranties = await _actions.GetAllWarrantiesAsync();
                
                oldWarranties.Clear();
                foreach (var warranty in warranties)
                {
                    warranty.WarrantyEndDate = warranty.ServiceDate.AddMonths(warranty.WarrantyMonths);
                    oldWarranties.Add(warranty);
                }

                Warranties.Clear();
                foreach (var warranty in oldWarranties)
                {
                    Warranties.Add(warranty);
                }
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

        private void WarrantyDataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_isLoading) return;

            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                if (scrollViewer.ScrollableWidth > 0)
                {
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                }
                else
                {
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ShowHomeScreen();
        }

        private void ShowWarrantyDetailsDialog(Warranty warranty)
        {
            var dialog = new WarrantyDetailsDialog(warranty, _configuration);

            dialog.OnClosedNotification += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    LoadWarrantiesAsync();
                });
            };

            dialog.ShowDialog();
        }

        private void NewWarrantyButton_Click(object sender, RoutedEventArgs e)
        {
            var newWarranty = new Warranty
            {
                ServiceDate = DateTime.Now,
                WarrantyMonths = 3
            };

            ShowWarrantyDetailsDialog(newWarranty);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (WarrantyDataGrid.SelectedItem is Warranty warranty)
            {
                string osCode = string.IsNullOrEmpty(warranty.OsCode) ? null : osCode = warranty.OsCode;

                var parameter = osCode ?? warranty.SerialNumber;

                if (parameter == warranty.OsCode)
                {
                    _actions.DeleteWarrantyByOsCodeAsync(parameter);

                    Dispatcher.Invoke(() =>
                    {
                        LoadWarrantiesAsync();
                    });
                }
                else if (parameter == warranty.SerialNumber)
                {
                    _actions.DeleteWarrantyAsync(parameter);

                    Dispatcher.Invoke(() =>
                    {
                        LoadWarrantiesAsync();
                    });
                }

                LoadWarrantiesAsync();
            }
            else
            {
                MessageBox.Show("Nenhuma garantia selecionada");
            }
        }
        
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (WarrantyDataGrid.SelectedItem is Warranty selectedWarranty)
            {
                ShowWarrantyDetailsDialog(selectedWarranty);
            }
        }

        private async void DebouncerTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();

            try
            {
                var searchText = SearchBar.Text.Trim();

                if (string.IsNullOrEmpty(searchText))
                {
                    await ResetToFullListAsync();
                    return;
                }

                if (searchText.Length < 2)
                {
                    return;
                }

                var stopwatch = Stopwatch.StartNew();
                var results = await _actions.GetWarrantyBySearchAsync(searchText, _token.Token);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    WarrantyDataGrid.ItemsSource = results;
                });
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Busca cancelada!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Erro ao realizar busca", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private CancellationTokenSource _token;
        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _logger.Debug("TextChanged disparado. Texto: {Text}", SearchBar.Text);

                _token?.Cancel();
                _token = new CancellationTokenSource();

                _searchTimer.Stop();

                if (string.IsNullOrEmpty(SearchBar.Text))
                {
                    _logger.Debug("Texto vazio - resetando para lista completa");
                    await ResetToFullListAsync();
                    return;
                }

                _searchTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ResetToFullListAsync()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    WarrantyDataGrid.ItemsSource = oldWarranties;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 