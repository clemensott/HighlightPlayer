using HighlightLib;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HighlightPlayer.Controls
{
    class HighlightsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (((HighlightCollection)value)?.Count ?? 0).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("HighlightsCountConverter.ConvertBack");
        }
    }
}
