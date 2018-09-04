using HighlightLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace HighlightPlayer.Controls
{
    public class CurrentHighlights : IEnumerable<Highlight>, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private TimeSpan position;
        private HighlightCollection highlights;
        private ObservableCollection<Highlight> current;

        public TimeSpan Position
        {
            get { return position; }
            set
            {
                if (value == position) return;

                position = value;
                Update();
            }
        }

        public CurrentHighlights()
        {
            position = new TimeSpan();
            current = new ObservableCollection<Highlight>();
            current.CollectionChanged += Current_CollectionChanged;
        }

        public CurrentHighlights(HighlightCollection collection, TimeSpan position) : this()
        {
            highlights = collection;
            highlights.CollectionChanged += Highlights_CollectionChanged;

            foreach (Highlight highlight in collection) Subscribe(highlight);

            this.position = position;
            Update();
        }

        private void Current_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
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
            SetHighlight(sender);
        }

        private void Update()
        {
            foreach (Highlight highlight in highlights ?? Enumerable.Empty<Highlight>())
            {
                SetHighlight(highlight);
            }
        }

        private void SetHighlight(Highlight highlight)
        {
            if (highlight.IsIn(Position))
            {
                if (!current.Contains(highlight)) current.Add(highlight);
            }
            else if (current.Contains(highlight)) current.Remove(highlight);
        }

        public IEnumerator<Highlight> GetEnumerator()
        {
            return current.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return current.GetEnumerator();
        }
    }
}
