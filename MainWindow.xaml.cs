using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StackPanel stackPanel = new();
            Image logoNeuro = new();

            logoNeuro.Source = new BitmapImage(new Uri("", UriKind.Relative));

            stackPanel.Children.Add(logoNeuro);
            stackPanel.Children.Add(new TextBox { Text = "User" });
            stackPanel.Children.Add(new TextBox { Text = "Senha" });
            stackPanel.Children.Add(new Button { Content = "Login" });

            this.Content = stackPanel;
        }
    }
}