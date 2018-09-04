using HighlightLib;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace HighlightPlayer.Controls
{
    delegate void MediaEndedEventHandler(HighlightTimeLineManager sender, MediaFile file);

    class HighlightTimeLineManager : INotifyPropertyChanged
    {
        private static readonly TimeSpan timeSpanMax = TimeSpan.FromDays(10);

        public event MediaEndedEventHandler MediaEnded;

        private bool isSeeking;
        //private MediaElement player;
        private MediaTimeline timeline;
        private MediaFile source, backupSource;
        private TimeSpan target;
        private PlayPauseState state;
        private PlayTypeState mode;
        private CurrentHighlights currentHighlights;
        private MediaClock clock;
        private DispatcherTimer timer;

        public MediaElement Player { get; private set; }
        //{
        //    get { return player; }
        //    set
        //    {
        //        if (value == player) return;

        //        if (player != null) player.Clock = null;
        //        player = value;
        //        if (player != null) player.Clock = Clock;

        //        OnPropertyChanged("Player");
        //    }
        //}

        public MediaFile Source
        {
            get { return source; }
            set
            {
                if (value == source) return;

                Unsubscribe(timeline);

                source = value;
                OnPropertyChanged("Source");


                if (source != null)
                {
                    timeline = new MediaTimeline(new Uri(source.FullPath));
                    timeline.FillBehavior = FillBehavior.HoldEnd;

                    backupSource = source;

                    source.LoadDuration();
                    Subscribe(timeline);
                }
                else timeline = null;

                target = GetNextTarget(TimeSpan.Zero);
                Clock = timeline?.CreateClock(true) as MediaClock;
                Clock?.Controller?.Begin();
                isSeeking = true;

                Position = TimeSpan.Zero;

                if (source != null) CurrentHighlights = new CurrentHighlights(source.Highlights, Position);
                else
                {
                    CurrentHighlights = null;

                    State = PlayPauseState.Pause;

                    OnPropertyChanged("Position");
                    OnPropertyChanged("NaturalDuration");
                }

                SetTimer();
            }
        }

        public TimeSpan Position
        {
            get { return Clock?.CurrentTime ?? new TimeSpan(); }
            set
            {
                if (Clock == null) return;

                value = CheckPosition(value);
                if (value == Position) return;

                target = GetNextTarget(Position);
                isSeeking = true;

                Clock.Controller.Seek(value, TimeSeekOrigin.BeginTime);
            }
        }

        public PlayPauseState State
        {
            get { return state; }
            set
            {
                if (value == state) return;

                state = value;
                OnPropertyChanged("State");

                SetPlayState();
                SetTimer();
            }
        }

        public Duration NaturalDuration { get { return Clock?.NaturalDuration ?? Duration.Automatic; } }

        public PlayTypeState Mode
        {
            get { return mode; }
            set
            {
                if (value == mode) return;

                mode = value;
                OnPropertyChanged("Mode");

                UpdatePosition();
                CheckTarget();
            }
        }

        public CurrentHighlights CurrentHighlights
        {
            get { return currentHighlights; }
            private set
            {
                if (value == currentHighlights) return;

                currentHighlights = value;
                OnPropertyChanged("CurrentHighlights");
            }
        }

        public MediaClock Clock
        {
            get { return clock; }
            private set
            {
                if (value == clock) return;

                Player.Clock = clock = value;
                OnPropertyChanged("Clock");
            }
        }

        public HighlightTimeLineManager(MediaElement player)
        {
            Player = player;
            CurrentHighlights = new CurrentHighlights();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            OnPropertyChanged("NaturalDuration");
            OnPropertyChanged("Position");
        }

        private void SetTimer()
        {
            timer.IsEnabled = state == PlayPauseState.Play && source != null;
        }

        private void Timeline_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            if (isSeeking)
            {
                isSeeking = false;

                OnPropertyChanged("NaturalDuration");
                OnPropertyChanged("Position");
            }

            CurrentHighlights.Position = Position;

            CheckTarget();
        }

        private void Timeline_CurrentStateInvalidated(object sender, EventArgs e)
        {
            if (State == PlayPauseState.Pause && Clock != null) Clock.Controller.Pause();
            if (Clock?.CurrentState == ClockState.Stopped) State = PlayPauseState.Pause;
        }

        private void Timeline_Completed(object sender, EventArgs e)
        {
            MediaEnded?.Invoke(this, Source);
        }

        private TimeSpan CheckPosition(TimeSpan newPosition)
        {
            if (Source == null) return new TimeSpan();
            if (Mode == PlayTypeState.Medias) return newPosition;
            if (Source.Highlights.Where(h => h.IsIn(newPosition)).Any()) return newPosition;

            return TryGetNextBegin(ref newPosition) ? newPosition : GetDurationAsTimeSpan();
        }

        private bool TryGetNextBegin(ref TimeSpan currentPosition)
        {
            foreach (Highlight highlight in Source?.Highlights.OrderByEnd() ?? Enumerable.Empty<Highlight>())
            {
                if (highlight.Begin <= currentPosition) continue;

                currentPosition = highlight.Begin;
                return true;
            }

            return false;
        }

        private TimeSpan GetNextTarget(TimeSpan currentPosition)
        {
            if (Source == null) return TimeSpan.MaxValue;

            return GetNextEnd(currentPosition)?.End ?? GetLastTarget();
        }

        private Highlight GetNextEnd(TimeSpan currentPosition)
        {
            foreach (Highlight highlight in Source?.Highlights.OrderByEnd() ?? Enumerable.Empty<Highlight>())
            {
                if (highlight.End <= currentPosition) continue;

                return highlight;
            }

            return null;
        }

        private TimeSpan GetLastTarget()
        {
            return Source?.Highlights.LastOrDefault()?.End ?? TimeSpan.Zero;
        }

        private TimeSpan GetDurationAsTimeSpan()
        {
            if (Source == null) return TimeSpan.Zero;
            if (NaturalDuration.HasTimeSpan) return NaturalDuration.TimeSpan;

            return Source.Duration == TimeSpan.Zero ? timeSpanMax :
                Source.Duration + TimeSpan.FromMilliseconds(10);
        }

        private void UpdatePosition()
        {
            if (timeline == null) return;

            target = GetNextTarget(Position);
            Position = Position;
        }

        private void SetPlayState()
        {
            if (Clock == null) return;

            if (Player != null) Player.Clock = Clock;

            if (State == PlayPauseState.Pause) Clock.Controller.Pause();
            else if (Clock.CurrentState != ClockState.Stopped) Clock.Controller.Resume();
            else
            {
                if (Source == null) Source = backupSource;
                else
                {
                    Clock.Controller.Begin();
                    Clock.Controller.Resume();

                    UpdatePosition();
                }
            }

            CheckTarget();
        }

        private void CheckTarget()
        {
            if (Position < target) return;

            if (Mode == PlayTypeState.Highlights &&
                Position >= GetLastTarget()) MediaEnded?.Invoke(this, Source);
            else UpdatePosition();
        }

        public void Stop()
        {
            backupSource = Source;
            Source = null;
        }

        private void Subscribe(Timeline timeline)
        {
            if (timeline == null) return;

            timeline.CurrentTimeInvalidated += Timeline_CurrentTimeInvalidated;
            timeline.CurrentStateInvalidated += Timeline_CurrentStateInvalidated;
            timeline.Completed += Timeline_Completed;
        }

        private void Unsubscribe(Timeline timeline)
        {
            if (timeline == null) return;

            timeline.CurrentTimeInvalidated -= Timeline_CurrentTimeInvalidated;
            timeline.CurrentStateInvalidated -= Timeline_CurrentStateInvalidated;
            timeline.Completed -= Timeline_Completed;
        }

        private void Subscribe(HighlightCollection collection)
        {
            if (collection == null) return;

            foreach (Highlight highlight in collection)
            {
                Subscribe(highlight);
            }

            collection.CollectionChanged += Highlights_CollectionChanged;
        }

        private void Unsubscribe(HighlightCollection collection)
        {
            if (collection == null) return;

            foreach (Highlight highlight in collection)
            {
                Unsubscribe(highlight);
            }

            collection.CollectionChanged -= Highlights_CollectionChanged;
        }

        private void Subscribe(Highlight highlight)
        {
            highlight.BeginChanged += Highlight_Changed;
            highlight.EndChanged += Highlight_Changed;
        }

        private void Unsubscribe(Highlight highlight)
        {
            highlight.BeginChanged -= Highlight_Changed;
            highlight.EndChanged -= Highlight_Changed;
        }

        private void Highlight_Changed(Highlight sender, EventArgs args)
        {
            UpdatePosition();
        }

        private void Highlights_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Highlight highlight in e.NewItems ?? (IList)Enumerable.Empty<Highlights>())
            {
                Subscribe(highlight);
            }

            foreach (Highlight highlight in e.OldItems ?? (IList)Enumerable.Empty<Highlights>())
            {
                Subscribe(highlight);
            }

            UpdatePosition();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
