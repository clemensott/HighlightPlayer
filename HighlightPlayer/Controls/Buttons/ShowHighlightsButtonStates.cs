using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum ShowHighlightsState { Show, Hide }

    class ShowHighlightsButtonStates : BackgroundColorStateButtonSource
    {
        private const double iconBorderThicknessFactor = 0.3;

        public ShowHighlightsButtonStates() : base(Brushes.WhiteSmoke, Brushes.DeepSkyBlue, Brushes.CornflowerBlue, GetIconRenderers())
        {
            //InnerMarginPercent = new Thickness(20);
            //IconRatio = 1.3;
        }

        private static IEnumerable<IconRenderer> GetIconRenderers()
        {
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderShowIcon), ShowHighlightsState.Show);
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderHideIcon), ShowHighlightsState.Hide);
        }

        private static void RenderShowIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            RenderIcon(sender, context, Brushes.MediumPurple);
        }

        private static void RenderHideIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            RenderIcon(sender, context, Brushes.Gray);
        }

        private static void RenderIcon(StateButton sender, DrawingContext context, Brush foregroundBrush)
        {
            Rect iconRect = sender.IconRect;
            double thickness = iconRect.Width / 5;

            Point pL11 = iconRect.TopLeft;
            Point pL21 = new Point(iconRect.X + thickness * 2, iconRect.Y);
            Point pL22 = new Point(iconRect.X + thickness * 2, iconRect.Y + thickness);
            Point pL12 = new Point(iconRect.X + thickness * 1, iconRect.Y + thickness);
            Point pL13 = new Point(iconRect.X + thickness * 1, iconRect.Bottom - thickness);
            Point pL23 = new Point(iconRect.X + thickness * 2, iconRect.Bottom - thickness);
            Point pL24 = new Point(iconRect.X + thickness * 2, iconRect.Bottom);
            Point pL14 = iconRect.BottomLeft;

            Point pR11 = iconRect.TopRight;
            Point pR21 = new Point(iconRect.X + thickness * 3, iconRect.Y);
            Point pR22 = new Point(iconRect.X + thickness * 3, iconRect.Y + thickness);
            Point pR12 = new Point(iconRect.X + thickness * 4, iconRect.Y + thickness);
            Point pR13 = new Point(iconRect.X + thickness * 4, iconRect.Bottom - thickness);
            Point pR23 = new Point(iconRect.X + thickness * 3, iconRect.Bottom - thickness);
            Point pR24 = new Point(iconRect.X + thickness * 3, iconRect.Bottom);
            Point pR14 = iconRect.BottomRight;

            IEnumerable<PathFigure> leftFigures = StateButton.GetFigureWithBoder(pL11, pL21, pL22, pL12, pL13, pL23, pL24, pL14);
            IEnumerable<PathFigure> rightFigures = StateButton.GetFigureWithBoder(pR11, pR21, pR22, pR12, pR13, pR23, pR24, pR14);

            Pen pen = new Pen(Brushes.Black, thickness * iconBorderThicknessFactor);

            context.DrawGeometry(foregroundBrush, pen, new PathGeometry(leftFigures));
            context.DrawGeometry(foregroundBrush, pen, new PathGeometry(rightFigures));
        }
    }
}
