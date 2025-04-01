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
        private List<Person> _clientes = new();
        private List<PersonType> _tipos = new();

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
                
                if (apiResponse?.Response != null)
                {
                    _clientes = apiResponse.Response
                                .Where(p => p.Type != null && p.Type.Contains(PersonType.Cliente))
                                .ToList();
                }
                
                ClientesListView.ItemsSource = _clientes;

                _tipos = Enum.GetValues(typeof(PersonType)).Cast<PersonType>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao preencher ListView: {ex.Message}");
            }
        }

        private void ClientesListView_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (ClientesListView.SelectedItem is Person selectedClient)
            {
                ClientFrame.Navigate(new ClientPage(selectedClient));
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await FillListViewAsync();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchString = SearchBox.Text;

            if (_clientes.Count == 0) return;

            var filteredClients = _clientes
                .Where(c => c.Name != null &&
                            c.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                .ToList();

            ClientesListView.ItemsSource = filteredClients;
        }
    }
}