using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NeuroApp.Interfaces;
using NeuroApp.Services;
using NeuroApp.Views;

namespace NeuroApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
            // Inicializa o sistema de logging
            LoggingService.Initialize();
            LoggingService.LogInformation("Iniciando aplicação NeuroApp");

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Configuração
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Serviços
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<IApiService, SensioApiService>();

            // ViewModels
            services.AddSingleton<IMainViewModel, MainViewModel>();

            // Views
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginScreen>();

            LoggingService.LogInformation("Serviços configurados com sucesso");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var loginScreen = _serviceProvider.GetService<LoginScreen>();
            loginScreen.Show();

            LoggingService.LogInformation("Janela de login exibida");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LoggingService.LogInformation("Encerrando aplicação NeuroApp");
            base.OnExit(e);
        }
    }
}
