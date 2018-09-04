using System;
using System.Windows;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    class BackgroundColorButtonState : ButtonState
    {
        private Brush backgroundBrushOut, backgroundBrushOn, backgroundBrushClick;
        private Action<StateButton, DrawingContext, Brush> renderIcon;

        public BackgroundColorButtonState(Enum value, Action<StateButton, DrawingContext, Brush> renderIcon,
            Brush backgroundOut, Brush backgroundOn, Brush backgroundClick) : base(value)
        {
            InitializeComponent(renderIcon, backgroundOut, backgroundOn, backgroundClick);
        }

        public BackgroundColorButtonState(Enum value, string name, Action<StateButton, DrawingContext, Brush> renderIcon,
            Brush backgroundOut, Brush backgroundOn, Brush backgroundClick) : base(value, name)
        {
            InitializeComponent(renderIcon, backgroundOut, backgroundOn, backgroundClick);
        }

        private void InitializeComponent(Action<StateButton, DrawingContext, Brush> renderIcon,
            Brush backgroundOut, Brush backgroundOn, Brush backgroundClick)
        {
            backgroundBrushOut = backgroundOut;
            backgroundBrushOn = backgroundOn;
            backgroundBrushClick = backgroundClick;

            this.renderIcon = renderIcon;
        }

        public override void RenderMouseOut(StateButton sender, DrawingContext context)
        {
            RenderBackground(sender, context, backgroundBrushOut);

            renderIcon(sender, context, backgroundBrushOut);
        }

        public override void RenderMouseOn(StateButton sender, DrawingContext context)
        {
            RenderBackground(sender, context, backgroundBrushOn);

            renderIcon(sender, context, backgroundBrushOn);
        }

        public override void RenderMouseClick(StateButton sender, DrawingContext context)
        {
            RenderBackground(sender, context, backgroundBrushClick);

            renderIcon(sender, context, backgroundBrushClick);
        }

        private static void RenderBackground(StateButton sender, DrawingContext context, Brush background)
        {
            Pen transparentPen = new Pen(background, 0);
            Rect rect = new Rect(0, 0, sender.ActualWidth, sender.ActualHeight);

            context.DrawRectangle(background, transparentPen, rect);
        }
    }
}
