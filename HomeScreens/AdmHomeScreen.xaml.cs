using NeuroApp.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeuroApp
{
    /// <summary>
    /// Interação lógica para HomeScreen.xam
    /// </summary>
    public partial class AdmHomeScreen : UserControl
    {
        public ResponsiveBigButtons responsiveBigButtons { get; set; }
        public AdmHomeScreen()
        {
            InitializeComponent();
            responsiveBigButtons = new ResponsiveBigButtons();
            DataContext = Application.Current.MainWindow.DataContext;

            responsiveBigButtons.WindowHeight = this.Height;
            responsiveBigButtons.WindowWidth = this.Width;

            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            responsiveBigButtons.WindowHeight = this.ActualHeight;
            responsiveBigButtons.WindowWidth = this.ActualWidth;
        }

        private MainViewModel mainViewModel = new();

        private void OpenCockpit(object sender, RoutedEventArgs e)
        {
            mainViewModel.CurrentView = new Cockpit();
        }

        private void OpenCustomers(object sender, RoutedEventArgs e)
        {
            mainViewModel.CurrentView = new Customers();
        }

        private void OpenFinances(object sender, RoutedEventArgs e)
        {
            mainViewModel.CurrentView = new Finances();
        }

        private void OpenServiceOrders(object sender, RoutedEventArgs e)
        {
            mainViewModel.CurrentView = new ServiceOrders();
        }
    }
}
