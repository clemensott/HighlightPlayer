using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum ShuffleState { Off, OneTime, Complete }

    class ShuffleButtonStates : BackgroundColorStateButtonSource
    {
        private const double iconCurveRaiseSpeed = 1.5, iconCurveRaiseFactor = 500000,
            iconCurveMinValueFactor = 0.01, iconCurveThicknessFactor = 0.21,
            iconArrowWidthFactor = 0.25, iconArrowHeightFactor = 0.6, iconArrowHeightOffsetFactor = 0.02;

        public ShuffleButtonStates() : base(Brushes.WhiteSmoke, Brushes.DeepSkyBlue, Brushes.CornflowerBlue, GetIconRenderers())
        {
            //InnerMarginPercent = new Thickness(15);
            //IconRatio = 1.8;
        }

        private static IEnumerable<IconRenderer> GetIconRenderers()
        {
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderShuffleOff), ShuffleState.Off);
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderShuffleOneTime), ShuffleState.OneTime);
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderShuffleComplete), ShuffleState.Complete);
        }

        private static void RenderShuffleOff(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            RenderShuffleIcon(sender, context, Brushes.LightGray, backgroundBrush);
        }

        private static void RenderShuffleOneTime(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            RenderShuffleIcon(sender, context, Brushes.Black, backgroundBrush);
        }

        private static void RenderShuffleComplete(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            RenderShuffleIcon(sender, context, Brushes.Purple, backgroundBrush);
        }

        private static void RenderShuffleIcon(StateButton sender, DrawingContext context, Brush iconBrush, Brush backgroundBrush)
        {
            RenderDownCurve(sender, context, iconBrush);
            RenderUpCurve(sender, context, iconBrush, backgroundBrush);
            RenderArrow(sender, context, iconBrush);
        }

        private static void RenderDownCurve(StateButton sender, DrawingContext context, Brush iconBrush)
        {
            Rect iconRect = sender.IconRect;
            Pen iconPen = new Pen(iconBrush, iconRect.Height * iconCurveThicknessFactor);

            double startX = GetCurveX(iconCurveMinValueFactor);
            double endX = GetCurveX(1 - iconCurveMinValueFactor);
            double xPerPixel = (endX - startX) / (iconRect.Width * (1 - iconArrowWidthFactor / 2));

            Point previousPoint = new Point(iconRect.X, iconRect.Y);

            for (double xPixel = 0, xValue = startX; xValue <= endX; xValue += xPerPixel, xPixel++)
            {
                double yValue = (GetCurveValue(xValue) - iconCurveMinValueFactor) /
                    (1 - 2 * iconCurveMinValueFactor) * iconRect.Height + iconRect.Y;

                Point currentPoint = new Point(xPixel + iconRect.X, yValue);
                context.DrawLine(iconPen, previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }

        private static void RenderUpCurve(StateButton sender, DrawingContext context, Brush iconBrush, Brush backgroundBrush)
        {
            Rect iconRect = sender.IconRect;
            Pen iconPen = new Pen(iconBrush, iconRect.Height * iconCurveThicknessFactor);
            Pen backgroundPen = new Pen(backgroundBrush, iconRect.Height * iconCurveThicknessFactor * 2);

            double startX = GetCurveX(iconCurveMinValueFactor);
            double endX = GetCurveX(1 - iconCurveMinValueFactor);
            double xPerPixel = (endX - startX) / (iconRect.Width * (1 - iconArrowWidthFactor / 2));

            Point previousPoint = new Point(iconRect.X, iconRect.Y + iconRect.Height);

            for (double xPixel = 0, xValue = startX; xValue <= endX; xValue += xPerPixel, xPixel++)
            {
                double yValue = (GetCurveValue(xValue) - iconCurveMinValueFactor) /
                    (1 - 2 * iconCurveMinValueFactor) * iconRect.Height + iconRect.Y;

                Point currentPoint = new Point(xPixel + iconRect.X, sender.ActualHeight - yValue);
                context.DrawLine(backgroundPen, previousPoint, currentPoint);
                context.DrawLine(iconPen, previousPoint, currentPoint);
                previousPoint = currentPoint;
            }
        }

        private static void RenderArrow(StateButton sender, DrawingContext context, Brush iconBrush)
        {
            Rect iconRect = sender.IconRect;
            Pen iconPen = new Pen(iconBrush, iconRect.Height * iconCurveThicknessFactor);
            Size arrowSize = new Size(iconRect.Width * iconArrowWidthFactor, iconRect.Height * iconArrowHeightFactor);

            Point upperArrowOffset = new Point(iconRect.X + iconRect.Width * (1 - iconArrowWidthFactor),
                iconRect.Y + iconRect.Height * iconArrowHeightOffsetFactor);
            PathFigure upperArrow = GetArrowFigure(upperArrowOffset, arrowSize);

            Point lowerArrowOffset = new Point(iconRect.X + iconRect.Width * (1 - iconArrowWidthFactor),
                iconRect.Y + iconRect.Height * (1 - iconArrowHeightOffsetFactor));
            PathFigure lowerArrow = GetArrowFigure(lowerArrowOffset, arrowSize);

            Geometry geo = new PathGeometry(new PathFigure[] { upperArrow, lowerArrow });
            context.DrawGeometry(iconBrush, iconPen, geo);
        }

        private static double GetCurveValue(double x)
        {
            return (Math.Pow(iconCurveRaiseSpeed, x)) /
                (Math.Pow(iconCurveRaiseSpeed, x) + iconCurveRaiseFactor - 1);
        }

        private static double GetCurveX(double value)
        {
            return Math.Log((iconCurveRaiseFactor - 1) / (1 / value - 1)) / Math.Log(iconCurveRaiseSpeed);
        }

        private static PathFigure GetArrowFigure(Point offset, Size arrowSize)
        {
            Point p1 = new Point(offset.X + arrowSize.Width, offset.Y);
            Point p2 = new Point(offset.X, offset.Y - arrowSize.Height / 2);
            Point p3 = new Point(offset.X, offset.Y + arrowSize.Height / 2);

            PathSegment[] seg = new PathSegment[] { new LineSegment(p2, false),
                new LineSegment(p3, false), new LineSegment(p1, false) };

            return new PathFigure(p1, seg, true);
        }
    }
}
