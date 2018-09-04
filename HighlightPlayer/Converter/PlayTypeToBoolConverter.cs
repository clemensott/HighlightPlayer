using HighlightPlayer.Controls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class PlayTypeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (PlayTypeState)value == PlayTypeState.Highlights;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? PlayTypeState.Highlights : PlayTypeState.Medias;
        }
    }
}
