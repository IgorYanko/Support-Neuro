using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using NeuroApp.Classes;

namespace NeuroApp.ViewModels
{
    public class SupportStaticGuideViewModel : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<InstructionNode> Instructions { get; set; }

        private InstructionNode _selectedInstruction;
        public InstructionNode SelectedInstruction
        {
            get => _selectedInstruction;
            set
            {
                _selectedInstruction = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedInstructionSteps));
                OnPropertyChanged(nameof(ShowEmptyMessage));
            }
        }

        public ObservableCollection<string> SelectedInstructionSteps
        {
            get
            {
                if (SelectedInstruction?.Content == null) return new ObservableCollection<string>();
                return new ObservableCollection<string>(SelectedInstruction.Content.Split(" \n").Where(s => !string.IsNullOrWhiteSpace(s)));
            }
        }

        public bool ShowEmptyMessage => SelectedInstruction == null;

        public SupportStaticGuideViewModel(string title)
        {
            Title = title;
            InitializeInstructions(title);
        }

        private void InitializeInstructions(string title)
        {
            var generalInstructions = new[]
            {
                new InstructionNode("Assistência Técnica", 
                    "Ao receber o contato, solicitar o nome do cliente, aparelho e nº de série \n" +
                    "Verificar as instruções do aparelho específico e se possível guiar o cliente \n" +
                    "Caso o problema não possa ser resolvido, passar as instruções para envio \n" +
                    "Sugerir a coleta reversa para o cliente (frete por conta do cliente) \n" +
                    "Verificar se o aparelho está na garantia, na aba de garantias \n" +
                    "Quando criar a OS no Sensio, insira o defeito relatado"),

                new InstructionNode("Instalação de Software", 
                    "Criar instalação no Sensio como Assistência técnica \n" +
                    "Utilizar a tag de instalação para facilitar visualização \n" +
                    "A instalação de software é feita pela nossa equipe \n" +
                    "É necessário o software AnyDesk instalado \n" +
                    "Envie o ID do cliente para o técnico fazer o acesso \n" +
                    "É importante que o cliente acompanhe a instalação para:\n\n" +
                    "1. Entender o funcionamento básico do software\n" +
                    "2. Levantar possíveis dúvidas de uso\n" +
                    "3. Ajudar o técnico em caso de haver algum erro"),
            };

            var oldEquipments = new[]
            {
                new InstructionNode("Solução de Problemas", 
                    "Solicite o modelo do aparelho e o nº de série \n" +
                    "Verificar se o aparelho está na garantia, na aba de garantias \n" +
                    "Aparelhos muito antigos não receberão assistência \n" +
                    "Para esses aparelhos faça a sugestão de upgrade \n" +
                    "Em caso de dúvida contate o responsável para verificar \n" +
                    "Se o equipamento puder ser enviado, passar as instruções para envio \n" +
                    "Sugerir a coleta reversa para o cliente (frete por conta do cliente) \n" +
                    "Quando criar a OS no Sensio, insira o defeito relatado")
            };

            var deliveries = new[]
            {
                new InstructionNode("Coletas", 
                    "As coletas são realizadas nas Terças e Quintas \n" +
                    "A NF deve estar com o aparelho 1 dia antes da saída \n" +
                    "Se não estiverem prontas, as NFs devem ser solicitadas ao comercial/adm \n" +
                    "Antes da saída deve ser feito um checklist com a coleta \n" +
                    "Esse checklist é feito em uma ficha que estará preenchida previamente pelo técnico \n" +
                    "Após a conferência, embalagem e NF pronta, a coleta poderá ser realizada! \n")
            };

            Instructions = new ObservableCollection<InstructionNode>(title switch
            {
                "Instruções Gerais" => generalInstructions,
                "Aparelhos Antigos" => oldEquipments,
                "Coletas" => deliveries,
                _ => generalInstructions
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
