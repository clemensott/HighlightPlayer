using HighlightLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HighlightPlayer.Controls
{
    public class CurrentHighlightsChangedEventArgs : EventArgs
    {
        public TimeSpan CurrentPosition { get; private set; }

        public Highlight[] OldCurrentHighlights { get; private set; }

        public Highlight[] NewCurrentHighlights { get; private set; }

        public Highlight[] AddedHighlights { get; private set; }

        public Highlight[] RemovedHighlights { get; private set; }

        public CurrentHighlightsChangedEventArgs(IEnumerable<Highlight> oldCurrentHighlights, 
            IEnumerable<Highlight> newCurrentHighlights, TimeSpan currentPosition)
        {
            CurrentPosition = currentPosition;
            OldCurrentHighlights = oldCurrentHighlights.ToArray();
            NewCurrentHighlights = newCurrentHighlights.ToArray();

            AddedHighlights = NewCurrentHighlights.Except(OldCurrentHighlights).ToArray();
            RemovedHighlights = OldCurrentHighlights.Except(NewCurrentHighlights).ToArray();
        }
    }
}