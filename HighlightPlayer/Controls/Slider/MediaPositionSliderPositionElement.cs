using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    class MediaPositionSliderPositionElement : MediaPositionSliderElement
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

        public MediaPositionSliderPositionElement(MediaPositionSlider parent,
            Brush brush, double renderThickness) : base(parent, brush)
        {
            this.renderThickness = renderThickness;
        }

        public override IEnumerable<Rect> GetRects()
        {
            Point offset = GetOffset();
            offset.Offset(-RenderThickness / 2, 0);

            Size size = new Size(RenderThickness, parent.SliderRect.Height);

            yield return new Rect(offset, size);
        }
    }
}
