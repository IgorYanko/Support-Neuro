using System.Windows;
using System.Windows.Input;
using System.IO;
using NeuroApp.Classes;
using NeuroApp.Interfaces;
using NeuroApp.Views;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace NeuroApp
{
    public partial class LoginScreen : Window
    {
        private readonly IMainViewModel _mainViewModel;
        private readonly IConfiguration _configuration;

        public LoginScreen(IMainViewModel mainViewModel, IConfiguration configuration)
        {
            InitializeComponent();
            _mainViewModel = mainViewModel;
            _configuration = configuration;
        }

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
                MainWindow mainWindow = new MainWindow(_mainViewModel, _configuration);
                _mainViewModel.CurrentView = new HomeScreen(_mainViewModel, _configuration);
                mainWindow.Show();
                this.Close();
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

            if (RememberMeCheckBox.IsChecked == true)
            {
                try
                {
                    var authData = new
                    {
                        Username = user.UserName,
                        AuthToken = Guid.NewGuid().ToString(),
                        Function = user.Function,
                        Expiration = DateTime.Now.AddDays(30)
                    };

                    string json = JsonSerializer.Serialize(authData);

                    byte[] encryptedData = ProtectedData.Protect(
                        Encoding.UTF8.GetBytes(json),
                        null,
                        DataProtectionScope.CurrentUser);

                    File.WriteAllBytes("auth.dat", encryptedData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao salvar credenciais: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            ProcessLogin(user);
            return true;
        }

        private async Task<User?> AuthenticateUser(string username, string password)
        {
            DatabaseActions databaseActions = new(_configuration);
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

        public void ProcessLogin(User user)
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
