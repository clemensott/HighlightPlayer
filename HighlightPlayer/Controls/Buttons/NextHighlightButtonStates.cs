using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum NextHighlightState { Next }

    class NextHighlightButtonStates : BackgroundColorStateButtonSource
    {
        private const double iconStrokeWidthFactor = 0.40;

        public NextHighlightButtonStates() : base(Brushes.WhiteSmoke, Brushes.DeepSkyBlue, Brushes.CornflowerBlue, GetIconRenderers())
        {
            //parent.InnerMarginPercent = new Thickness(30);
            //parent.IconRatio = 1;
        }

        private static  IEnumerable<IconRenderer> GetIconRenderers()
        {
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderIcon),NextHighlightState.Next);
        }

        private static  void RenderIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Brush iconBrush = Brushes.Gray;
            Brush borderBrush = Brushes.Black;
            Pen borderPen = new Pen(borderBrush, 0.5);

            IEnumerable<PathFigure> figures = GetStrokeFigures(sender);
            figures = figures.Concat(GetArrows(sender));

            context.DrawGeometry(iconBrush, borderPen, new PathGeometry(figures));
        }

        private static  IEnumerable<PathFigure> GetStrokeFigures(StateButton sender)
        {
            Rect iconRect = sender.IconRect;
            double strokeWidth = iconRect.Width * iconStrokeWidthFactor;

            Point p11 = new Point(iconRect.X + iconRect.Width - strokeWidth, iconRect.Y);
            Point p12 = new Point(iconRect.X + iconRect.Width - strokeWidth, iconRect.Y + iconRect.Height);
            Point p21 = iconRect.TopRight;
            Point p22 = iconRect.BottomRight;

            return StateButton.GetFigureWithBoder(p11, p12, p22, p21);
        }

        private static  IEnumerable<PathFigure> GetArrows(StateButton sender)
        {
            Rect iconRect = sender.IconRect;
            double strokeWidth = iconRect.Width * iconStrokeWidthFactor;

            Size size = new Size(iconRect.Width - strokeWidth, iconRect.Height);
            Point offset = iconRect.TopLeft;

            return GetArrow(offset, size);
        }

        private static  IEnumerable<PathFigure> GetArrow(Point offset, Size size)
        {
            Point p1 = new Point(offset.X, offset.Y);
            Point p2 = new Point(offset.X, offset.Y + size.Height);
            Point p3 = new Point(offset.X + size.Width, offset.Y + size.Height / 2);

            return StateButton.GetFigureWithBoder(p1, p2, p3);
        }
    }
}
