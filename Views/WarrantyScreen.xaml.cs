using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using NeuroApp.Classes;
using NeuroApp.Interfaces;

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

        public WarrantyScreen(IMainViewModel mainViewModel, IConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration;
            _mainViewModel = mainViewModel;
            _actions = new DatabaseActions(_configuration);
            Warranties = new ObservableCollection<Warranty>();
            DataContext = this;
            LoadWarrantiesAsync();

            _searchTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _searchTimer.Tick += DebouncerTimer_Tick;
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
            string searchText = SearchBar.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                Dispatcher.Invoke(() => WarrantyDataGrid.ItemsSource = oldWarranties);
            }
            else if (searchText.Length >= 2)
            {
                var results = await _actions.GetWarrantyBySearchAsync(searchText);
                Dispatcher.Invoke(() => WarrantyDataGrid.ItemsSource = results);
            }
            else
            {
                Dispatcher.Invoke(() => WarrantyDataGrid.ItemsSource = null);
            }
        }

        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }
    }
} 