using HighlightLib;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class HighlightToEndConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as Highlight)?.End ?? new TimeSpan();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
