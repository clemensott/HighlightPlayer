using System;
using System.Collections.Generic;
using HighlightLib;

namespace HighlightPlayer
{
    class ShuffleCompleteMediaFileCollection : MediaFileCollection
    {
        private bool wentForward;
        private MediaFile lastMedia;
        private Random random;

        public ShuffleCompleteMediaFileCollection(IEnumerable<MediaFile> mediaFiles) : base(mediaFiles)
        {
            random = new Random();

            wentForward = true;
            if (Count > 0) lastMedia = this[random.Next(Count)];
        }

        protected override void ClearItems()
        {
            lastMedia = null;
        }

        protected override void RemoveItem(int index)
        {
            if (this[index] == lastMedia) lastMedia = null;
        }

        protected override void SetItem(int index, MediaFile item)
        {
            if (this[index] == lastMedia) lastMedia = null;
        }

        protected override IEnumerable<MediaFile> GetMediasBefore(MediaFile startFile)
        {
            if (wentForward) yield return lastMedia;

            while (true)
            {
                yield return this[random.Next(Count)];
            }
        }

        protected override IEnumerable<MediaFile> GetMediasAfter(MediaFile startFile)
        {
            if (!wentForward) yield return lastMedia;

            while (true)
            {
                yield return this[random.Next(Count)];
            }
        }

        public override MediaFile GetNextMedia(MediaFile currentMediaFile)
        {
            MediaFile newMedia = base.GetNextMedia(currentMediaFile);
            lastMedia = currentMediaFile;
            wentForward = true;

            return newMedia;
        }

        public override void GetNextHighlight(TimeSpan? currentTimeSpan,
            ref MediaFile currentMediaFile, out Highlight currentHighlight)
        {
            MediaFile lastCurrentMedia = currentMediaFile;
            base.GetNextHighlight(currentTimeSpan, ref currentMediaFile, out currentHighlight);

            if (currentMediaFile != lastCurrentMedia)
            {
                lastMedia = currentMediaFile;
                wentForward = true;
            }
        }

        public override void GetPreviousHighlight(TimeSpan? currentTimeSpan,
            ref MediaFile currentMediaFile, out Highlight currentHighlight)
        {
            MediaFile lastCurrentMedia = currentMediaFile;
            base.GetPreviousHighlight(currentTimeSpan, ref currentMediaFile, out currentHighlight);

            if (currentMediaFile != lastCurrentMedia)
            {
                lastMedia = currentMediaFile;
                wentForward = false;
            }
        }

        public override MediaFile GetPreviousMedia(MediaFile currentMediaFile)
        {
            MediaFile newMedia = base.GetPreviousMedia(currentMediaFile);
            lastMedia = currentMediaFile;
            wentForward = false;

            return newMedia;
        }

        public override void GetPreviousMediaAndFirstHighlight(ref MediaFile currentMediaFile, out Highlight currentHighlight)
        {
            MediaFile lastCurrentMedia = currentMediaFile;
            base.GetPreviousMediaAndFirstHighlight(ref currentMediaFile, out currentHighlight);

            if (currentMediaFile != lastCurrentMedia)
            {
                lastMedia = currentMediaFile;
                wentForward = false;
            }
        }

        public override bool IsLast(MediaFile mediaFile)
        {
            return false;
        }
    }
}
