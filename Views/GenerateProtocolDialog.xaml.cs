using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Extensions.Configuration;
using NeuroApp.Classes;

namespace NeuroApp.Views
{
    public partial class GenerateProtocolDialog : Window
    {
        private Protocol _protocol;
        private DatabaseActions _actions;

        public GenerateProtocolDialog(IConfiguration configuration)
        {
            InitializeComponent();
            _actions = new(configuration);
        }

        private void GenerateProtocolCode(object sender, EventArgs e)
        {
            var protocol = $"{new Random().Next(1000, 9999)}/{DateTime.Now:dd-MM-yyyy-HH:mm:ss}";
            CodeTextBox.Text = protocol;
            GenerateProtocolCodeButton.IsEnabled = false;
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

        private async void SaveProtocolButton_Click(object sender, RoutedEventArgs e)
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

                _protocol ??= new Protocol();
                UpdateProtocolFromUI(_protocol);

                await _actions.SaveProtocolAsync(_protocol);

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
            var invalidFields = new List<string>();

            var requiredFields = new List<(string FieldName, Func<bool> IsValid)>
            {
                ("Nome do cliente", () => !string.IsNullOrEmpty(CustomerTextBox.Text)),
                ("Número de série", () => !string.IsNullOrEmpty(SerialNumberTextBox.Text)),
                ("Título", () => !string.IsNullOrEmpty(TitleTextBox.Text)),
                ("Dispositivo", () => !string.IsNullOrEmpty(DeviceTextBox.Text)),
                ("Descrição", () => !string.IsNullOrEmpty(DescriptionTextBox.Text)),
                ("Nº de protocolo", () => !string.IsNullOrEmpty(CodeTextBox.Text))
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

        private void UpdateProtocolFromUI(Protocol protocol)
        {
            protocol.Customer = CustomerTextBox.Text;
            protocol.SerialNumber = SerialNumberTextBox.Text;
            protocol.Title = TitleTextBox.Text;
            protocol.Description = DescriptionTextBox.Text;
            protocol.Device = DeviceTextBox.Text;
            protocol.ProtocolCode = CodeTextBox.Text;
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
            SetBorderstyle(CustomerTextBox, string.IsNullOrEmpty(CustomerTextBox.Text));
            SetBorderstyle(SerialNumberTextBox, string.IsNullOrEmpty(SerialNumberTextBox.Text));
            SetBorderstyle(DeviceTextBox, string.IsNullOrEmpty(DeviceTextBox.Text));
            SetBorderstyle(TitleTextBox, string.IsNullOrEmpty(TitleTextBox.Text));
            SetBorderstyle(DescriptionTextBox, string.IsNullOrEmpty(DescriptionTextBox.Text));
            SetBorderstyle(CodeTextBox, string.IsNullOrEmpty(CodeTextBox.Text));
        }
    }
}
