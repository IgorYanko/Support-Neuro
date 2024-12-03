using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace NeuroApp
{
    public partial class MainWindow : Window
    {
        private ResponsiveWindow responsiveWindow;

        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainViewModel();
            DataContext = viewModel;

            responsiveWindow = new ResponsiveWindow();

            this.SizeChanged += (s, e) =>
            {
                responsiveWindow.responsiveBigButtons.WindowHeight = this.ActualHeight;
                responsiveWindow.responsiveBigButtons.WindowWidth = this.ActualWidth;
                viewModel._ResponsiveBigButtons = responsiveWindow.responsiveBigButtons;
            };
        }
    }
}