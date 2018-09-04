using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace HighlightLib
{
    public delegate void HighlightRatingChangedHandler(Highlight sender, RatingChangedEventArgs args);
    public delegate void HighlightBeginChangedHandler(Highlight sender, TimeChangedEventArgs args);
    public delegate void HighlightEndChangedHandler(Highlight sender, TimeChangedEventArgs args);
    public delegate void HighlightCommentChangedHandler(Highlight sender, CommentChangedEventArgs args);

    public class Highlight : IComparable<Highlight>
    {
        private const char dataStringSplitter = ';', addChar = '&';

        public event HighlightRatingChangedHandler RatingChanged;
        public event HighlightBeginChangedHandler BeginChanged;
        public event HighlightEndChangedHandler EndChanged;
        public event HighlightCommentChangedHandler CommentChanged;

        private double rating;
        private TimeSpan begin, end;
        private string comment;

        public double Rating
        {
            get { return rating; }
            set
            {
                if (value == rating) return;

                RatingChangedEventArgs args = new RatingChangedEventArgs(rating, value);
                rating = value;
                RatingChanged?.Invoke(this, args);
            }
        }

        public long BeginTicks
        {
            get { return Begin.Ticks; }
            set { Begin = new TimeSpan(value); }
        }

        [XmlIgnore]
        public TimeSpan Begin
        {
            get { return begin; }
            set
            {
                if (value == begin) return;

                TimeChangedEventArgs args = new TimeChangedEventArgs(begin, value);
                begin = value;
                BeginChanged?.Invoke(this, args);
            }
        }

        public long EndTicks
        {
            get { return End.Ticks; }
            set { End = new TimeSpan(value); }
        }

        [XmlIgnore]
        public TimeSpan End
        {
            get { return end; }
            set
            {
                if (value == end || value < Begin) return;

                TimeChangedEventArgs args = new TimeChangedEventArgs(begin, value);
                end = value;
                EndChanged?.Invoke(this, args);
            }
        }

        public string Comment
        {
            get { return comment; }
            set
            {
                if (value == comment) return;

                CommentChangedEventArgs args = new CommentChangedEventArgs(comment, value);
                comment = value;
                CommentChanged?.Invoke(this, args);
            }
        }

        public Highlight()
        {
            begin = end = new TimeSpan();
            rating = 0;
            comment = string.Empty;
        }

        public Highlight(TimeSpan begin) : this()
        {
            this.begin = this.end = begin;
        }

        public Highlight(TimeSpan begin, TimeSpan end) : this()
        {
            this.begin = begin;
            this.end = end;
        }

        internal Highlight(string dataString)
        {
            int i = 0;

            begin = ParseTimeSpan(dataString, ref i, "Begin");
            end = ParseTimeSpan(dataString, ref i, "End");
            rating = ParseDouble(dataString, ref i, "Rating");
            comment = dataString.Remove(0, i);
        }

        internal Highlight(ref byte[] bytes)
        {
            Rating = bytes.First();
            End = TimeSpan.FromSeconds(BitConverter.ToDouble(bytes, 9));
            Begin = TimeSpan.FromSeconds(BitConverter.ToDouble(bytes, 1));

            int startIndex = 1 + 8 + 8 + 4;
            int length = startIndex + BitConverter.ToInt32(bytes, 17);

            for (int i = startIndex; i < length; i++)
            {
                Comment += BitConverter.ToChar(bytes, i);
            }

            bytes = bytes.Skip(length).ToArray();
        }

        private double ParseDouble(string dataString, ref int i, string propertyName)
        {
            try
            {
                for (string tmp = string.Empty; true; tmp += dataString[i], i++)
                {
                    if (dataString[i] != dataStringSplitter) continue;

                    i++;
                    return double.Parse(tmp);
                }
            }
            catch (Exception e)
            {
                string message = "Exception while reading the " + propertyName + " of the highlight.";

                throw new Exception(message, e);
            }
        }

        private TimeSpan ParseTimeSpan(string dataString, ref int i, string propertyName)
        {
            try
            {
                for (string tmp = string.Empty; true; tmp += dataString[i], i++)
                {
                    if (dataString[i] != dataStringSplitter) continue;

                    i++;
                    return TimeSpan.Parse(tmp);
                }
            }
            catch (Exception e)
            {
                string message = "Exception while reading the " + propertyName + " of the highlight.";

                throw new Exception(message, e);
            }
        }

        internal string GetDataString()
        {
            return GetDataString(Begin.Ticks, End.Ticks, Rating, Comment);
        }

        private string GetDataString(params object[] objs)
        {
            string text = string.Empty;
            string splitterString = dataStringSplitter.ToString();
            string normalSplitterString = dataStringSplitter.ToString() + addChar.ToString();

            foreach (object obj in objs)
            {
                text += obj.ToString().Replace(splitterString, normalSplitterString) + splitterString;
            }

            text = text.Remove(text.Length - 1);

            return text;
        }

        public int CompareTo(Highlight other)
        {
            if (Begin < other.Begin) return -1;
            if (Begin > other.Begin) return 1;

            if (End == other.End) return 0;

            return End < other.End ? -1 : 1;
        }
    }
}
