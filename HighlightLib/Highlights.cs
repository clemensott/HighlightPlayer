using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HighlightLib
{
    public class Highlights
    {
        public long DurationTicks { get; set; }

        public int FirstID { get; set; }

        public int LastID { get; set; }

        public long LastChangedTicks
        {
            get { return LastChanged.Ticks; }
            set { LastChanged = new DateTime(value); }
        }

        [XmlIgnore]
        public DateTime LastChanged { get; private set; }

        public HighlightCollection Collection { get; private set; }

        public Highlights()
        {
            DurationTicks = 0;
            FirstID = LastID = -1;

            Collection = new HighlightCollection();
            Collection.CollectionChanged += Collection_CollectionChanged;
        }

        public Highlights(MediaFile mediaFile)
        {
            //FirstID = mediaFile.FirstID;
            //LastID = mediaFile.CurrentID;
            DurationTicks = mediaFile.Duration.Ticks;
            LastChanged = DateTime.Now;

            Collection = new HighlightCollection();
            Collection.CollectionChanged += Collection_CollectionChanged;
        }

        private void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Highlight highlight in e.NewItems ?? (IList)Enumerable.Empty<Highlights>())
            {
                Subscribe(highlight);
            }

            foreach (Highlight highlight in e.OldItems ?? (IList)Enumerable.Empty<Highlights>())
            {
                Subscribe(highlight);
            }
            
            LastChanged = DateTime.Now;
        }

        private void Subscribe(Highlight highlight)
        {
            highlight.BeginChanged += Highlight_Changed;
            highlight.EndChanged += Highlight_Changed;
            highlight.RatingChanged += Highlight_Changed;
            highlight.CommentChanged += Highlight_Changed;
        }

        private void Unsubscribe(Highlight highlight)
        {
            highlight.BeginChanged -= Highlight_Changed;
            highlight.EndChanged -= Highlight_Changed;
            highlight.RatingChanged -= Highlight_Changed;
            highlight.CommentChanged -= Highlight_Changed;
        }

        private void Highlight_Changed(Highlight sender, EventArgs args)
        {
            LastChanged = DateTime.Now;
        }

        internal void Change(Highlights newHighlights)
        {
            DurationTicks = newHighlights.DurationTicks;
            FirstID = newHighlights.FirstID;
            LastID = newHighlights.LastID;
            LastChanged = newHighlights.LastChanged;

            Collection.Clear();
            foreach (Highlight highlight in newHighlights.Collection)
            {
                Collection.Add(highlight);
            }
        }
    }
}
