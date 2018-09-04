using HighlightLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    public delegate void MediaPositionChangedHandler(MediaPositionSlider sender, MediaPositionChangedEventArgs args);

    public partial class MediaPositionSlider : UserControl
    {
        private static readonly TimeSpan defaultMinMediaPositionChange = TimeSpan.FromMilliseconds(200);
        private static readonly Thickness sliderMarginOffset = new Thickness(8, -3, 8, -3);

        public static readonly DependencyProperty MinMediaPositionChangeProperty =
            DependencyProperty.Register("MinMediaPositionChange", typeof(TimeSpan), typeof(MediaPositionSlider),
                new PropertyMetadata(defaultMinMediaPositionChange,
                    new PropertyChangedCallback(OnMinMediaPositionChangePropertyChanged)));

        private static void OnMinMediaPositionChangePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var value = (TimeSpan)e.NewValue;
        }

        public static readonly DependencyProperty MediaPositionFactorProperty =
            DependencyProperty.Register("MediaPositionFactor", typeof(double), typeof(MediaPositionSlider),
                new PropertyMetadata(0.0, new PropertyChangedCallback(OnMediaPositionFactorPropertyChanged)));

        private static void OnMediaPositionFactorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var value = (double)e.NewValue;

            s.MediaPosition = GetPosition(s.MediaDuration, value);
            s.InvalidateVisual();
        }

        public static readonly DependencyProperty MediaPositionProperty =
            DependencyProperty.Register("MediaPosition", typeof(TimeSpan), typeof(MediaPositionSlider),
                new PropertyMetadata(new TimeSpan(), new PropertyChangedCallback(OnMediaPositionPropertyChanged)));

        private static void OnMediaPositionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var oldValue = (TimeSpan)e.OldValue;
            var newValue = (TimeSpan)e.NewValue;
            double oldFactor = GetPositionFactor(oldValue, s.MediaDuration);
            double newFactor = GetPositionFactor(newValue, s.MediaDuration);

            s.MediaPositionFactor = newFactor;
            if (!s.IsSliding) s.UiPosition = newValue;

            var args = new MediaPositionChangedEventArgs(oldValue, newValue, oldFactor, newFactor, s.MediaDuration);
            s.MediaPositionChanged?.Invoke(s, args);

            s.SetHighlightState();
        }

        public static readonly DependencyProperty MediaDurationProperty =
            DependencyProperty.Register("MediaDuration", typeof(TimeSpan), typeof(MediaPositionSlider),
                new PropertyMetadata(new TimeSpan(), new PropertyChangedCallback(OnMediaDurationPropertyChanged)));

        private static void OnMediaDurationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var value = (TimeSpan)e.NewValue;

            s.tblDuration.Text = GetUiTime(value);
            s.MediaPositionFactor = GetPositionFactor(s.MediaPosition, value);

            s.InvalidateVisual();
        }

        public static readonly DependencyProperty IsSlidingProperty =
            DependencyProperty.Register("IsSliding", typeof(bool), typeof(MediaPositionSlider),
                new PropertyMetadata(false, new PropertyChangedCallback(OnIsSlidingPropertyChanged)));

        private static void OnIsSlidingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var value = (bool)e.NewValue;
        }

        public static readonly DependencyProperty UiPositionFactorProperty =
            DependencyProperty.Register("UiPositionFactor", typeof(double), typeof(MediaPositionSlider),
                new PropertyMetadata(0.0, new PropertyChangedCallback(OnUiPositionFactorPropertyChanged)));

        private static void OnUiPositionFactorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var value = (double)e.NewValue;

            s.UiPosition = GetPosition(s.MediaDuration, value);
            s.InvalidateVisual();
        }

        public static readonly DependencyProperty UiPositionProperty =
            DependencyProperty.Register("UiPosition", typeof(TimeSpan), typeof(MediaPositionSlider),
                new PropertyMetadata(new TimeSpan(), new PropertyChangedCallback(OnUiPositionPropertyChanged)));

        private static void OnUiPositionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var oldValue = (TimeSpan)e.OldValue;
            var newValue = (TimeSpan)e.NewValue;
            double oldFactor = GetPositionFactor(oldValue, s.MediaDuration);
            double newFactor = GetPositionFactor(newValue, s.MediaDuration);

            s.UiPositionFactor = newFactor;
            s.MousePosition = newValue;

            var args = new MediaPositionChangedEventArgs(oldValue, newValue, oldFactor, newFactor, s.MediaDuration);
            s.UiPositionChanged?.Invoke(s, args);

            s.tblPosition.Text = GetUiTime(newValue);
        }

        public static readonly DependencyProperty IsMouseOnProperty =
            DependencyProperty.Register("IsMouseOn", typeof(bool), typeof(MediaPositionSlider),
                new PropertyMetadata(false, new PropertyChangedCallback(OnIsMouseOnPropertyChanged)));

        private static void OnIsMouseOnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var value = (bool)e.NewValue;
        }

        public static readonly DependencyProperty MousePositionFactorProperty =
            DependencyProperty.Register("MousePositionFactor", typeof(double), typeof(MediaPositionSlider),
                new PropertyMetadata(0.0, new PropertyChangedCallback(OnMousePositionFactorPropertyChanged)));

        private static void OnMousePositionFactorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var value = (double)e.NewValue;

            s.MousePosition = GetPosition(s.MediaDuration, value);
            s.InvalidateVisual();
        }

        public static readonly DependencyProperty MousePositionProperty =
            DependencyProperty.Register("MousePosition", typeof(TimeSpan), typeof(MediaPositionSlider),
                new PropertyMetadata(new TimeSpan(), new PropertyChangedCallback(OnMousePositionPropertyChanged)));

        private static void OnMousePositionPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var oldValue = (TimeSpan)e.OldValue;
            var newValue = (TimeSpan)e.NewValue;
            double oldFactor = GetPositionFactor(oldValue, s.MediaDuration);
            double newFactor = GetPositionFactor(newValue, s.MediaDuration);

            s.MousePositionFactor = newFactor;

            var args = new MediaPositionChangedEventArgs(oldValue, newValue, oldFactor, newFactor, s.MediaDuration);
            s.MousePositionChanged?.Invoke(s, args);
        }

        public static readonly DependencyProperty HighlightsProperty =
            DependencyProperty.Register("Highlights", typeof(HighlightCollection), typeof(MediaPositionSlider),
                new PropertyMetadata(null, new PropertyChangedCallback(OnHighlightsPropertyChanged)));

        private static void OnHighlightsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var oldValue = (HighlightCollection)e.OldValue;
            var newValue = (HighlightCollection)e.NewValue;

            if (oldValue != null) oldValue.CollectionChanged -= s.Highlights_CollectionChanged;
            if (newValue != null) newValue.CollectionChanged += s.Highlights_CollectionChanged;

            s.SetHighlightElements(newValue, oldValue);
            s.SetHighlightState();
        }

        public static readonly DependencyProperty HighlightStateProperty =
            DependencyProperty.Register("HighlightState", typeof(HighlightState), typeof(MediaPositionSlider),
                new PropertyMetadata(HighlightState.Begin, new PropertyChangedCallback(OnHighlightStatePropertyChanged)));

        private static void OnHighlightStatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (MediaPositionSlider)sender;
            var value = (HighlightState)e.NewValue;

            s.SetHighlightState();
        }

        public event MediaPositionChangedHandler MediaPositionChanged;
        public event MediaPositionChangedHandler UiPositionChanged;
        public event MediaPositionChangedHandler MousePositionChanged;

        private Thickness sliderMargin;
        private MediaPositionSliderPositionElement positionElement;
        private MediaPositionSliderProceededElement proceededElement;
        private MediaPositionSliderUnproceededElement unproceededElement;
        private Dictionary<Highlight, MediaPositionSliderHighlightElement> highlightElements;

        public TimeSpan MinMediaPositionChange
        {
            get { return (TimeSpan)GetValue(MinMediaPositionChangeProperty); }
            set { SetValue(MinMediaPositionChangeProperty, value); }
        }

        public double MediaPositionFactor
        {
            get { return (double)GetValue(MediaPositionFactorProperty); }
            set { SetValue(MediaPositionFactorProperty, value); }
        }

        public TimeSpan MediaPosition
        {
            get { return (TimeSpan)GetValue(MediaPositionProperty); }
            set
            {
                TimeSpan delta = value - MediaPosition;
                if (IsSliding || delta.Duration() < MinMediaPositionChange) return;

                SetValue(MediaPositionProperty, value);
            }
        }

        public TimeSpan MediaDuration
        {
            get { return (TimeSpan)GetValue(MediaDurationProperty); }
            set { SetValue(MediaDurationProperty, value); }
        }

        public bool IsSliding
        {
            get { return (bool)GetValue(IsSlidingProperty); }
            private set { SetValue(IsSlidingProperty, value); }
        }

        public double UiPositionFactor
        {
            get { return (double)GetValue(UiPositionFactorProperty); }
            private set { SetValue(UiPositionFactorProperty, value); }
        }

        public TimeSpan UiPosition
        {
            get { return (TimeSpan)GetValue(UiPositionProperty); }
            private set { SetValue(UiPositionProperty, value); }
        }

        public bool IsMouseOn
        {
            get { return (bool)GetValue(IsMouseOnProperty); }
            private set { SetValue(IsMouseOnProperty, value); }
        }

        public double MousePositionFactor
        {
            get { return (double)GetValue(MousePositionFactorProperty); }
            private set { SetValue(MousePositionFactorProperty, value); }
        }

        public TimeSpan MousePosition
        {
            get { return (TimeSpan)GetValue(MousePositionProperty); }
            private set { SetValue(MousePositionProperty, value); }
        }

        public HighlightCollection Highlights
        {
            get { return (HighlightCollection)GetValue(HighlightsProperty); }
            set { SetValue(HighlightsProperty, value); }
        }

        public HighlightState HighlightState
        {
            get { return (HighlightState)GetValue(HighlightStateProperty); }
            private set { SetValue(HighlightStateProperty, value); }
        }

        public Thickness SliderMargin
        {
            get { return sliderMargin; }
            set
            {
                if (value == sliderMargin) return;

                sliderMargin = value;
                InvalidateVisual();
            }
        }

        public Rect SliderRect { get; private set; }

        public MediaPositionSlider()
        {
            InitializeComponent();

            MinMediaPositionChange = TimeSpan.FromSeconds(1);
            MediaDuration = new TimeSpan();
            MediaPosition = new TimeSpan();

            positionElement = new MediaPositionSliderPositionElement(this, Brushes.Blue, 10);
            proceededElement = new MediaPositionSliderProceededElement(this, Brushes.LightBlue, 8);
            unproceededElement = new MediaPositionSliderUnproceededElement(this, Brushes.LightGray, 8);
            highlightElements = new Dictionary<Highlight, MediaPositionSliderHighlightElement>();

            SetSliderRect();
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement highestParent = GetHighestParent();

            highestParent.MouseEnter += HighestParent_MouseEnter;
            highestParent.MouseMove += HighestParent_MouseMove;
            highestParent.MouseLeave += HighestParent_MouseLeave;

            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
            highestParent.PreviewMouseLeftButtonUp += HighestParent_PreviewMouseLeftButtonUp;
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            IsMouseOn = false;
        }

        private FrameworkElement GetHighestParent()
        {
            FrameworkElement element = this;

            while (true)
            {
                FrameworkElement parent = element.Parent as FrameworkElement;

                if (parent == null) return element;

                element = parent;
            }
        }

        private void Highlights_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            SetHighlightElements(args.NewItems?.OfType<Highlight>(), args.OldItems?.OfType<Highlight>());
        }

        private void SetHighlightElements(IEnumerable<Highlight> added, IEnumerable<Highlight> removed)
        {
            if (added == null) added = Enumerable.Empty<Highlight>();
            if (removed == null) removed = Enumerable.Empty<Highlight>();

            foreach (Highlight highlight in removed)
            {
                highlightElements.Remove(highlight);

                highlight.BeginChanged -= Highlight_Changed;
                highlight.EndChanged -= Highlight_Changed;
            }

            foreach (Highlight highlight in added ?? Enumerable.Empty<Highlight>())
            {
                var element = new MediaPositionSliderHighlightElement(this, highlight, Brushes.Gray, 3);
                highlightElements.Add(highlight, element);

                highlight.BeginChanged += Highlight_Changed;
                highlight.EndChanged += Highlight_Changed;
            }

            InvalidateVisual();
        }

        private void Highlight_Changed(Highlight sender, EventArgs args)
        {
            SetHighlightState();
        }

        private void TblPosition_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void TblDuration_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void SetSliderRect()
        {
            Thickness absSliderMargin = GetAbsoluteSliderMargin();

            double x = tblPosition.ActualWidth + absSliderMargin.Left;
            double y = (ActualHeight - tblPosition.ActualHeight) / 2 + absSliderMargin.Top;
            double w = ActualWidth - tblPosition.ActualWidth - absSliderMargin.Left -
                tblDuration.ActualWidth - absSliderMargin.Right;
            double h = tblPosition.ActualHeight - absSliderMargin.Top - absSliderMargin.Bottom;

            if (w >= 0 && h >= 0) SliderRect = new Rect(x, y, w, h);
        }

        private Thickness GetAbsoluteSliderMargin()
        {
            return new Thickness(SliderMargin.Left + sliderMarginOffset.Left, SliderMargin.Top + sliderMarginOffset.Top,
                SliderMargin.Right + sliderMarginOffset.Right, SliderMargin.Bottom + sliderMarginOffset.Bottom);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            SetSliderRect();

            base.OnRender(drawingContext);

            unproceededElement.Render(drawingContext);
            proceededElement.Render(drawingContext);
            positionElement.Render(drawingContext);

            foreach (var element in highlightElements.Values)
            {
                element.Render(drawingContext);
            }
        }

        private void HighestParent_MouseEnter(object sender, MouseEventArgs e)
        {
            SetHighlighted(e.GetPosition(this));
        }

        private void HighestParent_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && IsMouseOn) IsSliding = false;

            if (IsSliding)
            {
                double factor = (e.GetPosition(this).X - SliderRect.Left) / SliderRect.Width;

                if (factor < 0) factor = 0;
                else if (factor > 1) factor = 1;

                UiPositionFactor = factor;
            }

            SetHighlighted(e.GetPosition(this));
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsSliding) return;

            double factor = (e.GetPosition(this).X - SliderRect.Left) / SliderRect.Width;

            if (factor >= 0 && factor <= 1) MousePositionFactor = factor;
        }

        private void HighestParent_MouseLeave(object sender, MouseEventArgs e)
        {
            SetHighlighted(e.GetPosition(this));
        }

        private void SetHighlighted(Point mousePosition)
        {
            bool foundPositionOn = false;

            foreach (var element in highlightElements.Values)
            {
                SetHighlighted(element, mousePosition, ref foundPositionOn);
            }

            SetHighlighted(positionElement, mousePosition, ref foundPositionOn);

            proceededElement.IsHighlighted = unproceededElement.IsHighlighted = !foundPositionOn &&
                (proceededElement.IsPointOn(mousePosition) || unproceededElement.IsPointOn(mousePosition));
        }

        private void SetHighlighted(MediaPositionSliderElement element, Point mousePosition, ref bool foundPositionOn)
        {
            if (foundPositionOn) element.IsHighlighted = false;
            else if (element.IsPointOn(mousePosition)) element.IsHighlighted = foundPositionOn = true;
            else element.IsHighlighted = false;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseOn = true;
        }

        private void HighestParent_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            bool wasSliding = e.Handled = IsSliding;
            IsSliding = false;

            if (SliderRect.Width == 0) return;

            Point mousePoint = e.GetPosition(this);

            if (wasSliding)
            {
                MediaPositionFactor = (mousePoint.X - SliderRect.Left) / SliderRect.Width;
                SetHighlighted(mousePoint);
                return;
            }

            foreach (var highlightElement in highlightElements.Values)
            {
                if (highlightElement.IsPointOnBegin(mousePoint))
                {
                    MediaPosition = highlightElement.Highlight.Begin;
                    return;
                }
                else if (highlightElement.IsPointOnEnd(mousePoint))
                {
                    MediaPosition = highlightElement.Highlight.End;
                    return;
                }
            }

            if (!proceededElement.IsPointOn(mousePoint) && !unproceededElement.IsPointOn(mousePoint)) return;

            MediaPositionFactor = (mousePoint.X - SliderRect.Left) / SliderRect.Width;
            SetHighlighted(mousePoint);
        }

        private void SetHighlightState()
        {
            HighlightState = GetHighlightState(Highlights, MediaPosition);
        }

        public static HighlightState GetHighlightState(HighlightCollection highlights, TimeSpan position)
        {
            if (highlights == null) return HighlightState.Begin;

            return highlights.Any(h => !h.IsHighlightClosed() && h.Begin < position) ?
                HighlightState.End : HighlightState.Begin;
        }

        private static string GetUiTime(TimeSpan timeSpan)
        {
            try
            {
                string time = string.Empty;
                time += timeSpan.Hours > 0 ? timeSpan.Hours.ToString() + ":" : string.Empty;
                time += timeSpan.Hours > 0 ? string.Format("{0,2}", timeSpan.Minutes) : timeSpan.Minutes.ToString();
                time += string.Format(":{0,2}", timeSpan.Seconds);

                return time.Replace(" ", "0");
            }
            catch { }

            return "Catch";
        }

        private static double GetPositionFactor(TimeSpan position, TimeSpan duration)
        {
            if (duration.TotalDays > 0) return position.TotalDays / duration.TotalDays;
            return position.TotalDays > 0 ? 1 : 0;
        }

        private static TimeSpan GetPosition(TimeSpan duration, double positionFactor)
        {
            if (positionFactor > 1) positionFactor = 1;
            else if (positionFactor < 0) positionFactor = 0;

            return TimeSpan.FromDays(duration.TotalDays * positionFactor);
        }
    }
}
