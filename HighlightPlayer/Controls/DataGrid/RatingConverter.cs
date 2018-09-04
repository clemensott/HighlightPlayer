using System;
using System.Globalization;
using System.Windows.Data;

namespace HighlightPlayer.Controls
{
    class RatingConverter : IValueConverter
    {
        private double value;
        private string text;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (this.value == (double)value) return text;

            this.value = (double)value;
            return text = value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double newValue;

            if (double.TryParse(value.ToString(), out newValue)) return this.value = newValue;

            return this.value;
        }
    }
}
