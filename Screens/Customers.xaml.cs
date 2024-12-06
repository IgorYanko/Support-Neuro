using Microsoft.Extensions.Configuration;
using NeuroApp.Api;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using NeuroApp.Classes;

namespace NeuroApp.Screens
{
    public partial class Customers : UserControl
    {

        public Customers()
        {
            InitializeComponent();
        }

        public async Task FillListViewAsync()
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

                var apiService = new SensioApiService(configuration);
                var endpoint = "people";

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var response = await apiService.GetDataAsync(endpoint);
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response, options);
                var clientes = apiResponse.Response;             

                ClientesListView.ItemsSource = clientes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao preencher ListView: {ex.Message}");
            }

        }

        private void ClientesListView_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (ClientesListView.SelectedItem != null)
            {
                var selectedClient = ClientesListView.SelectedItem.ToString();

                ClientFrame.Navigate(new ClientPage(selectedClient));
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await FillListViewAsync();
        }
    }
}