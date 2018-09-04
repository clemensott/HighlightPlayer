using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlightPlayer.Controls
{
    public class ButtonStateChangedEventArgs : EventArgs
    {
        public Enum OldValue { get; private set; }

        public Enum NewValue { get; private set; }

        public ButtonStateChangedEventArgs(Enum oldValue, Enum newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
