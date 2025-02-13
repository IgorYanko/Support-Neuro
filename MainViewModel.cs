using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NeuroApp
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(backingField, value)) return false;
            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }


    public class RelayCommand(Action execute, Func<bool> canExecute = null) : ICommand
    {
        private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Func<bool> _canExecute = canExecute;   

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class MainViewModel : BaseViewModel
    {
        public ICommand GoBackCommand { get; }

        private ResponsiveBigButtons _responsiveBigButtons;
        public ResponsiveBigButtons _ResponsiveBigButtons
        {
            get => _responsiveBigButtons;
            set => SetProperty(ref _responsiveBigButtons, value);
        }

        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        public ICommand ShowCockpitScreen_ { get; }
        public ICommand ShowCustomersScreen_ { get; }

        private static MainViewModel _instance;

        public static MainViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainViewModel();
                return _instance;
            }
        }

        public MainViewModel()
        {
            GoBackCommand = new RelayCommand(
                ShowHomeScreen,
                () => CurrentView != null
            );

            ShowCockpitScreen_ = new RelayCommand(() => CurrentView = new Cockpit(MainViewModel.Instance));
            ShowCustomersScreen_ = new RelayCommand(() => CurrentView = new Screens.Customers());

            ShowHomeScreen();
        }

        public void ShowHomeScreen()
        {
            Console.WriteLine("🏠 Indo para a HomeScreen!");
            CurrentView = new HomeScreen(this);
        }

        public ResponsiveBigButtons ResponsiveBigButtons { get; set; }
    }
}
