using System;
using System.Collections.Generic;
using System.Linq;
using HighlightLib;

namespace HighlightPlayer
{
    class ShuffleOneMediaFileCollection : MediaFileCollection
    {
        private List<MediaFile> shuffleList;
        private Random random;

        public ShuffleOneMediaFileCollection(IEnumerable<MediaFile> mediaFiles) : base(mediaFiles)
        {
            List<MediaFile> remainingFiles = mediaFiles.ToList();
            shuffleList = new List<MediaFile>();
            random = new Random();

            while (remainingFiles.Count > 0)
            {
                int index = random.Next(remainingFiles.Count);

                shuffleList.Add(remainingFiles[index]);
                remainingFiles.RemoveAt(index);
            }
        }

        protected override void ClearItems()
        {
            shuffleList.Clear();
        }

        protected override void InsertItem(int index, MediaFile item)
        {
            int shuffleListIndex = random.Next(Count);
            shuffleList.Insert(shuffleListIndex, item);
        }

        protected override void RemoveItem(int index)
        {
            shuffleList.Remove(this[index]);
        }

        protected override void SetItem(int index, MediaFile item)
        {
            int shuffleListIndex = shuffleList.IndexOf(this[index]);

            shuffleList[shuffleListIndex] = item;
        }

        protected override IEnumerable<MediaFile> GetMediasBefore(MediaFile startFile)
        {
            int startIndex = shuffleList.IndexOf(startFile);

            for (int i = 1; i <= shuffleList.Count; i++)
            {
                yield return shuffleList[(startIndex - i + Count) % shuffleList.Count];
            }
        }

        protected override IEnumerable<MediaFile> GetMediasAfter(MediaFile startFile)
        {
            int startIndex = shuffleList.IndexOf(startFile);

            for (int i = 1; i <= shuffleList.Count; i++)
            {
                yield return shuffleList[(startIndex + i) % shuffleList.Count];
            }
        }

        public override bool IsLast(MediaFile mediaFile)
        {
            return shuffleList.Last() == mediaFile;
        }
    }
}
