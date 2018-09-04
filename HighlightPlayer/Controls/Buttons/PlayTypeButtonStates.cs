using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum PlayTypeState { Medias, Highlights }

    class PlayTypeButtonStates : BackgroundColorStateButtonSource
    {
        private const double iconVerticalStrokeThicknessFactor = 0.1, iconHorizontalStrokeThicknessFactor = 0.075,
             highlightIconVerticalStrokeDistanceFactor = 0.7, highlightIconBeginCloseWidthFactor = 0.1;

        public PlayTypeButtonStates() : base(Brushes.WhiteSmoke, Brushes.DeepSkyBlue, Brushes.CornflowerBlue, GetIconRenderers())
        {
            //InnerMarginPercent = new Thickness(15);
            //IconRatio = 1.5;
        }

        private static IEnumerable<IconRenderer> GetIconRenderers()
        {
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderMediaIcon), PlayTypeState.Medias);
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderHighlightIcon), PlayTypeState.Highlights);
        }

        private static void RenderHighlightIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Rect horizontalStrokeRect = GetHorizontalStrokeRect(sender);
            IEnumerable<PathFigure> figures = GetHighlightIconFigures(sender);

            Brush iconBrush = Brushes.Black;
            Pen pen = new Pen(iconBrush, 0);

            context.DrawRectangle(iconBrush, pen, horizontalStrokeRect);
            context.DrawGeometry(iconBrush, pen, new PathGeometry(figures));
        }

        private static IEnumerable<PathFigure> GetHighlightIconFigures(StateButton sender)
        {
            yield return GetBeginBracketFigure(sender);
            yield return GetEndBracketFigure(sender);
        }

        private static Rect GetHorizontalStrokeRect(StateButton sender)
        {
            Rect iconRect = sender.IconRect;
            double thickness = iconRect.Height * iconHorizontalStrokeThicknessFactor;

            Point offset = new Point(iconRect.X, iconRect.Y + (iconRect.Height - thickness) / 2);
            Size size = new Size(iconRect.Width, thickness);

            return new Rect(offset, size);
        }

        private static PathFigure GetBeginBracketFigure(StateButton sender)
        {
            Rect iconRect = sender.IconRect;
            double thickness = iconRect.Height * iconVerticalStrokeThicknessFactor;
            double beginCloseWidth = iconRect.Width * highlightIconBeginCloseWidthFactor + thickness;
            double offsetX = iconRect.X + iconRect.Width * (1 - highlightIconVerticalStrokeDistanceFactor) / 2;

            Point pTopLeft = new Point(offsetX, iconRect.Y);
            Point pBottomLeft = new Point(offsetX, iconRect.Y + iconRect.Height);
            Point pTopInner = new Point(offsetX + thickness, iconRect.Y + thickness);
            Point pBottomInner = new Point(offsetX + thickness, iconRect.Y + iconRect.Height - thickness);
            Point pTopRightTop = new Point(offsetX + beginCloseWidth, iconRect.Y);
            Point pTopRightBottom = new Point(offsetX + beginCloseWidth, iconRect.Y + thickness);
            Point pBottomRightTop = new Point(offsetX + beginCloseWidth, iconRect.Y + iconRect.Height - thickness);
            Point pBottomRightBottom = new Point(offsetX + beginCloseWidth, iconRect.Y + iconRect.Height);

            return StateButton.GetFigure(pTopLeft, pTopRightTop, pTopRightBottom, pTopInner,
                pBottomInner, pBottomRightTop, pBottomRightBottom, pBottomLeft);
        }

        private static PathFigure GetEndBracketFigure(StateButton sender)
        {
            Rect iconRect = sender.IconRect;
            double thickness = iconRect.Height * iconVerticalStrokeThicknessFactor;
            double beginCloseWidth = iconRect.Width * highlightIconBeginCloseWidthFactor + thickness;
            double offsetX = iconRect.X + iconRect.Width * (1 + highlightIconVerticalStrokeDistanceFactor) / 2;

            Point pTopRight = new Point(offsetX, iconRect.Y);
            Point pBottomRight = new Point(offsetX, iconRect.Y + iconRect.Height);
            Point pTopInner = new Point(offsetX - thickness, iconRect.Y + thickness);
            Point pBottomInner = new Point(offsetX - thickness, iconRect.Y + iconRect.Height - thickness);
            Point pTopLeftTop = new Point(offsetX - beginCloseWidth, iconRect.Y);
            Point pTopLeftBottom = new Point(offsetX - beginCloseWidth, iconRect.Y + thickness);
            Point pBottomLeftTop = new Point(offsetX - beginCloseWidth, iconRect.Y + iconRect.Height - thickness);
            Point pBottomLeftBottom = new Point(offsetX - beginCloseWidth, iconRect.Y + iconRect.Height);

            return StateButton.GetFigure(pTopRight, pTopLeftTop, pTopLeftBottom, pTopInner,
                pBottomInner, pBottomLeftTop, pBottomLeftBottom, pBottomRight);
        }

        private static void RenderMediaIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Rect horizontalStrokeRect = GetHorizontalStrokeRect(sender);
            IEnumerable<PathFigure> figures = GetMediaIconFigures(sender);

            Brush iconBrush = Brushes.Black;
            Pen pen = new Pen(iconBrush, 0);

            context.DrawRectangle(iconBrush, pen, horizontalStrokeRect);
            context.DrawGeometry(iconBrush, pen, new PathGeometry(figures));
        }

        private static IEnumerable<PathFigure> GetMediaIconFigures(StateButton sender)
        {
            yield return GetVerticalStrokeBeginFigure(sender);
            yield return GetVerticalStrokeEndFigure(sender);
        }

        private static PathFigure GetVerticalStrokeBeginFigure(StateButton sender)
        {
            Rect iconRect = sender.IconRect;
            double thickness = iconRect.Height * iconVerticalStrokeThicknessFactor;

            Point p11 = iconRect.TopLeft;
            Point p12 = iconRect.BottomLeft;
            Point p21 = new Point(iconRect.X + thickness, iconRect.Y);
            Point p22 = new Point(iconRect.X + thickness, iconRect.Y + iconRect.Height);

            return StateButton.GetFigure(p11, p12, p22, p21);
        }

        private static PathFigure GetVerticalStrokeEndFigure(StateButton sender)
        {
            Rect iconRect = sender.IconRect;
            double thickness = iconRect.Height * iconVerticalStrokeThicknessFactor;

            Point p11 = iconRect.TopRight;
            Point p12 = iconRect.BottomRight;
            Point p21 = new Point(iconRect.X + iconRect.Width - thickness, iconRect.Y);
            Point p22 = new Point(iconRect.X + iconRect.Width - thickness, iconRect.Y + iconRect.Height);

            return StateButton.GetFigure(p11, p12, p22, p21);
        }
    }
}
