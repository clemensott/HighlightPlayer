using HighlightLib;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace HighlightPlayer.Converter
{
    class MediaFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Path.GetFileNameWithoutExtension((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("MediaFileNameConverter.ConvertBack");
        }
    }
}
