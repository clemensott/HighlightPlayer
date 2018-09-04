using HighlightLib;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Media;

namespace HighlightPlayer.Controls
{
    class MediaPositionSliderHighlightElementsList
    {
        private MediaPositionSlider parent;
        private Dictionary<Highlight, MediaPositionSliderHighlightElement> begins;
        private Dictionary<Highlight, MediaPositionSliderEndHighlightElement> ends;

        public Dictionary<Highlight, MediaPositionSliderHighlightElement> Begins
        {
            get { return begins; }
        }

        public Dictionary<Highlight, MediaPositionSliderEndHighlightElement> Ends
        {
            get { return ends; }
        }

        public MediaPositionSliderHighlightElementsList(MediaPositionSlider parent, HighlightCollection highlights)
        {
            this.parent = parent;

            begins = new Dictionary<Highlight, MediaPositionSliderHighlightElement>();
            ends = new Dictionary<Highlight, MediaPositionSliderEndHighlightElement>();

            SetDictionaries(highlights);

            highlights.CollectionChanged += Highlights_CollectionChanged;
        }

        private void Highlights_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            SetDictionaries(sender as HighlightCollection);

            parent.InvalidateVisual();
        }

        private void SetDictionaries(HighlightCollection highlights)
        {
            begins.Clear();
            ends.Clear();

            foreach (Highlight highlight in highlights)
            {
                begins.Add(highlight, new MediaPositionSliderHighlightElement(parent, highlight, Brushes.Gray, 2));
                ends.Add(highlight, new MediaPositionSliderEndHighlightElement(parent, highlight, Brushes.Gray, 2));
            }
        }
    }
}
