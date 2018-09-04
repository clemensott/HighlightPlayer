using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    class IconRenderer
    {
        public Action<StateButton, DrawingContext, Brush> Action { get; private set; }

        public Enum Value { get; private set; }

        public IconRenderer(Action<StateButton, DrawingContext, Brush> action, Enum value)
        {
            Action = action;
            Value = value;
        }
    }
}
