using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using NeuroApp.Classes;


namespace NeuroApp
{
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

        /// <summary>
        /// Parte relacionada ao tema será ignorada na primeira versão
        /// </summary>
        //public bool IsDarkTheme { get; set; }
        //private readonly PaletteHelper _paletteHelper = new();
        
        //private void toggleTheme(object sender, RoutedEventArgs e)
        //{
        //    ITheme theme = _paletteHelper.GetTheme();

        //    if (IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark)
        //    {
        //        IsDarkTheme = false;
        //        theme.SetBaseTheme(Theme.Light);
        //    }
        //    else
        //    {
        //        IsDarkTheme = true;
        //        theme.SetBaseTheme(Theme.Dark);
        //    }
            
        //    _paletteHelper.SetTheme(theme);
        //}

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        protected void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (await Login())
            {
                new MainWindow().Show();
                Close();
            }
            else
            {
                txtUser.Clear();
                txtPassword.Clear();
            }
        }

        private async Task<bool> Login()
        {
            string username = txtUser.Text;
            string password = txtPassword.Password;

            if (!AreCredentialsValid(username, password))
                return false;

            User? user = await AuthenticateUser(username, password);
            if (user == null)
            {
                MessageBox.Show("Credenciais incorretas!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            ProcessLogin(user);
            return true;
        }

        private async Task<User?> AuthenticateUser(string username, string password)
        {
            DatabaseActions databaseActions = new();
            var (loginSuccess, userRole) = await databaseActions.UserLoginAsync(username, password);

            if (!loginSuccess) return null;

            User.UserFunction function = userRole switch
            {
                "high" => User.UserFunction.high,
                "mid" => User.UserFunction.mid,
                "low" => User.UserFunction.low,
                _ => User.UserFunction.generic
            };

            return new User(username, "password", function);
        }

        private void ProcessLogin(User user)
        {
            PermissionSystem.Instance.SetCurrentUser(user);
        }

        private bool AreCredentialsValid(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("O campo de usuário não pode estar vazio!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("O campo de senha não pode estar vazio!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
