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
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace NeuroApp
{
    /// <summary>
    /// Tela inicial para Login no app
    /// </summary>
    public partial class LoginScreen : Window
    {
        private readonly MainViewModel _mainViewModel;

        public LoginScreen()
        {
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;
        }

        public LoginScreen(MainViewModel mainViewModel)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
        }

        public bool IsDarkTheme { get; set; }
        private readonly PaletteHelper _paletteHelper = new();

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void toggleTheme(object sender, RoutedEventArgs e)
        {
            ITheme theme = _paletteHelper.GetTheme();

            if (IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
            {
                IsDarkTheme = false;
                theme.SetBaseTheme(Theme.Light);
            }
            else
            {
                IsDarkTheme = true;
                theme.SetBaseTheme(Theme.Dark);
            }
            
            _paletteHelper.SetTheme(theme);
        }

        protected void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (Login())
            {
                MainWindow mainWindow = new();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                txtUser.Clear();
                txtPassword.Clear();
            }
        }

        private bool Login()
        {
            string username = txtUser.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Preencha os campos de Login!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            User user = new() { UserName = username, Password = password};
            DatabaseActions databaseActions = new();

            bool loginSuccess = databaseActions.userLogin(user);

            if (/*!loginSuccess*/ 1 < 0)
            {
                MessageBox.Show("Credenciais incorretas!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }
    }
}
