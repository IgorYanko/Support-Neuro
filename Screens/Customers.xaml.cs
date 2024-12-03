using Microsoft.Extensions.Configuration;
using NeuroApp.Api;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Extensions.Configuration;

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
                .AddJsonFile("appsettings.json").Build();

                var apiService = new SensioApiService(configuration);
                var endpoint = "/people";

                var response = await apiService.GetDataAsync(endpoint);
                var clientes = JsonSerializer.Deserialize<List<Person>>(response);

                Console.WriteLine($"Resposta da API: {response}");

                ClientesListView.ItemsSource = clientes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao preencher ListView: {ex.Message}");
            }

        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await FillListViewAsync();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ClientesListView.SelectedItem != null)
            {
                var cliente = (Api.Person)ClientesListView.SelectedItem;
                ClienteWebView.Source = new Uri($"");
            }
        }
    }
}