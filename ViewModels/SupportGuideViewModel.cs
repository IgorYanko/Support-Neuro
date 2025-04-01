using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NeuroApp.Api;
using NeuroApp.Classes;

namespace NeuroApp.ViewModels
{
    public class SupportGuideViewModel : INotifyPropertyChanged
    {
        private readonly SupportFlowManager _flowManager;
        private string _currentEquipment;
        private SupportFlowNode _currentNode;
        private bool _showSolutions;
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

        public string CurrentQuestion => _currentNode?.Question ?? string.Empty;

        public bool ShowSolutions
        {
            get => _showSolutions;
            set
            {
                _showSolutions = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowOptions));
            }
        }

        public bool ShowOptions => !ShowSolutions && _currentNode != null && !_currentNode.IsFinal;

        public ObservableCollection<string> Solutions => _currentNode?.Solutions ?? new ObservableCollection<string>();

        public ICommand YesCommand { get; }
        public ICommand NoCommand { get; }
        public ICommand RestartCommand { get; }

        public SupportGuideViewModel(string equipment)
        {
            _flowManager = new SupportFlowManager();
            _currentEquipment = equipment;
            Title = $"Suporte - {equipment}";

            YesCommand = new RelayCommand(ExecuteYesCommand);
            NoCommand = new RelayCommand(ExecuteNoCommand);
            RestartCommand = new RelayCommand(ExecuteRestartCommand);

            _currentNode = _flowManager.GetStartNode(equipment);
            ShowSolutions = false;
        }

        private void ExecuteYesCommand()
        {
            NavigateToNext(true);
        }

        private void ExecuteNoCommand()
        {
            NavigateToNext(false);
        }

        private void ExecuteRestartCommand()
        {
            _currentNode = _flowManager.GetStartNode(_currentEquipment);
            ShowSolutions = false;
            NotifyStateChanged();
        }

        private void NavigateToNext(bool isYesResponse)
        {
            var nextNode = _flowManager.GetNextNode(_currentEquipment, _currentNode.Id, isYesResponse);
            if (nextNode != null)
            {
                _currentNode = nextNode;
                ShowSolutions = _currentNode.IsFinal;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged()
        {
            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(Solutions));
            OnPropertyChanged(nameof(ShowOptions));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 