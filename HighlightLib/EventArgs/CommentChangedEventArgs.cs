using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlightLib
{
    public class CommentChangedEventArgs : EventArgs
    {
        public string OldValue { get; private set; }

        public string NewValue { get; private set; }

        public CommentChangedEventArgs(string oldValue, string newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
