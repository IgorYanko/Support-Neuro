using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.Configuration;
using NeuroApp.Api;

namespace NeuroApp
{
    public partial class Cockpit : UserControl
    {
        private readonly SensioApiService _apiService;

        public Cockpit()
        {
            InitializeComponent();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _apiService = new SensioApiService(configuration);

        }

        private void OpenPopup(object sender, RoutedEventArgs e)
        {
            PopupOs.IsOpen = true;
        }

        private void CreateOs(object sender, RoutedEventArgs e)
        {
            bool Warranty = GuaranteeYes.IsChecked == true ? true : false;

            var NewServiceOrder = new ServiceOrder(
                customer: TxtCustomer.Text,
                osNumber: int.Parse(TxtNumberOs.Text),
                arrivalDate: TxtArrivalDate.SelectedDate,
                observation: TxtObservation.Text,
                isGuarantee: Warranty
            );

            if (NewServiceOrder != null)
            {
                DatabaseActions database = new();

                if (database.BuildOS(NewServiceOrder))
                {
                    MessageBox.Show("OS criada com sucesso!");
                }
                else
                {
                    MessageBox.Show("Falha na criação da OS!");
                }
            }
            else
            {
                MessageBox.Show("Preencha os campos obrigatórios!");
            }
        }


        private void CleanUp(object sender, RoutedEventArgs e)
        {
            TxtCustomer.Text = String.Empty;
            TxtNumberOs.Text = String.Empty;
            TxtArrivalDate.SelectedDate = DateTime.Today;
            GuaranteeYes.IsChecked = false;
            GuaranteeNo.IsChecked = false;
        }
    }
}
