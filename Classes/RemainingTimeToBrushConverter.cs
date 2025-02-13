using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace NeuroApp.Classes
{
    public class RemainingTimeToBrushConverter : IValueConverter
    {
        private const int NOT_APPROVED_FLAG = -999;

        private static readonly SolidColorBrush DefaultForeground = new(Color.FromRgb(0, 0, 0));
        private static readonly SolidColorBrush DefaultBackground = new(Color.FromRgb(255, 255, 255));

        private static readonly SolidColorBrush PausedForeground = new(Color.FromRgb(115, 115, 115));
        private static readonly SolidColorBrush PausedBackground = new(Color.FromRgb(225, 225, 225));

        private static readonly SolidColorBrush SafeBackground = new(Color.FromRgb(176, 215, 176));
        private static readonly SolidColorBrush WarningBackground = new(Color.FromRgb(236, 236, 83));
        private static readonly SolidColorBrush DangerForeground = new(Color.FromRgb(255, 255, 255));
        private static readonly SolidColorBrush DangerBackground = new(Color.FromRgb(236, 83, 83));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Sales sale)
            {
                return Brushes.Transparent;
            }

            if (sale.IsPaused)
            {
                return parameter?.ToString() == "Foreground" ? PausedForeground : PausedBackground;
            }

            int remainingDays = sale.RemainingBusinessDays ?? int.MaxValue;
            string status = sale.Status.ToString();
            string normalizedStatus = status.Replace(" ", "");

            if (status == "Aprovado" || Sales.IsLocalStatus(normalizedStatus))
            {
                Console.WriteLine($"{status} e {normalizedStatus}");
                if (remainingDays == NOT_APPROVED_FLAG)
                {
                    return parameter?.ToString() == "Foreground" ? DefaultForeground : DefaultBackground;
                }

                if (remainingDays >= 4)
                {
                    return parameter?.ToString() == "Foreground" ? DefaultForeground : SafeBackground;
                }
                else if (remainingDays > 1)
                {
                    return parameter?.ToString() == "Foreground" ? DefaultForeground : WarningBackground;
                }
                else
                {
                    return parameter?.ToString() == "Foreground" ? DangerForeground : DangerBackground;
                }
            }
            else
            {
                Console.WriteLine($"{status} e {normalizedStatus}");
                return parameter?.ToString() == "Foreground" ? DefaultForeground : DefaultBackground;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This converter does not support ConvertBack.");
        }
    }
}
