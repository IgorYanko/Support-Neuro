using System;
using System.Windows;
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
            _warranty = warranty;
            _actions = new(configuration);

            ClientNameTextBox.Text = _warranty.ClientName;
            OsCodeTextBox.Text = _warranty.OsCode;
            SerialNumberTextBox.Text = _warranty.SerialNumber;
            DeviceTextBox.Text = _warranty.Device;
            ServiceDatePicker.SelectedDate = _warranty.ServiceDate;
            ObservationsTextBox.Text = _warranty.Observation;

            if (_warranty.WarrantyMonths == 3)
            {
                WarrantyTypeComboBox.SelectedIndex = 0;
            }
            else if (_warranty.WarrantyMonths == 12)
            {
                WarrantyTypeComboBox.SelectedIndex = 1;
            }

            WarrantyMonthsTextBox.Text = _warranty.WarrantyMonths.ToString();
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
                string clientName = ClientNameTextBox.Text;
                string osCode = OsCodeTextBox.Text;
                string serialNumber = SerialNumberTextBox.Text;
                string device = DeviceTextBox.Text;
                DateTime serviceDate = ServiceDatePicker.SelectedDate ?? DateTime.Now;
                int warrantyMonths = int.Parse(WarrantyMonthsTextBox.Text);
                string observation = ObservationsTextBox.Text;

                if (!string.IsNullOrEmpty(clientName) && !string.IsNullOrEmpty(serialNumber) && !string.IsNullOrEmpty(device))
                {
                    if (_warranty == null)
                        _warranty = new Warranty();

                    _warranty.ClientName = clientName;
                    _warranty.SerialNumber = serialNumber;
                    _warranty.Device = device;
                    _warranty.ServiceDate = serviceDate;
                    _warranty.WarrantyMonths = warrantyMonths;
                    _warranty.Observation = observation;

                    if (_warranty.Id == 0)
                    {
                        await _actions.SaveWarrantyAsync(_warranty);
                        MessageBox.Show("Garantia salva com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        await _actions.UpdateWarrantyAsync(_warranty);
                        MessageBox.Show("Garantia salva com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                }
                else
                {
                    MessageBox.Show("Preencha os campos obrigatórios!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar a garantia: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 