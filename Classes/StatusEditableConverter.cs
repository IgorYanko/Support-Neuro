using System;
using System.Globalization;
using System.Windows.Data;

namespace NeuroApp.Classes
{
    public class StatusEditableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return SalesUtils.IsStatusEditable(value as string);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
