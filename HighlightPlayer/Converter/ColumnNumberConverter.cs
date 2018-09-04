using HighlightPlayer.Controls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class ColumnNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SwitchViewState)value == SwitchViewState.Player ? 0 : 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? SwitchViewState.Player : SwitchViewState.Medias;
        }
    }
}
