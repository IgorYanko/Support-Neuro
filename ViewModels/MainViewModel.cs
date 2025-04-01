using System.Windows.Input;
using Microsoft.Extensions.Configuration;
using NeuroApp.Interfaces;
using NeuroApp.Views;
using NeuroApp.Services;
using NeuroApp.Classes;

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

        public ICommand ShowCockpitScreen_ { get; }
        public ICommand ShowCustomersScreen_ { get; }

        public MainViewModel(IApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
            LoggingService.LogInformation("MainViewModel inicializado");

            GoBackCommand = new RelayCommand(
                ShowHomeScreen,
                () => CurrentView != null
            );

            ShowCockpitScreen_ = new RelayCommand(() => CurrentView = new Cockpit(this, _configuration));
            ShowCustomersScreen_ = new RelayCommand(() => CurrentView = new Screens.Customers());
            ShowSupportGuideCommand = new RelayCommand(() => CurrentView = new SupportGuideScreen(_configuration, this));
            ShowWarrantyScreenCommand = new RelayCommand(() => CurrentView = new WarrantyScreen(this, _configuration));

            ShowHomeScreen();
        }

        public void ShowHomeScreen()
        {
            LoggingService.LogDebug("Navegando para a HomeScreen");
            CurrentView = new HomeScreen(this, _configuration);
        }
    }
} 