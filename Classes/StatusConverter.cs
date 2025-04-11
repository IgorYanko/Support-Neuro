using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NeuroApp.Classes
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status)
            {
                return GetStatusToComboBox.GetStatusToComboBoxList<Status>()
                    .FirstOrDefault(s => GetStatusToComboBox.ConvertDisplayToEnum<Status>(s) == status);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return GetStatusToComboBox.ConvertDisplayToEnum<Status>(str) ?? Status.Unknown;
            }
            return Status.Unknown;
        }
    }
}
