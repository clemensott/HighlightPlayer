using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HighlightLib
{
    public class HighlightCollection : ObservableCollection<Highlight>
    {
        public HighlightCollection()
        {
        }

        public HighlightCollection(IEnumerable<Highlight> collection)
        {
            foreach(Highlight highlight in collection)
            {
                Add(highlight);
            }
        }

        protected override void ClearItems()
        {
            base.ClearItems();

            foreach (Highlight highlight in this)
            {
                Unsubscribe(highlight);
            }
        }

        protected override void InsertItem(int index, Highlight item)
        {
            if ((index != 0 && this[index - 1].CompareTo(item) > 0) || (index < Count && this[index].CompareTo(item) < 0))
            {
                for (index = 0; index < Count; index++)
                {
                    if (this[index].CompareTo(item) >= 0) break;
                }
            }

            base.InsertItem(index, item);

            Subscribe(item);

            CheckOrder();
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            if (oldIndex < newIndex ^ this[oldIndex].CompareTo(this[newIndex]) < 0) base.MoveItem(oldIndex, newIndex);

            CheckOrder();
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
            CheckOrder();
        }

        private void CheckOrder()
        {
            Highlight[] ordered = this.OrderBy(h => h.Begin).ThenBy(h => h.End).ToArray();

            for (int i = 0; i < ordered.Length; i++)
            {
                int currentIndex = IndexOf(ordered[i]);

                if (i != currentIndex) Move(currentIndex, i);
            }
        }

        public IEnumerable<Highlight> OrderByBegin()
        {
            return this.OrderBy(h => h.Begin);
        }

        public IEnumerable<Highlight> OrderByEnd()
        {
            return this.OrderBy(h => h.End);
        }
    }
}
