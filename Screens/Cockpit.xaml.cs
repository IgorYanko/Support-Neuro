using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using NeuroApp.Api;
using NeuroApp.Classes;

namespace NeuroApp
{
    public partial class Cockpit : UserControl
    {
        public ObservableCollection<Sales> SalesData { get; set; } = new ObservableCollection<Sales>();

        public Cockpit()
        {
            InitializeComponent();
        }

        public async Task<ObservableCollection<Sales>> FetchSalesDataAsync()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

                var apiService = new SensioApiService(configuration);
                var endpoint = "sales/list/1";

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                };

                var response = await apiService.GetDataAsync(endpoint);
                var apiResponse = JsonSerializer.Deserialize<ApiResponseSales>(response, options);

                DateTime maxDate = DateTime.Today.AddDays(-30);

                var filteredSales = apiResponse.Response
                    .Where(sale => sale.DateCreated >= maxDate && sale.DateCreated <= DateTime.Today)
                    .ToList();

                return new ObservableCollection<Sales>(filteredSales ?? new List<Sales>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao preencher DataGrid: {ex.Message}");
                return new ObservableCollection<Sales>();
            }
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SalesData = await FetchSalesDataAsync();
            DataContext = this;
        }

        //private Color ChangeBackgroundColor()
        //{
        //    Color green = Color.FromArgb(255,60,179,113);
        //    Color yellow = Color.FromArgb(255,253,233,16);
        //    Color red = Color.FromArgb(255,227,38,54);
        //}
    }
}
