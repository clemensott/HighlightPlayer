using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlightLib
{
    public class TimeChangedEventArgs : EventArgs
    {
        public TimeSpan OldValue { get; private set; }

        public TimeSpan NewValue { get; private set; }

        public TimeChangedEventArgs(TimeSpan oldValue, TimeSpan newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
