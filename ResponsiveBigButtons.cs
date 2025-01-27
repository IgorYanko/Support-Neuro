using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;

namespace NeuroApp
{
    public class ResponsiveBigButtons : INotifyPropertyChanged
    {
        private double windowHeight;
        private double windowWidth;
        private const double MinButtonSize = 50;
        private const double MaxButtonSize = 300;

        public double WindowHeight
        {
            get => windowHeight;
            set
            {
                if (Math.Abs(windowHeight - value) > 0.1)
                {
                    windowHeight = value;
                    OnPropertyChanged();
                    UpdateButtonSize();
                }
            }
        }

        public double WindowWidth
        {
            get => windowWidth;
            set
            {
                if (Math.Abs(windowWidth - value) > 0.1)
                {
                    windowWidth = value;
                    OnPropertyChanged();
                    UpdateButtonSize();
                }
            }
        }

        private double buttonSize;
        public double ButtonSize
        {
            get => buttonSize;
            private set
            {
                if (Math.Abs(buttonSize - value) > 0.1)
                {
                    buttonSize = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateButtonSize()
        {
            ButtonSize = Math.Max(MinButtonSize, Math.Min(MaxButtonSize, Math.Min(WindowHeight, WindowWidth) * 0.35));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
