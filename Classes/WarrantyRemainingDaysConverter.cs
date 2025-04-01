using System;
using System.Globalization;
using System.Windows.Data;

namespace NeuroApp.Classes
{
    public class WarrantyRemainingDaysConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Warranty warranty)
            {
                var remainingDays = (warranty.WarrantyEndDate - DateTime.Now).Days;
                return remainingDays.ToString();
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 