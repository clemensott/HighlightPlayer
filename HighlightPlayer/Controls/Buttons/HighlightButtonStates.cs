using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum HighlightState { Begin, End }

    class HighlightButtonStates : BackgroundColorStateButtonSource
    {
        private const double iconStrokeThicknessFactor = 0.15;

        public HighlightButtonStates() : base(Brushes.WhiteSmoke, Brushes.DeepSkyBlue, Brushes.CornflowerBlue, GetIconRenderers())
        {
            //InnerMarginPercent = new Thickness(20);
            //IconRatio = 0.5;
        }

        private static IEnumerable<IconRenderer> GetIconRenderers()
        {
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderBeginIcon), HighlightState.Begin);
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderEndIcon), HighlightState.End);
        }

        private static void RenderBeginIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Rect iconRect = sender.IconRect;
            double thickness = iconRect.Height * iconStrokeThicknessFactor;

            Point pTopLeft = iconRect.TopLeft;
            Point pBottomLeft = iconRect.BottomLeft;
            Point pTopInner = new Point(iconRect.X + thickness, iconRect.Y + thickness);
            Point pBottomInner = new Point(iconRect.X + thickness, iconRect.Y + iconRect.Height - thickness);
            Point pTopRightTop = iconRect.TopRight;
            Point pTopRightBottom = new Point(iconRect.X + iconRect.Width, iconRect.Y + thickness);
            Point pBottomRightTop = new Point(iconRect.X + iconRect.Width, iconRect.Y + iconRect.Height - thickness);
            Point pBottomRightBottom = iconRect.BottomRight;

            IEnumerable<PathFigure> figures = StateButton.GetFigureWithBoder(pTopLeft, pTopRightTop, pTopRightBottom,
                pTopInner, pBottomInner, pBottomRightTop, pBottomRightBottom, pBottomLeft);

            Brush iconBrush = Brushes.Gray;
            Pen borderPen = new Pen(Brushes.Black, 0.5);

            context.DrawGeometry(iconBrush, borderPen, new PathGeometry(figures));
        }

        private static void RenderEndIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Rect iconRect = sender.IconRect;
            double thickness = iconRect.Height * iconStrokeThicknessFactor;

            Point pTopRight = iconRect.TopRight;
            Point pBottomRight = iconRect.BottomRight;
            Point pTopInner = new Point(iconRect.X + iconRect.Width - thickness, iconRect.Y + thickness);
            Point pBottomInner = new Point(iconRect.X + iconRect.Width - thickness, iconRect.Y + iconRect.Height - thickness);
            Point pTopLeftTop = iconRect.TopLeft;
            Point pTopLeftBottom = new Point(iconRect.X, iconRect.Y + thickness);
            Point pBottomLeftTop = new Point(iconRect.X, iconRect.Y + iconRect.Height - thickness);
            Point pBottomLeftBottom = iconRect.BottomLeft;

            IEnumerable<PathFigure> figures = StateButton.GetFigureWithBoder(pTopLeftTop, pTopRight, pBottomRight,
                pBottomLeftBottom, pBottomLeftTop, pBottomInner, pTopInner, pTopLeftBottom);

            Brush iconBrush = Brushes.Gray;
            Pen borderPen = new Pen(Brushes.Black, 0.5);

            context.DrawGeometry(iconBrush, borderPen, new PathGeometry(figures));
        }
    }
}
