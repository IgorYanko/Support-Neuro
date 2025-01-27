using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NeuroApp.Classes;


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
        private bool ProcessLogin(string username, string password)
        {
            User user = new() { UserName = username, Password = password };
            DatabaseActions databaseActions = new();

            var (loginSuccess, userRole) = databaseActions.userLogin(user);

            if (!loginSuccess)
            {
                MessageBox.Show("Credenciais incorretas!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            PermissionSystem.Instance.SetCurrentUser(new User
            {
                UserName = username,
                function = Enum.Parse<User.Function>(userRole)
            });

            return true;
        }

        private bool AreCredentialsValid(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("O campo de usuário não pode estar vazio!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("O campo de senha não pode estar vazio!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private bool Login()
        {
            string username = txtUser.Text;
            string password = txtPassword.Password;

            if (!AreCredentialsValid(username, password))
                return false;

            return ProcessLogin(username, password);
        }

    }
}
