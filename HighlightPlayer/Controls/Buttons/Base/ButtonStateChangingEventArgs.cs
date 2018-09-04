using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlightPlayer.Controls
{
    public class ButtonStateChangingEventArgs : EventArgs
    {
        public Enum OldValue { get; private set; }

        public Enum NewValue { get; set; }

        public ButtonStateChangingEventArgs(Enum oldValue, Enum newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
