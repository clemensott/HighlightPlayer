using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public enum MousePosition { On, Click, Out }

    public delegate void ClickHandler(object sender, ButtonStateChangingEventArgs args);
    public delegate void ButtonStateChangingHandler(object sender, ButtonStateChangingEventArgs args);
    public delegate void ButtonStateChangedHandler(object sender, ButtonStateChangedEventArgs args);

    public class StateButton : UserControl
    {
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(Enum), typeof(StateButton),
                new PropertyMetadata(null, OnCurrentValuePropertyChanged));

        private static void OnCurrentValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = sender as StateButton;

            if ((s.Source?.Count ?? 0) > 0 && e.NewValue == null) s.CurrentValue = (Enum)e.OldValue;
            else
            {
                s.InvalidateVisual();

                ButtonStateChangedEventArgs changedArgs = new ButtonStateChangedEventArgs((Enum)e.OldValue, (Enum)e.NewValue);
                s.ButtonStateChanged?.Invoke(s, changedArgs);
            }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(StateButtonSource), typeof(StateButton),
                new PropertyMetadata(null, new PropertyChangedCallback(OnSourcePropertyChanged)));

        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (StateButton)sender;
            var newValue = (StateButtonSource)e.NewValue;
            var oldValue = (StateButtonSource)e.OldValue;

            if (newValue?.All(state => state.Value != s.CurrentValue) ?? true) s.CurrentValue = newValue.FirstOrDefault()?.Value;
            
            newValue?.AddParent(s);
            oldValue?.RemoveParent(s);
        }

        private double iconRatio;
        private Thickness innerMarginPercent;
        private MousePosition mouseState;

        public event ClickHandler Click;
        public event ButtonStateChangingHandler ButtonStateChanging;
        public event ButtonStateChangedHandler ButtonStateChanged;

        public double IconRatio
        {
            get { return iconRatio; }
            set
            {
                if (value == iconRatio || value <= 0 || double.IsInfinity(value) || double.IsNaN(value)) return;

                iconRatio = value;
                InvalidateVisual();
            }
        }

        public Thickness InnerMarginPercent
        {
            get { return innerMarginPercent; }
            set
            {
                if (value == innerMarginPercent || value.Left + value.Right >= 100 || value.Top + value.Bottom >= 100) return;

                innerMarginPercent = value;
                InvalidateVisual();
            }
        }

        public Rect IconRect { get; private set; }

        public Enum CurrentValue
        {
            get { return (Enum)GetValue(CurrentValueProperty); }
            set
            {
                if (Source?.All(s => s.Value != value) ?? true) return;

                Enum oldValue = CurrentValue;
                ButtonStateChangingEventArgs changingArgs = new ButtonStateChangingEventArgs(oldValue, value);
                ButtonStateChanging?.Invoke(this, changingArgs);

                if (changingArgs.NewValue.Equals(oldValue)) return;

                SetValue(CurrentValueProperty, value);
            }
        }

        public StateButtonSource Source
        {
            get { return (StateButtonSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public MousePosition MouseState
        {
            get { return mouseState; }
            set
            {
                if (value == mouseState) return;

                mouseState = value;
                InvalidateVisual();
            }
        }

        public StateButton()
        {
            iconRatio = 1;
            innerMarginPercent = new Thickness(0);

            mouseState = MousePosition.Out;

            BorderBrush = Brushes.Gray;
            BorderThickness = new Thickness(1);

            MouseEnter += UserControl_MouseEnter;
            MouseLeave += UserControl_MouseLeave;
            MouseLeftButtonDown += UserControl_MouseLeftButtonDown;
            MouseLeftButtonUp += UserControl_MouseLeftButtonUp;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            SetIconRect();
            FillUserControl(drawingContext);

            ButtonState currentState = Source?.FirstOrDefault(s => s.Value.Equals(CurrentValue));

            switch (MouseState)
            {
                case MousePosition.Out:
                    currentState?.RenderMouseOut(this, drawingContext);
                    break;

                case MousePosition.On:
                    currentState?.RenderMouseOn(this, drawingContext);
                    break;

                case MousePosition.Click:
                    currentState?.RenderMouseClick(this, drawingContext);
                    break;
            }
        }

        private void SetIconRect()
        {
            double maxSizeX = ActualWidth * (1 - InnerMarginPercent.Left / 100 - InnerMarginPercent.Right / 100);
            double maxSizeY = ActualHeight * (1 - InnerMarginPercent.Top / 100 - InnerMarginPercent.Bottom / 100);
            Size maxSize = new Size(maxSizeX, maxSizeY);

            Size iconSize = maxSize.Width / maxSize.Height < IconRatio ?
                   new Size(maxSize.Width, maxSize.Width / IconRatio) :
                   new Size(maxSize.Height * IconRatio, maxSize.Height);

            Point iconOffset = new Point((ActualWidth - iconSize.Width) / 2, (ActualHeight - iconSize.Height) / 2);

            IconRect = new Rect(iconOffset, iconSize);
        }

        private void FillUserControl(DrawingContext drawingContext)
        {
            Pen transparentPen = new Pen(Brushes.Transparent, 1);
            Rect rect = new Rect(0, 0, ActualWidth, ActualHeight);

            drawingContext.DrawRectangle(Brushes.Transparent, transparentPen, rect);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseState = e.LeftButton == MouseButtonState.Pressed ? MousePosition.Click : MousePosition.On;

            InvalidateVisual();
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseState = MousePosition.Out;

            InvalidateVisual();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MouseState = MousePosition.Click;

            InvalidateVisual();
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MouseState = MousePosition.On;

            SetNextState();
        }

        private void SetNextState()
        {
            if ((Source?.Count ?? 0) == 0) return;

            int currentStateIndex = Source.Select(s => s.Value).ToList().IndexOf(CurrentValue);
            int nextStateIndex = (currentStateIndex + 1) % Source.Count;

            Enum nextValue = Source.ElementAt(nextStateIndex).Value;

            ButtonStateChangingEventArgs changingArgs = new ButtonStateChangingEventArgs(CurrentValue, nextValue);
            Click?.Invoke(this, changingArgs);

            CurrentValue = nextValue;
        }

        public static PathFigure GetFigure(Point p1, Point p2, params Point[] points)
        {
            PathSegment[] seg = new PathSegment[1 + points.Length];
            seg[0] = new LineSegment(p2, false);

            for (int i = 0; i < points.Length; i++)
            {
                seg[1 + i] = new LineSegment(points[i], false);
            }

            return new PathFigure(p1, seg, true);
        }

        public static IEnumerable<PathFigure> GetFigures(Point p1, Point p2, params Point[] points)
        {
            yield return GetFigure(p1, p2, (Point[])points);
        }

        public static IEnumerable<PathFigure> GetFigureWithBoder(Point p1, Point p2, params Point[] points)
        {
            return GetBorderFigures(p1, p2, (Point[])points).Concat(GetFigures(p1, p2, (Point[])points));
        }

        public static IEnumerable<PathFigure> GetBorderFigures(Point p1, Point p2, params Point[] points)
        {
            yield return GetFigure(p1, p2);

            Point previousPoint = p2;

            for (int i = 0; i < points.Length; i++)
            {
                yield return GetFigure(previousPoint, points[i]);

                previousPoint = points[i];
            }
        }
    }
}
