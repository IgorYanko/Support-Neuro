using System.Security.Cryptography.X509Certificates;
using System.Windows;
using MahApps.Metro.Controls;

namespace NeuroApp
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel mainViewModel = new();
            DataContext = mainViewModel;

            HomeScreen homeScreen = new(mainViewModel);
            mainViewModel.CurrentView = homeScreen;
        }
    }
}