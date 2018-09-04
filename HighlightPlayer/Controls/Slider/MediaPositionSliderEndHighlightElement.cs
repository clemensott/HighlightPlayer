using HighlightLib;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    class MediaPositionSliderEndHighlightElement : MediaPositionSliderElement
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

        public MediaPositionSliderEndHighlightElement(MediaPositionSlider parent,
           Highlight highlight, Brush brush, double renderThickness) : base(parent, brush)
        {
            this.renderThickness = renderThickness;
            Highlight = highlight;

            Highlight.EndChanged += Highlight_EndChanged;
        }

        private void Highlight_EndChanged(Highlight sender, TimeChangedEventArgs args)
        {
            parent.InvalidateVisual();
        }

        public override IEnumerable<Rect> GetRects()
        {
            Point positionOffset = GetOffset();

            Point endOffsetTop = positionOffset;
            endOffsetTop.Offset(-1.5 * RenderThickness, 0);

            Point endOffsetRight = positionOffset;
            endOffsetRight.Offset(RenderThickness / 2, 0);

            Point endOffsetBottom = positionOffset;
            endOffsetBottom.Offset(-1.5 * RenderThickness, parent.SliderRect.Height - RenderThickness);

            Size sizeVertical = new Size(2 * RenderThickness, RenderThickness);
            Size sizeHorizontal = new Size(RenderThickness, parent.SliderRect.Height);

            yield return new Rect(endOffsetTop, sizeHorizontal);
            yield return new Rect(endOffsetBottom, sizeHorizontal);
            yield return new Rect(endOffsetRight, sizeVertical);
        }
    }
}
