using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void WarrantyTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WarrantyTypeComboBox.SelectedIndex == 0)
            {
                WarrantyMonthsTextBox.Text = "3";
                WarrantyMonthsTextBox.IsReadOnly = true;
            }
            else if (WarrantyTypeComboBox.SelectedIndex == 1)
            {
                WarrantyMonthsTextBox.Text = "12";
                WarrantyMonthsTextBox.IsReadOnly = true;
            }
            else
            {
                WarrantyMonthsTextBox.Text = string.Empty;
                WarrantyMonthsTextBox.IsReadOnly = false;
            }
        }

        private void IsNumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var errorMessage = GetValidationErrorMessage();
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    HighlightEmptyFields();
                    MessageBox.Show(errorMessage, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _warranty ??= new Warranty();
                UpdateWarrantyFromUI(_warranty);

                bool isNew = _warranty.Id == 0;
                await (isNew ? _actions.SaveWarrantyAsync(_warranty)
                             : _actions.UpdateWarrantyAsync(_warranty));

                MessageBox.Show("Garantia Salva com sucesso!", "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar a garantia: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetValidationErrorMessage()
        {
            int? warrantyMonths = null;
            if (int.TryParse(WarrantyMonthsTextBox.Text, out int parsedWarrantyMonths))
            {
                warrantyMonths = parsedWarrantyMonths;
            }

            if (warrantyMonths != null && (warrantyMonths < 1 || warrantyMonths > 12))
            {
                return "A garantia deve variar entre 1 e 12 meses";
            }

            var invalidFields= new List<string>();

            var requiredFields = new List<(string FieldName, Func<bool> IsValid)>
            {
                ("Nome do cliente", () => !string.IsNullOrEmpty(ClientNameTextBox.Text)),
                ("Número de série", () => !string.IsNullOrEmpty(SerialNumberTextBox.Text)),
                ("Dispositivo", () => !string.IsNullOrEmpty(DeviceTextBox.Text)),
                ("Observações", () => !string.IsNullOrEmpty(ObservationsTextBox.Text)),
                ("Data do serviço", () => ServiceDatePicker.SelectedDate != null),
                ("Meses de garantia", () => !string.IsNullOrEmpty(WarrantyMonthsTextBox.Text))
            };

            foreach (var (fieldName, isValid) in requiredFields)
            {
                if (!isValid())
                {
                    invalidFields.Add($"{fieldName}");
                }
            }

            if (invalidFields.Any())
            {
                return $"Preencha os seguintes campos obrigatórios:\n\n- {string.Join("\n- ", invalidFields)}";
            }

            return null;
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

        private void SetBorderstyle(FrameworkElement element, bool isInvalid)
        {
            if (element is Control control)
            {
                control.BorderBrush = isInvalid ? Brushes.Red : Brushes.Black;
                control.BorderThickness = isInvalid ? new Thickness(1.5) : new Thickness(1);
            }
        }

        private void HighlightEmptyFields()
        {
            SetBorderstyle(ClientNameTextBox, string.IsNullOrEmpty(ClientNameTextBox.Text));
            SetBorderstyle(SerialNumberTextBox, string.IsNullOrEmpty(SerialNumberTextBox.Text));
            SetBorderstyle(DeviceTextBox, string.IsNullOrEmpty(DeviceTextBox.Text));
            SetBorderstyle(WarrantyTypeComboBox, string.IsNullOrEmpty(WarrantyTypeComboBox.Text));
            SetBorderstyle(WarrantyMonthsTextBox, string.IsNullOrEmpty(WarrantyMonthsTextBox.Text));
            SetBorderstyle(ObservationsTextBox, string.IsNullOrEmpty(ObservationsTextBox.Text));
            SetBorderstyle(ServiceDatePicker, ServiceDatePicker.SelectedDate == null);
        }
    }
} 