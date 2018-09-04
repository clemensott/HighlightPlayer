using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum StopState { Stop }

    class StopButtonStates : BackgroundColorStateButtonSource
    {
        public StopButtonStates() : base(Brushes.WhiteSmoke, Brushes.DeepSkyBlue, Brushes.CornflowerBlue, GetIconRenderers())
        {
            //InnerMarginPercent = new Thickness(25);
        }

        private static IEnumerable<IconRenderer> GetIconRenderers()
        {
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderIcon), StopState.Stop);
        }

        private static void RenderIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Brush iconBrush = Brushes.Gray;
            Brush borderBrush = Brushes.Black;
            Pen borderPen = new Pen(borderBrush, 0.5);

            context.DrawGeometry(iconBrush, borderPen, new PathGeometry(GetSquare(sender)));
        }

        private static IEnumerable<PathFigure> GetSquare(StateButton sender)
        {
            Rect iconRect = sender.IconRect;

            Point p11 = iconRect.TopLeft;
            Point p12 = iconRect.BottomLeft;
            Point p21 = iconRect.TopRight;
            Point p22 = iconRect.BottomRight;

            return StateButton.GetFigureWithBoder(p11, p12, p22, p21);
        }
    }
}
