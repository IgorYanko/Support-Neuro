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
using Serilog;
using NeuroApp.Classes;
using System.Text.Json.Serialization;
using System;

namespace NeuroApp
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _configuration;
        private IMainViewModel _mainViewModel;
        private ICacheService _cacheService;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            _mainViewModel = _serviceProvider.GetRequiredService<IMainViewModel>();
            _cacheService = _serviceProvider.GetRequiredService<ICacheService>();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _configuration = configuration;

            services.AddSingleton<IConfiguration>(configuration);

            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<IApiService, SensioApiService>();
            services.AddSingleton<IDatabaseActions, DatabaseActions>();
            services.AddSingleton<IMainViewModel, MainViewModel>();

            services.AddTransient<HomeScreen>();
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginScreen>();
            services.AddTransient<LoadingScreen>();

            LoggingService.LogInformation("Serviços configurados com sucesso");
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = (Exception)args.ExceptionObject;
                Log.Fatal(exception, "Exceção não tratada encerrou a aplicação");
                Log.CloseAndFlush();
            };

            DispatcherUnhandledException += (sender, args) =>
            {
                Log.Error(args.Exception, "Exceção não tratada na thread da UI");
                args.Handled = true;
            };

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

                        var loginScreen = new LoginScreen(_mainViewModel ,_configuration);
                        var user = new User(username, userFunction);
                        await loginScreen.ProcessLogin(user);
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

        public async Task PreloadSalesDataAsync()
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            LoadingScreen loadingScreen = new();
            HomeScreen homeScreen = _serviceProvider.GetService<HomeScreen>();
            mainWindow?.Show();
            _mainViewModel.CurrentView = loadingScreen;

            try
            {
                await loadingScreen.UpdateMessageAsync("Conectando ao banco de dados...");

                var database = new DatabaseActions(_configuration);
                
                var apiService = _serviceProvider.GetRequiredService<IApiService>();
                
                var response = await apiService.GetDataAsync("sales/list/1");

                await loadingScreen.UpdateMessageAsync("Buscando dados da Api...");

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };

                var apiResponse = JsonSerializer.Deserialize<ApiResponseSales>(response, options);

                if (apiResponse?.Response == null)
                {
                    throw new InvalidOperationException("API response is null or invalid.");
                }

                var deletedOsCodesSet = await database.GetDeletedOsCodesAsync();
                await _cacheService.SetAsync("DeletedOsCodes", deletedOsCodesSet);

                await loadingScreen.UpdateMessageAsync("Processando dados...");

                var filteredSales = apiResponse.Response
                    .Where(sale =>
                        sale.DateCreated >= DateTime.UtcNow.AddDays(-30) &&
                        sale.DateCreated <= DateTime.UtcNow &&
                        !deletedOsCodesSet.Contains(sale.Code))
                    .ToList();

                var cachedSales = await database.GetSalesFromDatabaseAsync();
                var cachedSalesDict = cachedSales.ToDictionary(s => s.Code);

                var tasks = new List<Task>();

                foreach (var apiSale in filteredSales)
                {
                    if (cachedSalesDict.TryGetValue(apiSale.Code, out var cachedSale))
                    {
                        if (cachedSale.Excluded) continue;

                        if (cachedSale.Status != apiSale.Status)
                        {
                            cachedSale.Status = apiSale.Status;
                            cachedSale.IsStatusModified = false;
                            tasks.Add(database.VerifyAndSave(apiSale));
                        }
                    }
                    else if (apiSale.Status.ToString() != "Faturado" && !apiSale.Excluded)
                    {
                        tasks.Add(database.VerifyAndSave(apiSale));
                    }
                }

                await Task.WhenAll(tasks);

                await _cacheService.SetAsync("cachedSales", cachedSales);

                await SalesCacheManager.SaveCacheAsync(new SalesCacheData
                {
                    Sales = cachedSales,
                    DeletedOsCodes = deletedOsCodesSet
                });

                await loadingScreen.UpdateMessageAsync("Finalizando...");
                _mainViewModel.CurrentView = homeScreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no pré-carregamento: {ex.Message}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
