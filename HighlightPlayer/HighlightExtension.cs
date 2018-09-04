using HighlightLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighlightPlayer
{
    static class HighlightExtension
    {
        public static bool IsHighlightClosed(this Highlight highlight)
        {
            return highlight.Begin < highlight.End;
        }

        public static bool IsIn(this Highlight highlight, TimeSpan? timeSpan)
        {
            if (timeSpan == null) return false;

            return timeSpan >= highlight.Begin && timeSpan < highlight.End;
        }
    }

    static class MediaFileExtension
    {
        public static Uri GetUri(this MediaFile mediaFile)
        {
            return new Uri(mediaFile.FullPath);
        }
    }
}
