using System;
using System.Globalization;
using System.Windows.Data;

namespace TrafficViolationApp.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && parameter.ToString() == "IsNotification")
            {
                return (bool)value ? "Đã đọc" : "Chưa đọc";
            }
            return (bool)value ? "Đã nộp" : "Chưa nộp";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}