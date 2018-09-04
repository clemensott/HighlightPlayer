using System;

namespace HighlightPlayer.Controls
{
    public class MediaPositionChangedEventArgs : EventArgs
    {
        public double OldPositionFactor { get; private set; }

        public double NewPositionFactor { get; private set; }

        public TimeSpan OldPosition { get; private set; }

        public TimeSpan NewPosition { get; private set; }

        public TimeSpan Duration { get; private set; }

        public MediaPositionChangedEventArgs(TimeSpan oldPosition, TimeSpan newPosition,
            double oldFactor, double newFactor, TimeSpan duration)
        {
            OldPositionFactor = oldFactor;
            NewPositionFactor = newFactor;

            OldPosition = oldPosition;
            NewPosition = newPosition;
            Duration = duration;
        }
    }
}
