using System.Windows;
using NeuroApp.Classes;
using Microsoft.Extensions.Configuration;

namespace NeuroApp.Views
{
    public partial class ObservationsDialog : Window
    {
        private readonly IConfiguration _configuration;
        private readonly string _osCode;
        public event Action OnClosedNotification;

        public bool IsConfirmed { get; private set; }

        public ObservationsDialog(IConfiguration configuration, string osCode, string? previousText)
        {
            InitializeComponent();
            _configuration = configuration;
            _osCode = osCode;

            if (previousText != null)
            {
                ObservationTextBox.Text = previousText;
            }

            this.Closed += (sender, e) => OnClosedNotification?.Invoke();
        }

        private async void AddObservationButton_Click(object sender, RoutedEventArgs e)
        {
            string observationText = ObservationTextBox.Text;
            DatabaseActions databaseActions = new(_configuration);

            if (!string.IsNullOrWhiteSpace(observationText))
            {
                var observationOperation = await databaseActions.AddObservationsAsync(observationText, _osCode);

                if (observationOperation)
                {
                    MessageBox.Show("Observação adicionada com sucesso!", "Observação adicionada", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsConfirmed = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Essa observação já foi adicionada!", "Observação repetida", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Não é possível adicionar uma observação vazia!", "Observação vazia!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void RemoveObservationButton_Click(object sender, RoutedEventArgs e)
        {
            DatabaseActions databaseActions = new(_configuration);
            var removeObservationOperation = await databaseActions.AddObservationsAsync(null, _osCode);

            if (removeObservationOperation)
            {
                MessageBox.Show("Observação removida com sucesso!", "Observação removida", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            else
            {
                MessageBox.Show("Falha ao remover observação!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
