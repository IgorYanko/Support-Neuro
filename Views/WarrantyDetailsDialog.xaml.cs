using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;
using NeuroApp.Classes;

namespace NeuroApp
{
    public partial class WarrantyDetailsDialog : Window
    {
        private Warranty _warranty;
        private DatabaseActions _actions;

        public event Action OnClosedNotification;

        public WarrantyDetailsDialog(Warranty warranty, IConfiguration configuration)
        {
            InitializeComponent();
            _actions = new(configuration);
  
            this.Closed += (sender, e) => OnClosedNotification?.Invoke();
        }

        private void WarrantyTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (WarrantyTypeComboBox.SelectedIndex == 0)
            {
                WarrantyMonthsTextBox.Text = "3";
            }
            else if (WarrantyTypeComboBox.SelectedIndex == 1)
            {
                WarrantyMonthsTextBox.Text = "12";
            }
            else
            {
                
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Overlay_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!AreRequiredFieldsValid())
                {
                    HighlightEmptyFields();
                    MessageBox.Show("Preencha todos os campos obrigatórios!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _warranty ??= new Warranty();
                UpdateWarrantyFromUI(_warranty);

                bool isNew = _warranty.Id == 0;
                await (isNew ? _actions.SaveWarrantyAsync(_warranty)
                             : _actions.UpdateWarrantyAsync(_warranty));

                MessageBox.Show("Garantia Salva com sucesso!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar a garantia: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool AreRequiredFieldsValid()
        {
            return !string.IsNullOrEmpty(ClientNameTextBox.Text) &&
                   !string.IsNullOrEmpty(SerialNumberTextBox.Text) &&
                   !string.IsNullOrEmpty(DeviceTextBox.Text) &&
                   !string.IsNullOrEmpty(ObservationsTextBox.Text); /*&&
                   DateTime.TryParse(ServiceDatePicker.SelectedDate, out _);*/
        }

        private void UpdateWarrantyFromUI(Warranty warranty)
        {
            warranty.Customer = ClientNameTextBox.Text;
            warranty.OsCode = OsCodeTextBox.Text;
            warranty.SerialNumber = SerialNumberTextBox.Text;
            warranty.Device = DeviceTextBox.Text;
            warranty.ServiceDate = ServiceDatePicker.SelectedDate ?? DateTime.Now;
            warranty.Observation = ObservationsTextBox.Text;
            warranty.WarrantyEndDate = warranty.ServiceDate.AddMonths(warranty.WarrantyMonths);
        }

        private void HighlightEmptyFields()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                ClientNameTextBox.BorderBrush = string.IsNullOrEmpty(ClientNameTextBox.Text)
                    ? Brushes.Blue : Brushes.Red;

                ClientNameTextBox.BorderBrush = string.IsNullOrEmpty(SerialNumberTextBox.Text)
                    ? Brushes.Blue : Brushes.Red;

                ClientNameTextBox.BorderBrush = string.IsNullOrEmpty(DeviceTextBox.Text)
                    ? Brushes.Blue : Brushes.Red;

                ClientNameTextBox.BorderBrush = string.IsNullOrEmpty(ObservationsTextBox.Text)
                    ? Brushes.Blue : Brushes.Red;
            });
        }
    }
} 