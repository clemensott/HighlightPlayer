using HighlightPlayer.Controls;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class MediaStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (MediaState)value == MediaState.Play ? PlayPauseState.Pause : PlayPauseState.Play;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (PlayPauseState)value == PlayPauseState.Pause ? MediaState.Play : MediaState.Pause;
        }
    }
}
