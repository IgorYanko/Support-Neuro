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
using System.Text.Json.Serialization;
using System.Text.Json;

namespace NeuroApp
{
    public class MainViewModel : BaseViewModel, IMainViewModel
    {
        private readonly IApiService _apiService;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        private readonly IDatabaseActions _database;
        public ObservableCollection<Sales> SalesData { get; set; } = new();

        public ICommand GoBackCommand { get; }
        public ICommand ShowSupportGuideCommand { get; }
        public ICommand ShowWarrantyScreenCommand { get; }
        public ICommand ShowObservationsCommand { get; }
        public ICommand ShowCockpitScreen { get; }
        public ICommand ShowCustomersScreen { get; }
        public ICommand ClosePopupCommand { get; }
        public ICommand UpdateCommand => new RelayCommand(async () =>
        {
            if (CurrentView is Cockpit cockpit)
            {
                await cockpit.UpdateDataAsync();
            }
        });

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

        private ObservableCollection<Sales> _sales = new();
        public ObservableCollection<Sales> Sales
        {
            get => _sales;
            set { _sales = value; OnPropertyChanged(nameof(Sales)); }
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

        public MainViewModel(IApiService apiService, IConfiguration configuration, ICacheService cacheService, IDatabaseActions database)
        {
            _apiService = apiService;
            _configuration = configuration;
            _cacheService = cacheService;
            _database = database;

            GoBackCommand = new RelayCommand(
                ShowHomeScreen,
                () => CurrentView != null
            );

            ShowCockpitScreen = new RelayCommand(() => CurrentView = new Cockpit(this, _configuration, _apiService, _cacheService, _database));
            ShowCustomersScreen = new RelayCommand(() => CurrentView = new Screens.Customers());
            ShowSupportGuideCommand = new RelayCommand(() => CurrentView = new SupportGuideScreen(_configuration, this));
            ShowWarrantyScreenCommand = new RelayCommand(() => CurrentView = new WarrantyScreen(this, _configuration));
            ShowObservationsCommand = new RelayCommand<Sales>(ShowObservations);

            ShowHomeScreen();
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