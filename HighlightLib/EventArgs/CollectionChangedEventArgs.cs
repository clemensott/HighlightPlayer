using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlightLib
{
    public enum CollectionChangeAction { Add, Remove, Change, Clear }

    public class CollectionChangedEventArgs : EventArgs
    {
        public int Index { get; private set; }

        public Highlight OldItem { get; private set; }

        public Highlight NewItem { get; private set; }

        public Highlight[] RemovedItems { get; private set; }

        public CollectionChangeAction Action { get; private set; }

        public CollectionChangedEventArgs(Highlight newItem, int index)
        {
            Index = index;
            OldItem = null;
            NewItem = newItem;
            RemovedItems = new Highlight[0];
            Action = CollectionChangeAction.Change;
        }

        public CollectionChangedEventArgs(int index, Highlight oldItem)
        {
            Index = index;
            OldItem = oldItem;
            NewItem = null;
            RemovedItems = new Highlight[] { oldItem };
            Action = CollectionChangeAction.Remove;
        }

        public CollectionChangedEventArgs(int index, Highlight oldItem, Highlight newItem)
        {
            Index = index;
            OldItem = oldItem;
            NewItem = newItem;
            RemovedItems = new Highlight[0];
            Action = CollectionChangeAction.Change;
        }

        public CollectionChangedEventArgs(IEnumerable<Highlight> highlights)
        {
            Index = -1;
            OldItem = null;
            NewItem = null;
            RemovedItems = highlights.ToArray();
            Action = CollectionChangeAction.Clear;
        }
    }
}
