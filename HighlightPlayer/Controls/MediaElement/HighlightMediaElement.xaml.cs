using HighlightLib;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HighlightPlayer.Controls
{
    /// <summary>
    /// Interaktionslogik für HighlightMediaElement.xaml
    /// </summary>

    public partial class HighlightMediaElement : UserControl
    {
        private const double minPositionChangeMillis = 1;
        private static readonly TimeSpan defaultTimerInterval = TimeSpan.FromMilliseconds(250);

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(MediaFile), typeof(HighlightMediaElement),
                new PropertyMetadata(null, new PropertyChangedCallback(OnSourcePropertyChanged)));

        private static void OnSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (HighlightMediaElement)sender;
            var value = (MediaFile)e.NewValue;

            try
            {
                if (value != null)
                {
                    Uri uri = new Uri(value.FullPath);
                    s.player.Source = uri;
                    var timeline = new System.Windows.Media.MediaTimeline(new Uri(value.FullPath));
                    timeline.BeginTime = s.PositionTimer;
                    s.player.Clock = timeline.CreateClock();
                    //s.Position = s.PositionTimer;
                    s.UpdateCurrentHighlights();
                }
            }
            catch
            {
                s.Source = null;
            }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(MediaState), typeof(HighlightMediaElement),
                new PropertyMetadata(MediaState.Pause, new PropertyChangedCallback(OnStatePropertyChanged)));

        private static void OnStatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (HighlightMediaElement)sender;
            var value = (MediaState)e.NewValue;

            if (value == MediaState.Play) s.player.Clock.Controller.Resume();
            else s.player.Clock.Controller.Pause();

            s.SetTickTimer();
            s.positionTimer.IsEnabled = true;// value == MediaState.Play;
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(PlayTypeState), typeof(HighlightMediaElement),
                new PropertyMetadata(PlayTypeState.Medias, new PropertyChangedCallback(OnModePropertyChanged)));

        private static void OnModePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (HighlightMediaElement)sender;
            var value = (PlayTypeState)e.NewValue;
        }

        public static readonly DependencyProperty PositionTimerProperty =
            DependencyProperty.Register("PositionTimer", typeof(TimeSpan), typeof(HighlightMediaElement),
                new PropertyMetadata(new TimeSpan(), OnPositionTimerPropertyChanged));

        private static void OnPositionTimerPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (HighlightMediaElement)sender;
            var value = (TimeSpan)e.NewValue;

            s.SetValue(NaturalDurationProperty, s.player.NaturalDuration);
            s.Position = value;
        }

        public static readonly DependencyProperty TimerIntervalProperty =
            DependencyProperty.Register("TimerInterval", typeof(TimeSpan), typeof(HighlightMediaElement),
                new PropertyMetadata(defaultTimerInterval, new PropertyChangedCallback(OnTimerIntervalPropertyChanged)));

        private static void OnTimerIntervalPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (HighlightMediaElement)sender;
            var value = (TimeSpan)e.NewValue;

            s.positionTimer.Interval = value;
        }

        public static readonly DependencyProperty NaturalDurationProperty =
            DependencyProperty.Register("NaturalDuration", typeof(Duration), typeof(HighlightMediaElement),
                new PropertyMetadata(Duration.Automatic, new PropertyChangedCallback(OnNaturalDurationPropertyChanged)));

        private static void OnNaturalDurationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (HighlightMediaElement)sender;
            var value = (Duration)e.NewValue;
        }

        public static readonly DependencyProperty CurrentHighlightsProperty =
            DependencyProperty.Register("CurrentHighlights", typeof(Highlight[]), typeof(HighlightMediaElement),
                new PropertyMetadata(new Highlight[0], new PropertyChangedCallback(OnCurrentHighlightsPropertyChanged)));

        private static void OnCurrentHighlightsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var s = (HighlightMediaElement)sender;
            var oldCurrentHighlights = (Highlight[])e.OldValue;
            var newCurrentHighlights = (Highlight[])e.NewValue;

            var args = new CurrentHighlightsChangedEventArgs(oldCurrentHighlights, newCurrentHighlights, s.Position);
            s.CurrentHighlightsChanged?.Invoke(s, args);
        }

        public event EventHandler<CurrentHighlightsChangedEventArgs> CurrentHighlightsChanged;
        public event EventHandler<RoutedEventArgs> MediaEnded;

        private DispatcherTimer tickTimer, positionTimer;

        public MediaFile Source
        {
            get { return (MediaFile)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public MediaState State
        {
            get { return (MediaState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public PlayTypeState Mode
        {
            get { return (PlayTypeState)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public TimeSpan Position
        {
            get { return player.Clock?.CurrentTime ?? TimeSpan.Zero; }
            set
            {
                TimeSpan currentPosition = Position;
                PositionTimer = value = GetPosition(value);

                if (Math.Abs((value - currentPosition).TotalMilliseconds) < minPositionChangeMillis) return;

                player.Clock.Controller.Seek(value, System.Windows.Media.Animation.TimeSeekOrigin.BeginTime);

                DoTick();
            }
        }

        public TimeSpan PositionTimer
        {
            get { return (TimeSpan)GetValue(PositionTimerProperty); }
            set { SetValue(PositionTimerProperty, value); }
        }

        public TimeSpan TimerInterval
        {
            get { return (TimeSpan)GetValue(TimerIntervalProperty); }
            set { SetValue(TimerIntervalProperty, value); }
        }

        public Duration NaturalDuration
        {
            get { return (Duration)GetValue(NaturalDurationProperty); }
            private set { SetValue(NaturalDurationProperty, value); }
        }

        public Highlight[] CurrentHighlights
        {
            get { return (Highlight[])GetValue(CurrentHighlightsProperty); }
            private set { SetValue(CurrentHighlightsProperty, value); }
        }

        public HighlightMediaElement()
        {
            InitializeComponent();

            tickTimer = new DispatcherTimer();
            tickTimer.Tick += TickTimer_Tick;

            positionTimer = new DispatcherTimer();
            positionTimer.Interval = TimeSpan.FromMilliseconds(250);
            positionTimer.Tick += PositionTimer_Tick;
        }

        private void TickTimer_Tick(object sender, EventArgs e)
        {
            DoTick();
        }

        private void DoTick()
        {
            tickTimer.Stop();

            Position = GetPosition(Position);

            if (NaturalDuration.HasTimeSpan && Position >= NaturalDuration.TimeSpan)
            {
                MediaEnded?.Invoke(this, new RoutedEventArgs());
            }

            SetTickTimer();
        }

        private void PositionTimer_Tick(object sender, EventArgs e)
        {
            double deltaDays = Math.Abs((PositionTimer - Position).TotalDays);

            if (deltaDays < TimerInterval.TotalDays * 2) PositionTimer = Position;
            else Position = PositionTimer;
        }

        private void UpdateCurrentHighlights()
        {
            Highlight[] newCurrentHighlights = Source != null ?
                Source.Highlights.Where(h => h.IsIn(Position)).ToArray() : new Highlight[0];

            if (newCurrentHighlights.SequenceEqual(CurrentHighlights)) return;
            CurrentHighlights = newCurrentHighlights;
        }

        private void SetTickTimer()
        {
            TimeSpan value = GetNextCurrentHighlightsChange(Position) ?? TimeSpan.FromMilliseconds(-1);
            TimeSpan delta = value - Position;

            if (delta.Ticks > 0 && (!NaturalDuration.HasTimeSpan || value < NaturalDuration.TimeSpan))
            {
                tickTimer.Interval = delta;
                tickTimer.Start();
            }
            else tickTimer.Stop();
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            NaturalDuration = player.NaturalDuration;
            Position = PositionTimer;
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            //DoTick();
            //MediaEnded?.Invoke(this, new RoutedEventArgs());
        }

        private void SetSource()
        {
            if (Source != null)
            {
                Uri uri = new Uri(Source.FullPath);
                var timeline = new System.Windows.Media.MediaTimeline(uri);

                player.Source = uri;

                timeline.BeginTime = PositionTimer;
                player.Clock = timeline.CreateClock();
                //s.Position = s.PositionTimer;
                UpdateCurrentHighlights();
            }
        }

        private TimeSpan GetPosition(TimeSpan newPosition)
        {
            if (Mode == PlayTypeState.Medias) return newPosition;
            if (Source.Highlights.Where(h => h.IsIn(newPosition)).Any()) return newPosition;

            TimeSpan? nextPosition = GetNextCurrentHighlightsChange(newPosition);
            if (nextPosition != null) return (TimeSpan)nextPosition;
            return NaturalDuration.HasTimeSpan ? NaturalDuration.TimeSpan : TimeSpan.MaxValue;
        }

        private TimeSpan? GetNextCurrentHighlightsChange(TimeSpan currentPosition)
        {
            TimeSpan? nextTimeSpan = null;

            foreach (Highlight highlight in Source?.Highlights ?? Enumerable.Empty<Highlight>())
            {
                if (highlight.Begin > Position && highlight.Begin < (nextTimeSpan ?? TimeSpan.MaxValue))
                {
                    nextTimeSpan = highlight.Begin;
                }

                if (highlight.End > Position && highlight.End < (nextTimeSpan ?? TimeSpan.MaxValue))
                {
                    nextTimeSpan = highlight.End;
                }
            }

            return nextTimeSpan;
        }
    }
}
