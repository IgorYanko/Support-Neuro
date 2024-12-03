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

namespace NeuroApp
{
    /// <summary>
    /// Lógica interna para TelaLogin.xaml
    /// </summary>
    public partial class LoginScreen : UserControl
    {
        public LoginScreen()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                TextBlock placeholder = (TextBlock)textBox.Tag;

                placeholder.Visibility = string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                TextBlock placeholder = (TextBlock)passwordBox.Tag;
                placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Hidden;
                
                passwordTextBox.Text = passwordBox.Password;
            }
        }

        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            bool isChecked = (sender as CheckBox)?.IsChecked == true;

            passwordTextBox.Visibility = isChecked ? Visibility.Visible : Visibility.Collapsed;
            txtPassword.Visibility = isChecked ? Visibility.Collapsed : Visibility.Visible;

            if (isChecked)
            {
                passwordTextBox.Text = txtPassword.Password;
            }
            else
            {
                txtPassword.Password = passwordTextBox.Text;
            }
        }


        private void Login(object sender, RoutedEventArgs e)
        {
            MainViewModel mainViewModel = (MainViewModel)this.DataContext;

            User user = new();
            user.UserName = txtUser.Text;
            user.Password = txtPassword.Password;

            DatabaseActions databaseActions = new();
            bool loginSuccess = databaseActions.userLogin(user);

            if (/*loginSuccess*/1 > 0) mainViewModel.CurrentView = new AdmHomeScreen();
            else MessageBox.Show("Credenciais Incorretas!");
            txtUser.Clear();
            txtPassword.Clear();
            passwordTextBox.Clear();
        }

    }
}
