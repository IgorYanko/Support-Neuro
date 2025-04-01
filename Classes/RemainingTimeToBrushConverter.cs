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

        private static class Colors
        {
            public static readonly SolidColorBrush DefaultForeground = new(Color.FromRgb(0, 0, 0));
            public static readonly SolidColorBrush DefaultBackground = new(Color.FromRgb(255, 255, 255));
            public static readonly SolidColorBrush PausedForeground = new(Color.FromRgb(115, 115, 115));
            public static readonly SolidColorBrush PausedBackground = new(Color.FromRgb(225, 225, 225));
            public static readonly SolidColorBrush DangerForeground = new(Color.FromRgb(255, 255, 255));
            public static readonly SolidColorBrush DangerBackground = new(Color.FromRgb(236, 83, 83));
            public static readonly SolidColorBrush LateForeground = new(Color.FromRgb(236, 83, 83));
            public static readonly SolidColorBrush SafeForeground = new(Color.FromRgb(22, 145, 65));
            public static readonly SolidColorBrush SafeBackground = new(Color.FromArgb(55, 22, 145, 65));
            public static readonly SolidColorBrush WarningBackground = new(Color.FromArgb(115, 255, 215, 0));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not Sales sale)
            {
                return Brushes.Transparent;
            }

            string status = sale.DisplayStatus?.ToString() ?? string.Empty;
            bool isForeground = parameter?.ToString() == "Foreground";

            if (sale.IsPaused)
            {
                return parameter?.ToString() == "Foreground" ? Colors.PausedForeground : Colors.PausedBackground;
            }

            int remainingDays = CalculateRemainingDays(sale, status);

            if (status == "Aprovado" || SalesUtils.IsLocalStatus(status))
            {
                return GetBrushForApprovedStatus(remainingDays, isForeground);
            }

            return status switch
            {
                "Em Orçamento" or "Em Aberto" => GetBrushForLateSales(remainingDays, isForeground),
                "Faturado" => GetBrushForSaleStatus(remainingDays, isForeground),
                _ => isForeground ? Colors.DefaultForeground : null
            };
        }

        private static int CalculateRemainingDays(Sales sale, string status)
        {
            return status switch
            {
                "Aprovado" => SalesUtils.CalculateRemainingApprovedDays(sale.ApprovedAt) ?? NOT_APPROVED_FLAG,
                "Em Orçamento" => SalesUtils.CalculateRemainingQuotationDays(sale.QuotationDate) ?? NOT_APPROVED_FLAG,
                "Em Aberto" => SalesUtils.CalculateRemainingDaysToCheckout(sale.DateCreated) ?? NOT_APPROVED_FLAG,
                "Faturado" => (DateTime.Now - sale.DateCreated).Days,
                _ => NOT_APPROVED_FLAG
            };
        }

        private static object GetBrushForApprovedStatus(int remainingDays, bool isForeground)
        {
            if (remainingDays >= 4)
            {
                return isForeground ? Colors.DefaultForeground : Colors.SafeBackground;
            }
            else if (remainingDays > 2)
            {
                return isForeground ? Colors.DefaultForeground : Colors.WarningBackground;
            }
            else
            {
                return isForeground ? Colors.DangerForeground : Colors.DangerBackground;
            }
        }

        private static object GetBrushForSaleStatus(int remainingDays, bool isForeground)
        {
            if (remainingDays <= 7)
            {
                return isForeground ? Colors.SafeForeground : null;
            }
            else
            {
                return isForeground ? Colors.DangerForeground : Colors.DangerBackground;
            }
        }

        private static object GetBrushForLateSales(int remainingDays, bool isForeground)
        {
            return remainingDays < 2
                ? isForeground ? Colors.LateForeground : null
                : isForeground ? Colors.SafeForeground : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("This converter does not support ConvertBack.");
        }
    }
}
