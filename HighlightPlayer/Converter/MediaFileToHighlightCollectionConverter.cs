using HighlightLib;
using System;
using System.Globalization;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class MediaFileToHighlightCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as MediaFile)?.Highlights;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
