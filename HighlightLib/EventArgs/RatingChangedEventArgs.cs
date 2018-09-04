using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlightLib
{
    public class RatingChangedEventArgs : EventArgs
    {
        public double OldValue { get; private set; }

        public double NewValue { get; private set; }

        public RatingChangedEventArgs(double oldValue, double newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
