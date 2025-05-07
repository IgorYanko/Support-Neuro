using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeuroApp.Interfaces;
using NeuroApp.Services;
using NeuroApp.Views;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NeuroApp
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _configuration;
        private IMainViewModel _mainViewModel;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<IApiService, SensioApiService>();

            services.AddSingleton<IMainViewModel, MainViewModel>();

            services.AddTransient<HomeScreen>();
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginScreen>();

            LoggingService.LogInformation("Serviços configurados com sucesso");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (File.Exists("auth.dat"))
            {
                try
                {
                    byte[] encryptedData = File.ReadAllBytes("auth.dat");
                    byte[] decryptedData = ProtectedData.Unprotect(
                        encryptedData,
                        null,
                        DataProtectionScope.CurrentUser);
                    string json = Encoding.UTF8.GetString(decryptedData);

                    var authData = JsonSerializer.Deserialize<dynamic>(json);
                    string username = authData.GetProperty("Username").GetString();
                    string authToken = authData.GetProperty("AuthToken").GetString();
                    string function = authData.GetProperty("Function").GetInt32().ToString();
                    DateTime expiration = authData.GetProperty("Expiration").GetDateTime();

                    if (DateTime.Now < expiration && ValidateToken(username, authToken))
                    {
                        if (!Enum.TryParse<User.UserFunction>(function, out var userFunction))
                        {
                            userFunction = User.UserFunction.generic;
                        }

                        LoginScreen loginScreen = new(_mainViewModel ,_configuration);

                        var user = new User(username, userFunction);
                        loginScreen.ProcessLogin(user);

                        var mainWindow = _serviceProvider.GetService<MainWindow>();
                        mainWindow?.Show();
                    }
                    else
                    {
                        var loginWindow = _serviceProvider.GetService<LoginScreen>();
                        loginWindow?.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao autenticar automaticamente: {ex.Message}");
                    var loginWindow = _serviceProvider.GetService<LoginScreen>();
                    loginWindow?.Show();
                }
            }
            else
            {
                var loginWindow = _serviceProvider.GetService<LoginScreen>();
                loginWindow?.Show();
            }

        }

        private bool ValidateToken(string username, string authToken)
        {
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(authToken);
        }



        protected override void OnExit(ExitEventArgs e)
        {
            LoggingService.LogInformation("Encerrando aplicação NeuroApp");
            base.OnExit(e);
        }
    }
}
