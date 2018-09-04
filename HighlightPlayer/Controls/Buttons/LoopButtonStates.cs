using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum LoopState { Off, All, Current }

    class LoopButtonStates : BackgroundColorStateButtonSource
    {
        private const double iconCurveThicknessFactor = 0.15, iconArrowWidthFactor = 0.30, iconArrowHeightFactor = 0.5,
            iconDeleteOffsetFactor = 0.01, iconOneOffsetWidthFactor = 0.15, iconOneOffsetHeightFactor = 0.05,
            iconOneStrokeThicknessFactor = 0.1, iconOneVerticalStrokeHeightFactor = 0.6,
            iconOneDiagonalStrokeWidthFactor = 0.15, iconOneDiagonalStrokeHeightFactor = 0.075;

        public LoopButtonStates() : base(Brushes.WhiteSmoke, Brushes.DeepSkyBlue, Brushes.CornflowerBlue, GetIconRenderers())
        {
            //InnerMarginPercent = new Thickness(20);
            //IconRatio = 1;
        }

        private static  IEnumerable<IconRenderer> GetIconRenderers()
        {
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderLoopOff),LoopState.Off);
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderLoopAll), LoopState.All);
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderLoopCurrent), LoopState.Current);
        }

        private static  void RenderLoopOff(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            RenderLoopOffAndAllIcon(sender, context, Brushes.LightGray, backgroundBrush);
        }

        private static  void RenderLoopAll(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            RenderLoopOffAndAllIcon(sender, context, Brushes.Black, backgroundBrush);
        }

        private static  void RenderLoopCurrent(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            RenderLoopCurrentIcon(sender, context, Brushes.Black, backgroundBrush);
        }

        private static  void RenderLoopOffAndAllIcon(StateButton sender, DrawingContext context, Brush iconBrush, Brush backgroundBrush)
        {
            Rect iconRect = sender.IconRect;
            double radiusMiddle = iconRect.Width * (1 - iconCurveThicknessFactor) / 2;
            Point middlePoint = new Point(sender.ActualWidth / 2, sender.ActualHeight / 2);

            Pen iconPen = new Pen(iconBrush, iconRect.Width * iconCurveThicknessFactor);
            Pen backgroundPen = new Pen(backgroundBrush, 0);

            Geometry geoTopDelete = GetTopDeleteGeometry(sender);
            Geometry geoArrow = GetArrowGeometry(sender);

            context.DrawEllipse(backgroundBrush, iconPen, middlePoint, radiusMiddle, radiusMiddle);
            context.DrawGeometry(backgroundBrush, backgroundPen, geoTopDelete);
            context.DrawGeometry(iconBrush, backgroundPen, geoArrow);
        }

        private static  void RenderLoopCurrentIcon(StateButton sender, DrawingContext context, Brush iconBrush, Brush backgroundBrush)
        {
            Rect iconRect = sender.IconRect;
            double radiusMiddle = iconRect.Width * (1 - iconCurveThicknessFactor) / 2;
            Point middlePoint = new Point(sender.ActualWidth / 2, sender.ActualHeight / 2);

            Pen iconPen = new Pen(iconBrush, iconRect.Width * iconCurveThicknessFactor);
            Pen backgroundPen = new Pen(backgroundBrush, 0);

            Geometry geoTopDelete = GetTopDeleteGeometry(sender);
            Geometry geoRightDelete = GetRightDeleteGeometry( sender);
            Geometry geoArrow = GetArrowGeometry(sender);
            Geometry geoOne = GetOneGeomerty(sender);

            context.DrawEllipse(backgroundBrush, iconPen, middlePoint, radiusMiddle, radiusMiddle);
            context.DrawGeometry(backgroundBrush, backgroundPen, geoTopDelete);
            context.DrawGeometry(backgroundBrush, backgroundPen, geoRightDelete);
            context.DrawGeometry(iconBrush, backgroundPen, geoArrow);
            context.DrawGeometry(iconBrush, backgroundPen, geoOne);
        }

        private static  Geometry GetTopDeleteGeometry(StateButton sender)
        {
            Rect iconRect = sender.IconRect;

            double deleteOffsetWidth = iconRect.Width * iconDeleteOffsetFactor;
            double deleteOffsetHeight = iconRect.Height * iconDeleteOffsetFactor;
            Point middlePoint = new Point(sender.ActualWidth / 2, sender.ActualHeight / 2);

            Point pEmpty1 = new Point(iconRect.X, iconRect.Y - deleteOffsetHeight);
            Point pEmpty2 = new Point((sender.ActualWidth + deleteOffsetWidth) / 2, iconRect.Y + (iconRect.Height - deleteOffsetHeight) / 2);
            Point pEmpty3 = new Point(iconRect.X + iconRect.Width + deleteOffsetWidth, iconRect.Y - deleteOffsetHeight);

            return new PathGeometry(StateButton.GetFigures(pEmpty1, pEmpty2, pEmpty3));
        }

        private static  Geometry GetRightDeleteGeometry(StateButton sender)
        {
            Rect iconRect = sender.IconRect;

            double deleteOffsetWidth = iconRect.Width * iconDeleteOffsetFactor;
            double deleteOffsetHeight = iconRect.Height * iconDeleteOffsetFactor;

            Point pEmpty1 = new Point(sender.ActualWidth / 2, sender.ActualHeight / 2);
            Point pEmpty2 = new Point(iconRect.X + iconRect.Width + deleteOffsetWidth, iconRect.Y + iconRect.Height + deleteOffsetHeight);
            Point pEmpty3 = new Point(iconRect.X + iconRect.Width + deleteOffsetWidth, iconRect.Y - deleteOffsetHeight);

            return new PathGeometry(StateButton.GetFigures(pEmpty1, pEmpty2, pEmpty3));
        }

        private static  Geometry GetArrowGeometry(StateButton sender)
        {
            Rect iconRect = sender.IconRect;

            double sqrt2 = Math.Sqrt(2);
            double arrowWidth = iconRect.Width * iconArrowWidthFactor;
            double radiusMiddle = iconRect.Width * (1 - iconCurveThicknessFactor) / 2;
            Point middlePoint = new Point(sender.ActualWidth / 2, sender.ActualHeight / 2);

            Vector pArrowOffset1 = new Vector(-radiusMiddle * (1 + iconArrowHeightFactor) / sqrt2,
                -radiusMiddle * (1 + iconArrowHeightFactor) / sqrt2);
            Vector pArrowOffset2 = new Vector(-radiusMiddle * (1 - iconArrowHeightFactor) / sqrt2,
                -radiusMiddle * (1 - iconArrowHeightFactor) / sqrt2);
            Vector pArrowOffset3 = new Vector((arrowWidth - radiusMiddle) / sqrt2, (-arrowWidth - radiusMiddle) / sqrt2);

            Point pArrow1 = Point.Add(middlePoint, pArrowOffset1);
            Point pArrow2 = Point.Add(middlePoint, pArrowOffset2);
            Point pArrow3 = Point.Add(middlePoint, pArrowOffset3);

            return new PathGeometry(StateButton.GetFigures(pArrow1, pArrow2, pArrow3));
        }

        private static  Geometry GetOneGeomerty(StateButton sender)
        {
            Rect iconRect = sender.IconRect;

            double diagonalStrokeWidth = iconRect.Width * iconOneDiagonalStrokeWidthFactor;
            double diagonalStrokeHeight = iconRect.Height * iconOneDiagonalStrokeHeightFactor;

            double thickness = iconRect.Width * iconOneStrokeThicknessFactor;
            double thicknessFactor = thickness * Math.Sqrt(1 / (Math.Pow(diagonalStrokeWidth, 2) + Math.Pow(diagonalStrokeHeight, 2)));

            double diagonalShortWidth = diagonalStrokeWidth * thicknessFactor;
            double diagonalShortHeight = diagonalStrokeHeight * thicknessFactor;

            double diagonalLongWidth = thickness + diagonalStrokeWidth;
            double diagonalLongHeight = diagonalStrokeHeight + diagonalShortHeight;

            double diagonalMiddleWidth = diagonalStrokeWidth - diagonalShortHeight;
            double diagonalMiddleHeight = diagonalLongHeight / diagonalLongWidth * diagonalMiddleWidth;

            Vector vDiagonalShort = new Vector(diagonalShortHeight, diagonalShortWidth);
            Vector vDiagonalMiddle = new Vector(diagonalMiddleWidth, -diagonalMiddleHeight);
            Vector vDiagonalLong = new Vector(-diagonalLongWidth, diagonalLongHeight);

            Vector vVerticalLong = new Vector(0, iconRect.Height * iconOneVerticalStrokeHeightFactor);
            Vector vHorizontalShort = new Vector(-thickness, 0);

            Point pCornerSpire = new Point(iconRect.X + iconRect.Width * (1 - iconOneOffsetWidthFactor),
                iconRect.Y + iconRect.Height * iconOneOffsetHeightFactor);
            Point pMiddleTop = Point.Add(pCornerSpire, vDiagonalLong);
            Point pMiddleBottom = Point.Add(pMiddleTop, vDiagonalShort);
            Point pCornerInner = Point.Add(pMiddleBottom, vDiagonalMiddle);
            Point pBottomRight = Point.Add(pCornerSpire, vVerticalLong);
            Point pBottomLeft = Point.Add(pBottomRight, vHorizontalShort);

            return new PathGeometry(StateButton.GetFigures(pCornerSpire, pMiddleTop, pMiddleBottom, pCornerInner, pBottomLeft, pBottomRight));
        }
    }
}
