using HighlightLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace HighlightPlayer
{
    interface IMediaFileCollection : IEnumerable<MediaFile>, INotifyCollectionChanged
    {
        bool IsLast(MediaFile mediaFile);

        MediaFile GetNextMedia(MediaFile currentMediaFile);

        void GetNextHighlight(TimeSpan? currentTimeSpan,
            ref MediaFile currentMediaFile, out Highlight currentHighlight);

        MediaFile GetPreviousMedia(MediaFile currentMediaFile);

        void GetPreviousMediaAndFirstHighlight(ref MediaFile currentMediaFile, out Highlight currentHighlight);

        void GetPreviousHighlight(TimeSpan? currentTimeSpan,
            ref MediaFile currentMediaFile, out Highlight currentHighlight);
    }
}
