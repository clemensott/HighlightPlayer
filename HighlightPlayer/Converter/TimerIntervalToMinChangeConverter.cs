using System;
using System.Globalization;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class TimerIntervalToMinChangeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TimeSpan)value - TimeSpan.FromMilliseconds(50);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (TimeSpan)value + TimeSpan.FromMilliseconds(50);
        }
    }
}
