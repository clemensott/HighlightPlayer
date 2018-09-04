using HighlightPlayer.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class SwitchViewToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SwitchViewState)value == SwitchViewState.Medias ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible ? SwitchViewState.Medias : SwitchViewState.Player;
        }
    }
}
