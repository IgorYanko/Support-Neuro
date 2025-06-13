using System.Windows;
using NeuroApp.Classes;
using Microsoft.Extensions.Configuration;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using System.Windows.Media;

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

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            ObservationTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            ResetTextBoxStyle();
            DragMove();
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

        private void ObservationTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (ObservationTextBox.Effect as DropShadowEffect).Color = Colors.LightBlue;
            (ObservationTextBox.Effect as DropShadowEffect).ShadowDepth = 0;
            (ObservationTextBox.Effect as DropShadowEffect).BlurRadius = 15;
            (ObservationTextBox.Effect as DropShadowEffect).Opacity = 0.9;
        }

        private void ResetTextBoxStyle()
        {
            (ObservationTextBox.Effect as DropShadowEffect).Color = Colors.Gray;
            (ObservationTextBox.Effect as DropShadowEffect).ShadowDepth = 0;
            (ObservationTextBox.Effect as DropShadowEffect).BlurRadius = 15;
            (ObservationTextBox.Effect as DropShadowEffect).Opacity = 0.5;
        }
    }
}
