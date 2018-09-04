using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum PlayPauseState { Play, Pause }

    class PlayPauseButtonStates : BackgroundColorStateButtonSource
    {
        private const double pauseIconBetweenFactor = 0.1;

        public PlayPauseButtonStates() : base(Brushes.WhiteSmoke, Brushes.DeepSkyBlue, Brushes.CornflowerBlue, GetIconRenderers())
        {
            //InnerMarginPercent = new Thickness(20);
        }

        private static IEnumerable<IconRenderer> GetIconRenderers()
        {
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderPlayIcon), PlayPauseState.Pause);
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderPauseIcon), PlayPauseState.Play);
        }

        private static void RenderPlayIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Rect iconRect = sender.IconRect;

            Point p1 = iconRect.TopLeft;
            Point p2 = new Point(iconRect.X + iconRect.Width, iconRect.Y + iconRect.Height / 2);
            Point p3 = iconRect.BottomLeft;

            PathSegment[] seg = new PathSegment[] { new LineSegment(p2, false), new LineSegment(p3, false), new LineSegment(p1, false) };
            PathFigure[] figures = new PathFigure[] { new PathFigure(p1, seg, true) };

            Geometry geo = new PathGeometry(figures);
            Pen pen = new Pen(Brushes.Black, 0.5);

            context.DrawGeometry(Brushes.Gray, pen, geo);

            context.DrawLine(pen, p1, p2);
            context.DrawLine(pen, p2, p3);
            context.DrawLine(pen, p3, p1);
        }

        private static void RenderPauseIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Rect iconRect = sender.IconRect;

            Point p11 = iconRect.TopLeft;
            Point p12 = new Point(iconRect.X + iconRect.Width * (0.5 - pauseIconBetweenFactor), iconRect.Y + iconRect.Height);
            Point p21 = new Point(iconRect.X + iconRect.Width * (0.5 + pauseIconBetweenFactor), iconRect.Y);
            Point p22 = iconRect.BottomRight;

            Rect rect1 = new Rect(p11, p12);
            Rect rect2 = new Rect(p21, p22);

            Pen pen = new Pen(Brushes.Black, 0.5);

            context.DrawRectangle(Brushes.Gray, pen, rect1);
            context.DrawRectangle(Brushes.Gray, pen, rect2);
        }
    }
}
