using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace NeuroApp
{
    public class ResponsiveBigButtons : INotifyPropertyChanged
    {
        private double windowHeight;
        private double windowWidth;

        public double WindowHeight
        {
            get => windowHeight;
            set
            {
                if (windowHeight != value)
                {
                    windowHeight = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ButtonSize));
                }
            }
        }

        public double WindowWidth
        {
            get => windowWidth;
            set
            {
                if (windowWidth != value)
                {
                    windowWidth = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ButtonSize));
                }
            }
        }

        public double ButtonSize => Math.Min(WindowHeight, WindowWidth) * 0.35;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
