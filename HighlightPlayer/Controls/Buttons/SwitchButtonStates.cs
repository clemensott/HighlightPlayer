using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum SwitchViewState { Player, Medias }

    class SwitchViewButtonStates : BackgroundColorStateButtonSource
    {
        private const double mediasIconStrokeThicknessFactor = 0.15, mediasIconSquareStrokeDistanceFactor = 0.15,
            mediaElementIconStrokeHeightFactor = 0.7;

        public SwitchViewButtonStates() : base(Brushes.WhiteSmoke, Brushes.DeepSkyBlue, Brushes.CornflowerBlue, GetIconRenderers())
        {
            //InnerMarginPercent = new Thickness(20);
            //IconRatio = 1.3;
        }

        private static IEnumerable<IconRenderer> GetIconRenderers()
        {
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderMediasIcon), SwitchViewState.Player);
            yield return new IconRenderer(new Action<StateButton, DrawingContext, Brush>(RenderMediaElementIcon), SwitchViewState.Medias);
        }

        private static void RenderMediasIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Rect iconRect = sender.IconRect;
            double thickness = iconRect.Height * mediasIconStrokeThicknessFactor;
            double strokeOffsetX = iconRect.X + thickness + iconRect.Width * mediasIconSquareStrokeDistanceFactor;
            double middleOffsetY = iconRect.Y + (iconRect.Height - thickness) / 2;
            Size squareSize = new Size(thickness, thickness);
            Size strokeSize = new Size(iconRect.X + iconRect.Width - strokeOffsetX, thickness);
            double bottomOffsetY = iconRect.Y + iconRect.Height - squareSize.Height;

            Rect rectSquare1 = new Rect(iconRect.TopLeft, squareSize);
            Rect rectStroke1 = new Rect(new Point(strokeOffsetX, iconRect.Y), strokeSize);
            Rect rectSquare2 = new Rect(new Point(iconRect.X, middleOffsetY), squareSize);
            Rect rectStroke2 = new Rect(new Point(strokeOffsetX, middleOffsetY), strokeSize);
            Rect rectSquare3 = new Rect(new Point(iconRect.X, bottomOffsetY), squareSize);
            Rect rectStroke3 = new Rect(new Point(strokeOffsetX, bottomOffsetY), strokeSize);

            Brush squareBrush = Brushes.Black;
            Brush strokeBrush = Brushes.Gray;
            Pen pen = new Pen();

            context.DrawRectangle(squareBrush, pen, rectSquare1);
            context.DrawRectangle(strokeBrush, pen, rectStroke1);
            context.DrawRectangle(squareBrush, pen, rectSquare2);
            context.DrawRectangle(strokeBrush, pen, rectStroke2);
            context.DrawRectangle(squareBrush, pen, rectSquare3);
            context.DrawRectangle(strokeBrush, pen, rectStroke3);
        }

        private static void RenderMediaElementIcon(StateButton sender, DrawingContext context, Brush backgroundBrush)
        {
            Rect iconRect = sender.IconRect;
            Point pTopLeft = iconRect.TopLeft;
            Point pBottomRight = iconRect.BottomRight;

            double strokeOffsetY = iconRect.Y + iconRect.Height * mediaElementIconStrokeHeightFactor;
            Point pStrokeLeft = new Point(iconRect.X, strokeOffsetY);
            Point pStrokeRight = new Point(iconRect.X + iconRect.Width, strokeOffsetY);

            Pen framePen = new Pen(Brushes.Black, 1.5);
            Pen strokePen = new Pen(Brushes.Gray, 1);

            context.DrawRectangle(Brushes.Transparent, framePen, new Rect(pTopLeft, pBottomRight));
            context.DrawLine(strokePen, pStrokeLeft, pStrokeRight);
        }
    }
}
