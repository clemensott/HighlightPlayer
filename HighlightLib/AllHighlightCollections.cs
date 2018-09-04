using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;

namespace HighlightLib
{
    public class AllHighlightCollections
    {
        private const string defaultSavePath = "AllHighlights.xml";

        private XmlSerializer xmlSerializer;
        private List<string> savePaths;

        private ObservableCollection<Highlights> allHighlights;

        public AllHighlightCollections()
        {
            xmlSerializer = new XmlSerializer(typeof(List<Highlights>));
            savePaths = new List<string>();

            allHighlights = new ObservableCollection<Highlights>();
            allHighlights.CollectionChanged += OnCollectionsChanged;

            AddHighlightsFile(defaultSavePath);
        }

        public void AddHighlightsFile(string path)
        {
            savePaths.Add(path);

            if (!File.Exists(path)) return;

            try
            {
                using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
                {
                    List<Highlights> list = (List<Highlights>)xmlSerializer.Deserialize(stream);

                    foreach (Highlights newHighlights in list)
                    {
                        Highlights simalarHighlights = allHighlights.FirstOrDefault(c => Similar(c, newHighlights));

                        if (simalarHighlights == null) allHighlights.Add(newHighlights);
                        else if (simalarHighlights.LastChanged < newHighlights.LastChanged)
                        {
                            simalarHighlights.Change(newHighlights);
                        }
                    }
                }
            }
            catch { }
        }

        private bool Similar(Highlights collection1, Highlights collection2)
        {
            if (collection1.DurationTicks != collection2.DurationTicks) return false;
            if (collection1.FirstID == collection2.FirstID) return true;
            if (collection1.LastID == collection2.LastID) return true;

            return false;
        }

        private void OnCollectionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Highlights highlights in e.NewItems ?? (IList)Enumerable.Empty<Highlights>())
            {
                highlights.Collection.CollectionChanged += CollectionChanged;
                Subscribe(highlights.Collection);
            }

            foreach (Highlights highlights in e.OldItems ?? (IList)Enumerable.Empty<Highlights>())
            {
                Unsubscribe(highlights.Collection);
            }

            SaveHighlights();
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Highlight highlight in e.NewItems ?? (IList)Enumerable.Empty<Highlights>())
            {
                Subscribe(highlight);
            }

            foreach (Highlight highlight in e.OldItems ?? (IList)Enumerable.Empty<Highlights>())
            {
                Subscribe(highlight);
            }

            SaveHighlights();
        }

        private void Subscribe(HighlightCollection collection)
        {
            collection.CollectionChanged += Highlights_CollectionChanged;

            foreach (Highlight highlight in collection)
            {
                Subscribe(highlight);
            }
        }

        private void Unsubscribe(HighlightCollection collection)
        {
            collection.CollectionChanged -= Highlights_CollectionChanged;

            foreach (Highlight highlight in collection)
            {
                Unsubscribe(highlight);
            }
        }

        private void Highlights_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (Highlight newHighlights in args.NewItems)
                {
                    Subscribe(newHighlights);
                }
            }

            if (args.OldItems != null)
            {
                foreach (Highlight oldHighlights in args.OldItems)
                {
                    Unsubscribe(oldHighlights);
                }
            }

            SaveHighlights();
        }

        private void Subscribe(Highlight highlight)
        {
            highlight.BeginChanged += Highlight_Changed;
            highlight.EndChanged += Highlight_Changed;
            highlight.RatingChanged += Highlight_Changed;
            highlight.CommentChanged += Highlight_Changed;
        }

        private void Unsubscribe(Highlight highlight)
        {
            highlight.BeginChanged -= Highlight_Changed;
            highlight.EndChanged -= Highlight_Changed;
            highlight.RatingChanged -= Highlight_Changed;
            highlight.CommentChanged -= Highlight_Changed;
        }

        private void Highlight_Changed(Highlight sender, EventArgs args)
        {
            SaveHighlights();
        }

        public HighlightCollection GetHighlightCollection(MediaFile mediaFile)
        {
            Highlights highlights = allHighlights.FirstOrDefault(c => c.DurationTicks == mediaFile.Duration.Ticks);

            if (highlights == null)
            {
                highlights = new Highlights(mediaFile);
                allHighlights.Add(highlights);
            }

            return highlights.Collection;
        }

        private void Highlight_BeginChanged(Highlight sender, TimeChangedEventArgs args)
        {
            SaveHighlights();
        }

        private void Highlight_EndChanged(Highlight sender, TimeChangedEventArgs args)
        {
            SaveHighlights();
        }

        public void SaveHighlights()
        {
            List<Highlights> nonEmptyHighlights = allHighlights.Where(c => c.Collection.Count > 0).ToList();

            foreach (string path in GetSavePaths())
            {
                try
                {
                    if (nonEmptyHighlights.Count == 0) File.Delete(path);
                    else
                    {
                        using (Stream stream = new FileStream(path, FileMode.OpenOrCreate))
                        {
                            xmlSerializer.Serialize(stream, nonEmptyHighlights);
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }

        private IEnumerable<string> GetSavePaths()
        {
            return savePaths;
        }
    }
}
