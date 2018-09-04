using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    abstract class MediaPositionSliderElement
    {
        protected const double tolerance = 5;

        protected MediaPositionSlider parent;
        protected bool isHighlighted;
        protected Brush brush;

        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set
            {
                if (value == isHighlighted) return;

                isHighlighted = value;
                parent.InvalidateVisual();
            }
        }

        public double Tolerance { get { return tolerance; } }

        public Brush Brush
        {
            get { return brush; }
            set
            {
                if (value == brush) return;

                brush = value;

                parent.InvalidateVisual();
            }
        }

        public MediaPositionSliderElement(MediaPositionSlider parent) : this(parent, Brushes.Black)
        {
        }

        public MediaPositionSliderElement(MediaPositionSlider parent, Brush brush)
        {
            this.parent = parent;
            this.brush = brush;
        }

        public bool IsPointOn(Point point)
        {
            point.Offset(-Tolerance, -Tolerance);
            Rect pointRect = new Rect(point, new Size(2 * Tolerance, 2 * Tolerance));

            foreach (Rect rect in GetRects())
            {
                if (pointRect.IntersectsWith(rect)) return true;
            }

            return false;
        }

        public void Render(DrawingContext context)
        {
            Pen iconPen = new Pen(Brushes.Black, IsHighlighted ? 1 : 0);

            foreach (Rect rect in GetRects())
            {
                context.DrawRectangle(Brush, iconPen, rect);
            }
        }

        public abstract IEnumerable<Rect> GetRects();

        protected Point GetOffset()
        {
            return GetOffset(parent.UiPositionFactor);
        }

        protected Point GetOffset(double factor)
        {
            double x = parent.SliderRect.Left + parent.SliderRect.Width * factor;

            return new Point(x, parent.SliderRect.Top);
        }

        protected Point GetOffset(TimeSpan position)
        {
            return GetOffset(position.TotalDays / parent.MediaDuration.TotalDays);
        }
    }
}
