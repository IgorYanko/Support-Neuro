using System.Windows;
using MahApps.Metro.Controls;
using NeuroApp.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace NeuroApp.Views
{
    public partial class MainWindow : MetroWindow
    {
        private readonly ResponsiveBigButtons _buttons;
        private readonly IMainViewModel _mainViewModel;
        private readonly IConfiguration _configuration;

        public MainWindow(IMainViewModel mainViewModel, IConfiguration configuration)
        {
            InitializeComponent();
            
            _mainViewModel = mainViewModel;
            _configuration = configuration;
            DataContext = _mainViewModel;
            _buttons = (ResponsiveBigButtons)Application.Current.Resources["ResponsiveBigButtons"] as ResponsiveBigButtons ?? new ResponsiveBigButtons();
            SizeChanged += ContentControl_SizeChanged;

            HomeScreen homeScreen = new(_mainViewModel, _configuration);
            LoadingScreen loadingScreen = new();

            _mainViewModel.CurrentView = loadingScreen;
        }

        private void ContentControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _buttons.WindowWidth = ActualWidth;
            _buttons.WindowHeight = ActualHeight;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Application.Current.Shutdown();
        }
    }
}