using System.Configuration;
using System.Data;
using System.Windows;

namespace NeuroApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainViewModel mainViewModel = new();

            LoginScreen loginScreen = new(mainViewModel);

            loginScreen.Show();
        }
    }

}
