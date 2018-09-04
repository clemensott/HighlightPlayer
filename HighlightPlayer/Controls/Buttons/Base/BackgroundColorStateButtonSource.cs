using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    class BackgroundColorStateButtonSource : StateButtonSource
    {
        public BackgroundColorStateButtonSource(Brush backgroundBrushOut, Brush backgroundBrushOn,
            Brush backgroundBrushClick, IEnumerable<IconRenderer> iconRenderers)
            : base(ConvertToStates(backgroundBrushOut, backgroundBrushOn, backgroundBrushClick, iconRenderers))
        {
        }

        private static IEnumerable<ButtonState> ConvertToStates(Brush bbOut, Brush bbOn,
            Brush bbClick, IEnumerable<IconRenderer> iconRenderers)
        {
            return iconRenderers.Select(ir => new BackgroundColorButtonState(ir.Value, ir.Action, bbOut, bbOn, bbClick));
        }
    }
}
