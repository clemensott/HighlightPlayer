using ID3TagLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HighlightLib
{
    public delegate void DurationLoadedEventHandler(MediaFile sender, TimeSpan duration);

    public class MediaFile : INotifyPropertyChanged
    {
        private const char highlightSplitter = '|', addChar = '&';
        private const string ffmpegPath = "ffmpeg.exe", cmdFormat = "-i \"{0}\"",
           durationStartKey = "Duration: ", durationEndKey = ", start", highlightsKey = "<Highlight";

        private static readonly TimeSpan nonloadedDuration = new TimeSpan(-1);

        public event DurationLoadedEventHandler DurationLoaded;

        private TimeSpan duration;
        private object loadDurationLockObject = new object();
        private HighlightCollection highlights;

        public bool HasChanges { get; private set; }

        public string FullPath { get; private set; }

        //public int FirstID { get; private set; }

        //public int CurrentID { get; private set; }

        public TimeSpan Duration
        {
            get { return duration; }
            private set
            {
                if (value == duration) return;

                duration = value;
                OnPropertyChanged("Duration");
                DurationLoaded?.Invoke(this, duration);
            }
        }

        public HighlightCollection Highlights
        {
            get { return highlights; }
            private set
            {
                if (value == highlights) return;

                highlights = value;
                OnPropertyChanged("Highlights");
            }
        }


        public MediaFile(string path)
        {
            FullPath = path;
            //CurrentID = Path.GetFileName(FullPath).GetHashCode();
            //FirstID = GetFirstIDFromID3Tag();
            Highlights = null;

            duration = nonloadedDuration;

            SetHighlights();
        }

        //private int GetFirstIDFromID3Tag()
        //{
        //    int firstID;
        //    ID3File id3File = new ID3File(FullPath);

        //    if (id3File.ID3v2Tag == null) id3File.ID3v2Tag = new ID3v2Tag();

        //    TextFrame frame = FrameFactory.GetFrame(FrameFactory.UserDefinedTextFrameId) as TextFrame;
        //    frame.Text = "Test";
        //    id3File.ID3v2Tag.Frames.Add(frame);

        //    if (int.TryParse(id3File.ID3v1Tag?.Comment ?? "", out firstID)) return firstID;
        //    if (id3File.ID3v1Tag == null) id3File.ID3v1Tag = new ID3v1Tag();

        //    id3File.ID3v1Tag.Comment = CurrentID.ToString();
        //    id3File.Save(FullPath);

        //    return CurrentID;
        //}

        public void LoadDuration()
        {
            lock (loadDurationLockObject)
            {
                if (duration != nonloadedDuration) return;

                try
                {
                    string mediaFileInfo = GetMediaFileInfo(FullPath);
                    int durationStartIndex = mediaFileInfo.IndexOf(durationStartKey) + durationStartKey.Length;
                    int durationEndIndex = mediaFileInfo.IndexOf(durationEndKey);

                    string durationText = mediaFileInfo.Remove(durationEndIndex).Remove(0, durationStartIndex);

                    Duration = TimeSpan.Parse(durationText);
                }
                catch
                {
                    Duration = TimeSpan.Zero;
                }
            }
        }

        private string GetMediaFileInfo(string path)
        {
            string cmd = string.Format(cmdFormat, path);
            ProcessStartInfo info = new ProcessStartInfo(ffmpegPath, cmd)
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(info))
            {
                using (StreamReader reader = process.StandardError)
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void SetHighlights()
        {
            //lock (loadDurationLockObj)
            //{
            //    if (duration == nonloadedDuration) duration = GetDuration();
            //}

            //Highlights = collections.GetHighlightCollection(this);

            try
            {
                TextFrame frame;
                ID3File id3File = new ID3File(FullPath);

                Highlights = TryFindHighlightsFrame(id3File?.ID3v2Tag?.Frames, out frame) ?
                    DeserializeHighlights(frame.Text) : new HighlightCollection();

                Highlights.CollectionChanged += Highlights_CollectionChanged;

                foreach (Highlight highlight in Highlights)
                {
                    highlight.BeginChanged += Highlight_BeginChanged;
                    highlight.EndChanged += Highlight_EndChanged;
                }
            }
            catch
            {
                Highlights = new HighlightCollection();
            }
        }

        #region ID3v2
        private void Highlights_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (Highlight newHighlights in args.NewItems)
                {
                    newHighlights.BeginChanged += Highlight_BeginChanged;
                    newHighlights.EndChanged += Highlight_EndChanged;
                }
            }

            if (args.OldItems != null)
            {
                foreach (Highlight oldHighlights in args.OldItems)
                {
                    oldHighlights.BeginChanged -= Highlight_BeginChanged;
                    oldHighlights.EndChanged -= Highlight_EndChanged;
                }
            }

            SetHasChanges();
        }

        private void Highlight_BeginChanged(Highlight sender, TimeChangedEventArgs args)
        {
            SetHasChanges();
        }

        private void Highlight_EndChanged(Highlight sender, TimeChangedEventArgs args)
        {
            SetHasChanges();
        }

        private void SetHasChanges()
        {
            HasChanges = true;
            SaveHighlights();
        }

        public void SaveHighlights()
        {
            try
            {
                TextFrame frame;
                ID3File id3File = new ID3File(FullPath);

                if (id3File.ID3v2Tag == null) id3File.ID3v2Tag = new ID3v2Tag();

                if (!TryFindHighlightsFrame(id3File.ID3v2Tag.Frames, out frame))
                {
                    frame = FrameFactory.GetFrame(FrameFactory.UserDefinedTextFrameId) as TextFrame;
                    id3File.ID3v2Tag.Frames.Add(frame);
                }

                if (Highlights.Count > 0) frame.Text = SerializeHighlights(Highlights);
                else id3File.ID3v2Tag.Frames.Remove(frame);

                id3File.Save(FullPath);

                HasChanges = false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private bool TryFindHighlightsFrame(FrameCollection frames, out TextFrame highlightFrame)
        {
            foreach (TextFrame textFrame in frames?.OfType<TextFrame>() ?? Enumerable.Empty<TextFrame>())
            {
                highlightFrame = textFrame;

                if (textFrame.FrameId == "TXXX") return true;
                if (textFrame.Text.StartsWith(highlightsKey)) return true;
            }

            highlightFrame = null;
            return false;
        }

        private string SerializeHighlights(HighlightCollection highlights)
        {
            string dataString = string.Empty;
            string splitterString = highlightSplitter.ToString();
            string normalSplitterString = highlightSplitter.ToString() + addChar.ToString();

            foreach (Highlight highlight in highlights)
            {
                dataString += highlight.GetDataString().Replace(splitterString, normalSplitterString) + splitterString;
            }

            return dataString;
        }

        private HighlightCollection DeserializeHighlights(string dataString)
        {
            return new HighlightCollection(GetHighlights(dataString));
        }

        public static IEnumerable<Highlight> GetHighlights(string highlightsDataString)
        {
            var array = Split(highlightsDataString, highlightSplitter).ToArray();
            string splitterString = highlightSplitter.ToString();
            string normalSplitterString = highlightSplitter.ToString() + addChar.ToString();

            foreach (string highlightDataString in array)
            {
                Highlight highlight = null;

                try
                {
                    highlight = new Highlight(highlightDataString.Replace(normalSplitterString, splitterString));
                }
                catch { }

                if (highlight != null) yield return highlight;
            }
        }

        private static IEnumerable<string> Split(string dataString, char seperator)
        {
            while (dataString.Length > 0)
            {
                yield return GetUntil(ref dataString, seperator);
            }
        }

        private static string GetUntil(ref string text, char seperator)
        {
            int lenght = 0;
            string part = string.Empty;

            while (true)
            {
                char c = text.ElementAtOrDefault(lenght);
                if (c == seperator)
                {
                    lenght++;

                    if (text.ElementAtOrDefault(lenght) != addChar) break;
                }

                part += c;
                lenght++;

                if (lenght >= text.Length) break;
            }

            text = text.Remove(0, lenght);

            return part;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString()
        {
            return FullPath;
        }
    }
}
