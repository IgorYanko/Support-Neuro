using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;
using NeuroApp.Classes;
using NeuroApp.Interfaces;

namespace NeuroApp
{
    public partial class WarrantyScreen : UserControl
    {
        private readonly IConfiguration _configuration;
        private readonly IMainViewModel _mainViewModel;
        private readonly WarrantyManager _warrantyManager;
        public ObservableCollection<Warranty> Warranties { get; set; }
        private bool _isLoading = false;

        public WarrantyScreen(IMainViewModel mainViewModel, IConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration;
            _mainViewModel = mainViewModel;
            _warrantyManager = new WarrantyManager(_configuration);
            Warranties = new ObservableCollection<Warranty>();
            DataContext = this;
            LoadWarrantiesAsync();
        }

        private async void LoadWarrantiesAsync()
        {
            try
            {
                ShowLoading(true);
                var warranties = await _warrantyManager.GetAllWarrantiesAsync();
                
                foreach (var warranty in warranties)
                {
                    warranty.WarrantyEndDate = warranty.ServiceDate.AddMonths(warranty.WarrantyMonths);
                }

                Warranties.Clear();
                foreach (var warranty in warranties)
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
            var dialog = new WarrantyDetailsDialog(warranty, _warrantyManager);
            if (dialog.ShowDialog() == true)
            {
                LoadWarrantiesAsync();
            }
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
                    _warrantyManager.DeleteWarrantyByOsCodeAsync(parameter);
                }
                else if (parameter == warranty.SerialNumber)
                {
                    _warrantyManager.DeleteWarrantyAsync(parameter);
                }

                LoadWarrantiesAsync();
            }
            else
            {
                MessageBox.Show("Nenhuma garantia selecionada.");
            }
        }
        
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (WarrantyDataGrid.SelectedItem is Warranty selectedWarranty)
            {
                ShowWarrantyDetailsDialog(selectedWarranty);
            }
        }
    }
} 