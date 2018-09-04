using HighlightPlayer.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class CurrentHighlightsWidthConverter : IValueConverter
    {
        private const double defaultWidth = 250;

        private double width = defaultWidth;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new GridLength((ShowHighlightsState)value == ShowHighlightsState.Show ? width : 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            width = ((GridLength)value).Value;

            if (width > 0) return ShowHighlightsState.Show;

            width = defaultWidth;

            return ShowHighlightsState.Hide;
        }
    }
}
