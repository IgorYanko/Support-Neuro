using System.Security.Cryptography.X509Certificates;
using System.Windows;
using MahApps.Metro.Controls;

namespace NeuroApp
{
    public partial class MainWindow : MetroWindow
    {
        private readonly ResponsiveBigButtons _buttons;

        public MainWindow()
        {
            InitializeComponent();
            MainViewModel mainViewModel = new();
            DataContext = mainViewModel;
            _buttons = (ResponsiveBigButtons)Application.Current.Resources["ResponsiveBigButtons"] as ResponsiveBigButtons ?? new ResponsiveBigButtons();
            SizeChanged += ContentControl_SizeChanged;

            HomeScreen homeScreen = new(mainViewModel);
            mainViewModel.CurrentView = homeScreen;
        }

        private void ContentControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _buttons.WindowWidth = ActualWidth;
            _buttons.WindowHeight = ActualHeight;
        }
    }
}