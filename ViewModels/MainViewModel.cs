using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using NeuroApp.Interfaces;
using NeuroApp.Views;
using NeuroApp.Services;
using NeuroApp.Classes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace NeuroApp
{
    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _configuration;

        public ICommand GoBackCommand { get; }
        public ICommand ShowSupportGuideCommand { get; }
        public ICommand ShowWarrantyScreenCommand { get; }

        private ResponsiveBigButtons _responsiveBigButtons = new();
        public ResponsiveBigButtons ResponsiveBigButtons
        {
            get => _responsiveBigButtons;
            set => SetProperty(ref _responsiveBigButtons, value);
        }

        private object _currentView = new();
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        private ObservableCollection<Sales> _sales;
        public ObservableCollection<Sales> Sales
        {
            get => _sales;
            set => SetProperty(ref _sales, value);
        }

        public ICommand ShowObservationsCommand { get; }
        public ICommand ShowCockpitScreen { get; }
        public ICommand ShowCustomersScreen { get; }

        public MainViewModel(IApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;

            GoBackCommand = new RelayCommand(
                ShowHomeScreen,
                () => CurrentView != null
            );

            ShowCockpitScreen = new RelayCommand(() => CurrentView = new Cockpit(this, _configuration));
            ShowCustomersScreen = new RelayCommand(() => CurrentView = new Screens.Customers());
            ShowSupportGuideCommand = new RelayCommand(() => CurrentView = new SupportGuideScreen(_configuration, this));
            ShowWarrantyScreenCommand = new RelayCommand(() => CurrentView = new WarrantyScreen(this, _configuration));
            ShowObservationsCommand = new RelayCommand<Sales>(ShowObservations);

            ShowHomeScreen();
        }

        private Sales _selectedSale;
        public Sales SelectedSale
        {
            get => _selectedSale;
            set
            {
                _selectedSale = value;
                OnPropertyChanged(nameof(SelectedSale));
            }
        }

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set
            {
                _isPopupOpen = value;
                OnPropertyChanged(nameof(IsPopupOpen));
            }
        }

        private void ShowObservations(Sales sale)
        {
            if (sale == null) return;

            SelectedSale = sale;
            IsPopupOpen = true;
        }

        public void ShowHomeScreen()
        {
            CurrentView = new HomeScreen(this, _configuration);
        }
    }
} 