using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Admas_HRM2.Converters
{
    public class LeaveStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() switch
            {
                "Approved" => new SolidColorBrush(Colors.Green),
                "Pending" => new SolidColorBrush(Colors.Orange),
                "Rejected" => new SolidColorBrush(Colors.Red),
                _ => new SolidColorBrush(Colors.Gray),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
