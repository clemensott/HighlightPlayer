using System;
using System.Globalization;
using System.Windows.Data;

namespace HighlightPlayer.Controls
{
    class TimeSpanConverter : IValueConverter
    {
        private TimeSpan value;
        private string text;

        public TimeSpanConverter()
        {
            value = new TimeSpan();
            text = string.Empty;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.value == (TimeSpan)value) return text;

            this.value = (TimeSpan)value;

            return text = this.value.ToString("hh\\:mm\\:ss\\.ff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan newValue;
            text = value.ToString();

            if (TimeSpan.TryParse(text, out newValue)) return this.value = newValue;

            return this.value;
        }
    }
}
