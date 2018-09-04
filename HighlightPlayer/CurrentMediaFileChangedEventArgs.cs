using HighlightLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlightPlayer
{
    public class CurrentMediaFileChangedEventArgs : EventArgs
    {
        public MediaFile OldCurrentMediaFile { get; private set; }

        public MediaFile NewCurrentMediaFile { get; private set; }

        public CurrentMediaFileChangedEventArgs(MediaFile oldItem, MediaFile newItem)
        {
            OldCurrentMediaFile = oldItem;
            NewCurrentMediaFile = newItem;
        }
    }
}
