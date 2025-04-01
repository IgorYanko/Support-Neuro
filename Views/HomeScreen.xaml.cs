using NeuroApp.Screens;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using NeuroApp.Interfaces;
using Microsoft.Extensions.Configuration;

namespace NeuroApp
{
    public partial class HomeScreen : UserControl
    {
        private readonly IMainViewModel _mainViewModel;
        private readonly IConfiguration _configuration;

        public HomeScreen(IMainViewModel mainView, IConfiguration configuration)
        {
            InitializeComponent();
            _mainViewModel = mainView;
            _configuration = configuration;

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
        public IMainViewModel MainViewModel { get; }
        public ResponsiveBigButtons ResponsiveBigButtons { get; }

        public HomeScreenViewModel(IMainViewModel mainViewModel, ResponsiveBigButtons responsiveBigButtons)
        {
            MainViewModel = mainViewModel;
            ResponsiveBigButtons = responsiveBigButtons;
        }
    }
}
