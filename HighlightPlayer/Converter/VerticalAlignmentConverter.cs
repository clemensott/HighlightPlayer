using HighlightPlayer.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class VerticalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SwitchViewState)value == SwitchViewState.Player ? VerticalAlignment.Stretch : VerticalAlignment.Bottom;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (VerticalAlignment)value == VerticalAlignment.Stretch ? SwitchViewState.Player : SwitchViewState.Medias;
        }
    }
}
