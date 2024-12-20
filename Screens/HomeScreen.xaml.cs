using NeuroApp.Screens;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NeuroApp
{
    /// <summary>
    /// Interação lógica para HomeScreen.xam
    /// </summary>
    public partial class HomeScreen : UserControl
    {
        private readonly MainViewModel _mainViewModel;

        public ResponsiveBigButtons responsiveBigButtons { get; set; }

        public HomeScreen(MainViewModel mainView)
        {
            InitializeComponent();

            _mainViewModel = mainView;
            DataContext = _mainViewModel;

            responsiveBigButtons = new ResponsiveBigButtons
            {
                WindowHeight = this.Height,
                WindowWidth = this.Width
            };

            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            responsiveBigButtons.WindowHeight = this.ActualHeight;
            responsiveBigButtons.WindowWidth = this.ActualWidth;
        }

        private void OpenCockpit(object sender, RoutedEventArgs e)
        {
            _mainViewModel.CurrentView = new Cockpit();
        }

        private void OpenCustomers(object sender, RoutedEventArgs e)
        {
            _mainViewModel.CurrentView = new Customers();
        }

        private void OpenFinances(object sender, RoutedEventArgs e)
        {
            _mainViewModel.CurrentView = new Finances();
        }

        private void OpenServiceOrders(object sender, RoutedEventArgs e)
        {
            _mainViewModel.CurrentView = new ServiceOrders();
        }
    }
}
