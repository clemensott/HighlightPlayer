using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    class MediaPositionSliderUnproceededElement : MediaPositionSliderElement
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

        public MediaPositionSliderUnproceededElement(MediaPositionSlider parent,
            Brush brush, double renderThickness) : base(parent,brush)
        {
            this.renderThickness = renderThickness;
        }

        public override IEnumerable<Rect> GetRects()
        {
            Point startPoint = GetOffset();
            startPoint.Offset(0, (parent.SliderRect.Height - RenderThickness) / 2);
            Point endPoint = parent.SliderRect.TopRight;
            endPoint.Offset(0, (parent.SliderRect.Height + RenderThickness) / 2);

            yield return new Rect(startPoint, endPoint);
        }
    }
}
