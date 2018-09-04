using HighlightLib;
using HighlightPlayer.Controls;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HighlightPlayer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel viewModel;
        private AllHighlightCollections collections;

        public MainWindow()
        {
            InitializeComponent();

            //collections = new AllHighlightCollections();
            viewModel = new ViewModel(player);
            DataContext = viewModel;

            viewModel.ViewType = SwitchViewState.Medias;
            viewModel.ShowHighlights = ShowHighlightsState.Hide;

            viewModel.Manager.MediaEnded += Manager_MediaEnded;

            string dirPath = @"D:\Videos\Download\Tmp";
            //string dirPath = @"D:\Musik";
            foreach (string path in Directory.GetFiles(dirPath))
            {
                try
                {
                    MediaFile file = new MediaFile(path);
                    viewModel.MediaFiles.Add(file);
                }
                catch { }
            }

            viewModel.Manager.Source = viewModel.MediaFiles[0];
            viewModel.Manager.Mode = PlayTypeState.Medias;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Task(new Action(LoadDurations)).Start();
        }

        private void LoadDurations()
        {
            Parallel.ForEach(viewModel.MediaFiles, new Action<MediaFile>(LoadDuration));
            //foreach (MediaFile mediaFile in viewModel.MediaFiles) mediaFile.LoadDuration();
        }

        private void LoadDuration(MediaFile mediaFile)
        {
            mediaFile.LoadDuration();
        }

        private void BtnPreviousMedia_Click(object sender, ButtonStateChangingEventArgs args)
        {
            SetPreviousMedia();
        }

        private void BtnPreviousHighlight_Click(object sender, ButtonStateChangingEventArgs args)
        {
            SetPreviousHighlight();
        }

        private void BtnPlayPause_Click(object sender, ButtonStateChangingEventArgs args)
        {
            if (viewModel.Manager.State == PlayPauseState.Play && CurrentFileEnded()) AutoNext();
        }

        private bool CurrentFileEnded()
        {
            Duration duration = viewModel.Manager.NaturalDuration;
            TimeSpan durationTimeSpan = duration.HasTimeSpan ? duration.TimeSpan : TimeSpan.MaxValue;
            TimeSpan lastHighlightEnd = viewModel.Manager.Source?.Highlights?
                .OrderByEnd().LastOrDefault()?.End ?? TimeSpan.Zero;

            TimeSpan end = viewModel.Manager.Mode == PlayTypeState.Medias ? durationTimeSpan : lastHighlightEnd;
            return viewModel.Manager.Position >= end;
        }

        private void BtnStop_Click(object sender, ButtonStateChangingEventArgs args)
        {
            Stop();
        }

        private void BtnNextHighlight_Click(object sender, ButtonStateChangingEventArgs args)
        {
            SetNextHighlight();
        }

        private void BtnNextMedia_Click(object sender, ButtonStateChangingEventArgs args)
        {
            SetNextMediaFile();
        }

        private void BtnShuffle_Click(object sender, ButtonStateChangingEventArgs args)
        {

        }

        private void BtnLoop_Click(object sender, ButtonStateChangingEventArgs args)
        {

        }

        private void BtnSwitchView_Click(object sender, ButtonStateChangingEventArgs args)
        {
        }

        private void BtnPlayType_Click(object sender, ButtonStateChangingEventArgs args)
        {
            //manager.PlayType = args.NewValue;
        }

        private void BtnHighlight_Click(object sender, ButtonStateChangingEventArgs args)
        {
            HighlightCollection highlights = viewModel.Manager.Source.Highlights;
            Highlight firstOpenHighlight = highlights.OrderByBegin().FirstOrDefault(h => !h.IsHighlightClosed());

            if (firstOpenHighlight != null) firstOpenHighlight.End = mps.MediaPosition;
            else highlights.Add(new Highlight(mps.MediaPosition));

            args.NewValue = MediaPositionSlider.GetHighlightState(highlights, mps.MediaPosition);
        }

        private void Hme_MediaEnded(object sender, RoutedEventArgs e)
        {
            switch (viewModel.LoopType)
            {
                case LoopState.Off:
                    if (viewModel.MediaFiles.IsLast(viewModel.Manager.Source)) Stop();
                    else SetNextMediaFile();
                    break;

                case LoopState.All:
                    SetNextMediaFile();
                    break;

                case LoopState.Current:
                    viewModel.Manager.Position = TimeSpan.Zero;
                    break;
            }
        }

        private void Manager_MediaEnded(HighlightTimeLineManager sender, MediaFile file)
        {
            if (sender.State == PlayPauseState.Play) AutoNext();
        }

        private void AutoNext()
        {
            switch (viewModel.LoopType)
            {
                case LoopState.Off:
                    if (viewModel.MediaFiles.IsLast(viewModel.Manager.Source)) Stop();
                    else SetNextMediaFile();
                    break;

                case LoopState.All:
                    SetNextMediaFile();
                    break;

                case LoopState.Current:
                    viewModel.Manager.Position = TimeSpan.Zero;
                    break;
            }
        }

        private void SetNextMediaFile()
        {
            if (viewModel.Manager.Mode == PlayTypeState.Highlights)
            {
                Highlight highlight;
                MediaFile mediaFile = viewModel.Manager.Source;

                viewModel.MediaFiles.GetNextHighlight(TimeSpan.MaxValue, ref mediaFile, out highlight);

                viewModel.Manager.Source = mediaFile;
            }
            else viewModel.Manager.Source = viewModel.MediaFiles.GetNextMedia(viewModel.Manager.Source);
        }

        private void SetNextHighlight()
        {
            Highlight highlight;
            MediaFile mediaFile = viewModel.Manager.Source;

            viewModel.MediaFiles.GetNextHighlight(viewModel.Manager.Position, ref mediaFile, out highlight);

            if (highlight != null)
            {
                viewModel.Manager.Source = mediaFile;
                viewModel.Manager.Position = highlight.Begin;
            }
            else player.Stop();
        }

        private void SetPreviousMedia()
        {
            if (viewModel.Manager.Mode == PlayTypeState.Highlights)
            {
                Highlight highlight;
                MediaFile mediaFile = viewModel.Manager.Source;

                viewModel.MediaFiles.GetPreviousMediaAndFirstHighlight(ref mediaFile, out highlight);

                viewModel.Manager.Source = mediaFile;
            }
            else viewModel.Manager.Source = viewModel.MediaFiles.GetPreviousMedia(viewModel.Manager.Source);
        }

        private void SetPreviousHighlight()
        {
            Highlight highlight;
            MediaFile mediaFile = viewModel.Manager.Source;

            viewModel.MediaFiles.GetPreviousHighlight(viewModel.Manager.Position, ref mediaFile, out highlight);

            viewModel.Manager.Source = mediaFile;
            viewModel.Manager.Position = highlight.Begin;
        }

        private void Stop()
        {
            //player.Clock.Controller.Stop();
            //player.Clock = null;
            viewModel.Manager.Source = null;
        }

        private void Grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            viewModel.Manager.Source = null;

            foreach(MediaFile mediaFile in viewModel.MediaFiles.Where(m=>m.HasChanges))
            {
                mediaFile.SaveHighlights();
            }
        }
    }
}
