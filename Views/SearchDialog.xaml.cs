using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using NeuroApp.Classes;

namespace NeuroApp.Views
{

    public partial class SearchDialog : Window
    {
        private readonly DispatcherTimer _searchTimer;
        private DatabaseActions _actions;
        public List<Protocol> Protocols { get; set; } = new();

        public SearchDialog(IConfiguration configuration)
        {
            InitializeComponent();

            _actions = new DatabaseActions(configuration);

            _searchTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _searchTimer.Tick += DebouncerTimer_Tick;
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void DeleteProtocolButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;

                if (button?.DataContext is Protocol protocol)
                {
                    var result = MessageBox.Show("Deseja excluir o protocolo?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        string protocolCode = protocol.ProtocolCode;
                        await _actions.DeleteProtocolAsync(protocolCode);
                        MessageBox.Show("Protocolo excluído com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                        await ResetToFullListAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Protocolo inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao deletar o protocolo: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ExpandProtocolButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Protocol protocol)
            {
                protocol.IsExpanded = !protocol.IsExpanded;
            }
        }

        private async void DebouncerTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();

            try
            {
                var searchText = SearchBar.Text.Trim().ToLower();

                if (string.IsNullOrEmpty(searchText))
                {
                    await ResetToFullListAsync();
                    return;
                }

                if (searchText.Length < 2)
                {
                    return;
                }

                var stopwatch = Stopwatch.StartNew();
                Protocols = await _actions.GetProtocolBySearchAsync(searchText, _token.Token);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ProtocolsItemControl.ItemsSource = Protocols;
                });
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Busca cancelada!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("Erro ao realizar busca", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        private CancellationTokenSource _token;
        private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _token?.Cancel();
                _token = new CancellationTokenSource();

                _searchTimer.Stop();

                if (string.IsNullOrEmpty(SearchBar.Text))
                {
                    await ResetToFullListAsync();
                    return;
                }

                _searchTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ResetToFullListAsync()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    SearchBar.Text = null;
                    ProtocolsItemControl.ItemsSource = null;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
