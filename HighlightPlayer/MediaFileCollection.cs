using System;
using System.Collections.Generic;
using System.Linq;
using HighlightLib;
using System.Collections.ObjectModel;

namespace HighlightPlayer
{
    public class MediaFileCollection : ObservableCollection<MediaFile>, IMediaFileCollection
    {
        public MediaFileCollection()
        {
        }

        public MediaFileCollection(IEnumerable<MediaFile> mediaFiles) : base(mediaFiles)
        {
        }

        protected virtual IEnumerable<MediaFile> GetMediasBefore(MediaFile startFile)
        {
            int startIndex = IndexOf(startFile);

            for (int i = 1; i <= Count; i++)
            {
                yield return this[(startIndex - i + Count) % Count];
            }
        }

        protected virtual IEnumerable<MediaFile> GetMediasAfter(MediaFile startFile)
        {
            int startIndex = IndexOf(startFile);

            for (int i = 1; i <= Count; i++)
            {
                yield return this[(startIndex + i) % Count];
            }
        }

        public virtual void GetNextHighlight(TimeSpan? currentTimeSpan,
            ref MediaFile currentMediaFile, out Highlight currentHighlight)
        {
            if (currentTimeSpan == null) currentTimeSpan = new TimeSpan();

            currentHighlight = null;
            if (currentMediaFile == null) return;

            currentHighlight = currentMediaFile.Highlights.OrderByBegin().FirstOrDefault(h => h.Begin > currentTimeSpan);
            if (currentHighlight != null) return;

            if (!HaveMediaWithHighlights()) return;
            foreach (MediaFile media in GetMediasAfter(currentMediaFile))
            {
                if (!media.Highlights.Any()) continue;

                currentMediaFile = media;
                currentHighlight = currentMediaFile.Highlights.OrderByBegin().FirstOrDefault();
                break;
            }
        }

        public virtual MediaFile GetNextMedia(MediaFile currentMediaFile)
        {
            if (currentMediaFile == null) return null;

            return GetMediasAfter(currentMediaFile).FirstOrDefault();
        }

        public virtual void GetPreviousHighlight(TimeSpan? currentTimeSpan,
            ref MediaFile currentMediaFile, out Highlight currentHighlight)
        {
            if (currentTimeSpan == null) currentTimeSpan = new TimeSpan();

            currentHighlight = null;
            if (currentMediaFile == null) return;

            currentHighlight = currentMediaFile.Highlights.OrderByBegin().LastOrDefault(h => h.Begin < currentTimeSpan);
            if (currentHighlight != null) return;

            if (!HaveMediaWithHighlights()) return;
            foreach (MediaFile media in GetMediasBefore(currentMediaFile))
            {
                if (!media.Highlights.Any()) continue;

                currentMediaFile = media;
                currentHighlight = currentMediaFile.Highlights.OrderByEnd().LastOrDefault();
                break;
            }
        }

        public virtual MediaFile GetPreviousMedia(MediaFile currentMediaFile)
        {
            if (currentMediaFile == null) return null;

            return GetMediasBefore(currentMediaFile).FirstOrDefault();
        }

        public virtual void GetPreviousMediaAndFirstHighlight(ref MediaFile currentMediaFile, out Highlight currentHighlight)
        {
            currentHighlight = null;

            if (!HaveMediaWithHighlights()) return;

            foreach(MediaFile media in GetMediasBefore(currentMediaFile))
            {
                if (!media.Highlights.Any()) continue;

                currentMediaFile = media;
                currentHighlight = currentMediaFile.Highlights.FirstOrDefault();
                return;
            }
        }

        private bool HaveMediaWithHighlights()
        {
            foreach (MediaFile media in this)
            {
                if (media.Highlights.Count > 0) return true;
            }

            return false;
        }

        public virtual bool IsLast(MediaFile mediaFile)
        {
            return this.Last() == mediaFile;
        }
    }
}
