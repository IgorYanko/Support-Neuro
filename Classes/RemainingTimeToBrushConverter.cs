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

        private static readonly SolidColorBrush SafeApprovedBackground = new(Color.FromArgb(255, 168, 230, 168));
        private static readonly SolidColorBrush WarningApprovedBackground = new(Color.FromArgb(255, 242, 227, 148));
        private static readonly SolidColorBrush DangerForeground = new(Color.FromRgb(255, 255, 255));
        private static readonly SolidColorBrush DangerBackground = new(Color.FromArgb(230, 236, 83, 83));

        private static readonly SolidColorBrush SafeCheckoutBackground = new(Color.FromArgb(190, 194, 221, 194));
        private static readonly SolidColorBrush WarningCheckoutBackground = new(Color.FromArgb(200, 235, 224, 164));

        private static readonly SolidColorBrush SafeQuotationBackground = new(Color.FromArgb(200, 180, 225, 180));
        private static readonly SolidColorBrush WarningQuotationBackground = new(Color.FromArgb(190, 245, 225, 166));

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

            int remainingDays;
            string status = sale.DisplayStatus?.ToString() ?? string.Empty;

            if (status == "Aprovado" || Sales.IsLocalStatus(status))
            {
                remainingDays = sale.RemainingBusinessDays ?? NOT_APPROVED_FLAG;

                if (remainingDays >= 4)
                {
                    return parameter?.ToString() == "Foreground" ? DefaultForeground : SafeApprovedBackground;
                }
                else if (remainingDays > 1)
                {
                    return parameter?.ToString() == "Foreground" ? DefaultForeground : WarningApprovedBackground;
                }
                else
                {
                    return parameter?.ToString() == "Foreground" ? DangerForeground : DangerBackground;
                }
            }
            //else if (status == "Em Orçamento")
            //{
            //    remainingDays = sale.RemainingQuotationDays ?? NOT_APPROVED_FLAG;

            //    if (remainingDays >= 4)
            //    {
            //        return parameter?.ToString() == "Foreground" ? DefaultForeground : SafeQuotationBackground;
            //    }
            //    else if (remainingDays > 1)
            //    {
            //        return parameter?.ToString() == "Foreground" ? DefaultForeground : WarningQuotationBackground;
            //    }
            //    else
            //    {
            //        return parameter?.ToString() == "Foreground" ? DangerForeground : DangerBackground;
            //    }
            //}
            //else if (status == "Em Aberto")
            //{
            //    remainingDays = sale.RemainingDaysToCheckout ?? NOT_APPROVED_FLAG;
                
            //    if (remainingDays >= 15)
            //    {
            //        return parameter?.ToString() == "Foreground" ? DefaultForeground : SafeCheckoutBackground;
            //    }
            //    else if (remainingDays >= 10)
            //    {
            //        return parameter?.ToString() == "Foreground" ? DefaultForeground: WarningCheckoutBackground;
            //    }
            //    else
            //    {
            //        return parameter?.ToString() == "Foreground" ? DangerForeground: DangerBackground;
            //    }
            //}
            else if (sale.DisplayType == "Venda")
            {
                TimeSpan diff = DateTime.Now - sale.DateCreated;
                remainingDays = diff.Days;

                if (remainingDays <= 2)
                {
                    return parameter?.ToString() == "Foreground" ? DefaultForeground : SafeApprovedBackground;
                }
                else if (remainingDays <= 5)
                {
                    return parameter?.ToString() == "Foreground" ? DefaultForeground : WarningApprovedBackground;
                }
                else
                {
                    return parameter?.ToString() == "Foreground" ? DangerForeground : DangerBackground;
                }
            }
            else
            {
                return parameter?.ToString() == "Foreground" ? DefaultForeground : DefaultBackground;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This converter does not support ConvertBack.");
        }
    }
}
