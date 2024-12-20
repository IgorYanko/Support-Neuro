using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;

namespace NeuroApp.Classes
{
    public class TaskUpdater<T> : INotifyPropertyChanged
    {
        private readonly Func<Task<ObservableCollection<T>>> _updateFunction;
        private readonly Timer _timer;
        private ObservableCollection<T> _items;

        public bool IsUpdating { get; private set; }

        public ObservableCollection<T> Items
        {
            get => _items;
            private set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public event EventHandler<Exception> OnError;
        public event PropertyChangedEventHandler PropertyChanged;

        public TaskUpdater(Func<Task<ObservableCollection<T>>> updateFunction, TimeSpan interval)
        {
            _updateFunction = updateFunction ?? throw new ArgumentNullException(nameof(updateFunction));
            _timer = new Timer(OnTimerElapsed, null, Timeout.Infinite, Timeout.Infinite);
            Items = new ObservableCollection<T>();
            IsUpdating = false;

            Start(interval);
        }

        public void Start(TimeSpan interval)
        {
            _timer.Change(TimeSpan.Zero, interval);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private async void OnTimerElapsed(object state)
        {
            if (IsUpdating) return;

            try
            {
                IsUpdating = true;
                var updatedItems = await _updateFunction.Invoke();
                UpdateItems(updatedItems);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, ex);
            }
            finally
            {
                IsUpdating = false;
            }
        }

        private void UpdateItems(ObservableCollection<T> updatedItems)
        {
            if (updatedItems == null) return;

            var newItems = new List<T>(updatedItems);

            Application.Current.Dispatcher.Invoke(() =>
            {
                Items.Clear();
                foreach (var item in newItems)
                {
                    Items.Add(item);
                }
            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
