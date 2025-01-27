using NeuroApp.Screens;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;

namespace NeuroApp
{
    public partial class HomeScreen : UserControl
    {
        //public ResponsiveBigButtons responsiveBigButtons { get; }

        public HomeScreen(MainViewModel mainView)
        {
            InitializeComponent();

            var responsiveBigButtons = new ResponsiveBigButtons
            {
                WindowHeight = this.Height,
                WindowWidth = this.Width
            };

            this.DataContext = new HomeScreenViewModel(mainView, responsiveBigButtons);

            this.SizeChanged += (s, e) =>
            {
                responsiveBigButtons.WindowHeight = this.ActualHeight;
                responsiveBigButtons.WindowWidth = this.ActualWidth;
            };
        }
    }

    public class HomeScreenViewModel
    {
        public MainViewModel MainViewModel { get; }
        public ResponsiveBigButtons ResponsiveBigButtons { get; }

        public HomeScreenViewModel(MainViewModel mainViewModel, ResponsiveBigButtons responsiveBigButtons)
        {
            MainViewModel = mainViewModel;
            ResponsiveBigButtons = responsiveBigButtons;
        }
    }
}
