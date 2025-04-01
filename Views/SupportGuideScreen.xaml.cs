using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using NeuroApp.Classes;
using Microsoft.Extensions.Configuration;
using NeuroApp.Interfaces;
using NeuroApp.Views;

namespace NeuroApp
{
    public partial class SupportGuideScreen : UserControl
    {
        private ResponsiveBigButtons _buttons {  get; set; }
        private readonly IConfiguration _configuration;
        private readonly IMainViewModel _mainViewModel;

        public SupportGuideScreen(IConfiguration configuration, IMainViewModel mainViewModel)
        {
            InitializeComponent();
            _configuration = configuration;
            _mainViewModel = mainViewModel;
            DataContext = _buttons;
        }

        private async void OpenGuide_Click(object sender, RoutedEventArgs e)
        {  
            if (sender is Button button && button.Tag is string title)
            {
                var dialog = new SupportGuideDialog(title);
                await DialogHost.Show(dialog, "RootDialog"); 
            }
        }

        private async void OpenStaticGuide_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string title)
            {
                var dialog = new SupportStaticGuideDialog(title);
                await DialogHost.Show(dialog, "RootDialog");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mainViewModel.ShowHomeScreen();
        }
    }
} 