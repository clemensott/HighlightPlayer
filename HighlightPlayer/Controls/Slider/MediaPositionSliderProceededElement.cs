using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    class MediaPositionSliderProceededElement : MediaPositionSliderElement
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

        public MediaPositionSliderProceededElement(MediaPositionSlider parent,
            Brush brush, double renderThickness) : base(parent, brush)
        {
            this.renderThickness = renderThickness;
        }

        public override IEnumerable<Rect> GetRects()
        {
            Point startPoint = parent.SliderRect.TopLeft;
            startPoint.Offset(0, (parent.SliderRect.Height - RenderThickness) / 2);
            Point endPoint = GetOffset();
            endPoint.Offset(0, (parent.SliderRect.Height + RenderThickness) / 2);

            yield return new Rect(startPoint, endPoint);
        }
    }
}
