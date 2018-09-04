using HighlightLib;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System;

namespace HighlightPlayer.Controls
{
    class MediaPositionSliderHighlightElement : MediaPositionSliderElement
    {
        private double renderThickness;

        public double RenderThickness
        {
            get { return renderThickness; }
            set
            {
                if (value == renderThickness) return;

                renderThickness = value;
                parent.InvalidateVisual();
            }
        }

        public Highlight Highlight { get; private set; }

        public MediaPositionSliderHighlightElement(MediaPositionSlider parent,
            Highlight highlight, Brush brush, double renderThickness) : base(parent, brush)
        {
            this.renderThickness = renderThickness;
            Highlight = highlight;

            Highlight.BeginChanged += Highlight_Changed;
            Highlight.EndChanged += Highlight_Changed;
        }

        private void Highlight_Changed(Highlight sender, EventArgs args)
        {
            parent.InvalidateVisual();
        }

        public override IEnumerable<Rect> GetRects()
        {
            return GetBeginRects().Concat(GetEndRects());
        }

        private IEnumerable<Rect> GetBeginRects()
        {
            Point beginPositionOffset = GetOffset(Highlight.Begin);

            Point beginOffsetTop = beginPositionOffset;
            beginOffsetTop.Offset(-RenderThickness / 2, 0);

            Point beginOffsetBottom = beginOffsetTop;
            beginOffsetBottom.Offset(0, parent.SliderRect.Height - RenderThickness);

            Size sizeHorizontal = new Size(2 * RenderThickness, RenderThickness);
            Size sizeVertical = new Size(RenderThickness, parent.SliderRect.Height);

            yield return new Rect(beginOffsetTop, sizeHorizontal);
            yield return new Rect(beginOffsetBottom, sizeHorizontal);
            yield return new Rect(beginOffsetTop, sizeVertical);
        }

        private IEnumerable<Rect> GetEndRects()
        {
            if (!Highlight.IsHighlightClosed()) yield break;

            Point endPositionOffset = GetOffset(Highlight.End);

            Point endOffsetTop = endPositionOffset;
            endOffsetTop.Offset(-0.5 * RenderThickness, 0);

            Point endOffsetRight = endPositionOffset;
            endOffsetRight.Offset(RenderThickness / 2, 0);

            Point endOffsetBottom = endPositionOffset;
            endOffsetBottom.Offset(-0.5 * RenderThickness, parent.SliderRect.Height - RenderThickness);

            Size sizeHorizontal = new Size(2 * RenderThickness, RenderThickness);
            Size sizeVertical = new Size(RenderThickness, parent.SliderRect.Height);

            yield return new Rect(endOffsetTop, sizeHorizontal);
            yield return new Rect(endOffsetBottom, sizeHorizontal);
            yield return new Rect(endOffsetRight, sizeVertical);
        }

        public bool IsPointOnBegin(Point point)
        {
            point.Offset(-Tolerance, -Tolerance);
            Rect pointRect = new Rect(point, new Size(2 * Tolerance, 2 * Tolerance));

            foreach (Rect rect in GetBeginRects())
            {
                if (pointRect.IntersectsWith(rect)) return true;
            }

            return false;
        }

        public bool IsPointOnEnd(Point point)
        {
            point.Offset(-Tolerance, -Tolerance);
            Rect pointRect = new Rect(point, new Size(2 * Tolerance, 2 * Tolerance));

            foreach (Rect rect in GetEndRects())
            {
                if (pointRect.IntersectsWith(rect)) return true;
            }

            return false;
        }
    }
}
