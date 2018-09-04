using HighlightLib;
using HighlightPlayer.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HighlightPlayer
{
    public delegate void CurrentMediaFileChangedHandler(MediaPlayManager sender, CurrentMediaFileChangedEventArgs args);
    public delegate void CurrentHighlightChangedHandler(MediaPlayManager sender, CurrentHighlightChangedEventArgs args);

    public class MediaPlayManager
    {
        public event CurrentMediaFileChangedHandler CurrentMediaFileChanged;
        public event CurrentHighlightChangedHandler CurrentHighlightChanged;

        private PreviousMediaButton previousMediaButton;
        private PreviousHighlightButton previousHighlightButton;
        private PlayPauseButton playPauseButton;
        private NextHighlightButton nextHighlightButton;
        private NextMediaButton nextMediaButton;
        private ShuffleButton shuffleButton;
        private LoopButton loopButton;
        private PlayTypeButton playTypeButton;
        private HighlightButton highlightButton;

        private MediaPositionSlider slider;
        private MediaElement mediaElement;

        private bool isGoingForward;
        private int currentMediaFileIndex;
        private MediaFile currentMediaFile;
        private ObservableCollection<MediaFile> mediaFiles;

        private Highlight currentHighlight;

        public PreviousMediaButton PreviousMediaButton
        {
            get { return previousMediaButton; }
            set
            {
                if (value == previousMediaButton) return;

                if (previousMediaButton != null) previousMediaButton.ButtonStateChanged -= PreviousMediaButton_ButtonStateChanged;
                if (value != null) value.ButtonStateChanged += PreviousMediaButton_ButtonStateChanged;

                previousMediaButton = value;
            }
        }

        public PreviousHighlightButton PreviousHighlightButton
        {
            get { return previousHighlightButton; }
            set
            {
                if (value == previousHighlightButton) return;

                if (previousMediaButton != null) previousHighlightButton.ButtonStateChanged -= PreviousHighlightButton_ButtonStateChanged;
                if (value != null) value.ButtonStateChanged += PreviousHighlightButton_ButtonStateChanged;

                previousHighlightButton = value;
            }
        }

        public PlayPauseButton PlayPauseButton
        {
            get { return playPauseButton; }
            set
            {
                if (value == playPauseButton) return;

                if (playPauseButton != null)
                {
                    playPauseButton.ButtonStateChanging += PlayPauseButton_ButtonStateChanging;
                    playPauseButton.ButtonStateChanged -= PlayPauseButton_ButtonStateChanged;
                }

                if (value != null)
                {
                    value.ButtonStateChanging += PlayPauseButton_ButtonStateChanging;
                    value.ButtonStateChanged += PlayPauseButton_ButtonStateChanged;
                }

                playPauseButton = value;
            }
        }

        public NextHighlightButton NextHighlightButton
        {
            get { return nextHighlightButton; }
            set
            {
                if (value == nextHighlightButton) return;

                if (nextHighlightButton != null) nextHighlightButton.ButtonStateChanged -= NextHighlightButton_ButtonStateChanged;
                if (value != null) value.ButtonStateChanged += NextHighlightButton_ButtonStateChanged;

                nextHighlightButton = value;
            }
        }

        public NextMediaButton NextMediaButton
        {
            get { return nextMediaButton; }
            set
            {
                if (value == nextMediaButton) return;

                if (nextMediaButton != null) nextMediaButton.ButtonStateChanged -= NextMediaButton_ButtonStateChanged;
                if (value != null) value.ButtonStateChanged += NextMediaButton_ButtonStateChanged;

                nextMediaButton = value;
            }
        }

        public ShuffleButton ShuffleButton
        {
            get { return shuffleButton; }
            set
            {
                if (value == shuffleButton) return;

                if (shuffleButton != null) shuffleButton.ButtonStateChanged -= ShuffleButton_ButtonStateChanged;
                if (value != null) value.ButtonStateChanged += ShuffleButton_ButtonStateChanged;

                shuffleButton = value;
            }
        }

        public LoopButton LoopButton
        {
            get { return loopButton; }
            set
            {
                if (value == loopButton) return;

                if (loopButton != null) loopButton.ButtonStateChanged -= LoopButton_ButtonStateChanged;
                if (value != null) value.ButtonStateChanged += LoopButton_ButtonStateChanged;

                loopButton = value;
            }
        }

        public PlayTypeButton PlayTypeButton
        {
            get { return playTypeButton; }
            set
            {
                if (value == playTypeButton) return;

                if (playTypeButton != null) playTypeButton.ButtonStateChanged -= PlayTypeButton_ButtonStateChanged;
                if (value != null) value.ButtonStateChanged += PlayTypeButton_ButtonStateChanged;

                playTypeButton = value;
            }
        }

        public HighlightButton HighlightButton
        {
            get { return highlightButton; }
            set
            {
                if (value == highlightButton) return;

                if (highlightButton != null) highlightButton.ButtonStateChanged -= HighlightButton_ButtonStateChanged;
                if (value != null) value.ButtonStateChanged += HighlightButton_ButtonStateChanged;

                highlightButton = value;
            }
        }

        public MediaPositionSlider Slider
        {
            get { return slider; }
            set
            {
                if (value == slider) return;

                if (slider != null) slider.MediaPositionChanged -= Slider_MediaPositionChanged;
                if (value != null) value.MediaPositionChanged += Slider_MediaPositionChanged;

                slider = value;
            }
        }

        public MediaElement MediaElement
        {
            get { return mediaElement; }
            set
            {
                if (value == mediaElement) return;

                if (mediaElement != null)
                {
                    mediaElement.MediaOpened -= MediaElement_MediaOpened;
                    mediaElement.MediaFailed -= MediaElement_MediaFailed;
                }

                if (value != null)
                {
                    value.MediaOpened += MediaElement_MediaOpened;
                    value.MediaFailed += MediaElement_MediaFailed;
                }

                mediaElement = value;

                if (mediaElement != null && CurrentMediaFile != null) mediaElement.Source = CurrentMediaFile.Uri;
            }
        }

        public int CurrentMediaFileIndex
        {
            get { return currentMediaFileIndex < mediaFiles.Count ? currentMediaFileIndex : mediaFiles.Count - 1; }
            set { CurrentMediaFile = mediaFiles.ElementAtOrDefault(value); }
        }

        public MediaFile CurrentMediaFile
        {
            get { return currentMediaFile; }
            set
            {
                int newCurrentMediaFileIndex = mediaFiles.IndexOf(value);

                if (value == currentMediaFile || (newCurrentMediaFileIndex == -1 && value != null)) return;

                if (currentMediaFile != null) currentMediaFile.Highlights.CollectionChanged -= Highlights_CollectionChanged;
                if (value != null) value.Highlights.CollectionChanged += Highlights_CollectionChanged;

                CurrentMediaFileChangedEventArgs args = new CurrentMediaFileChangedEventArgs
                    (currentMediaFileIndex, newCurrentMediaFileIndex, currentMediaFile, value);

                currentMediaFile = value;
                currentMediaFileIndex = newCurrentMediaFileIndex;

                if (mediaElement != null) mediaElement.Source = currentMediaFile.Uri;

                CurrentMediaFileChanged?.Invoke(this, args);
            }
        }

        public ObservableCollection<MediaFile> MediaFiles { get { return mediaFiles; } }

        public int CurrentHighlightIndex
        {
            get { return GetCurrentHighlightIndex(); }
            set { CurrentHighlight = CurrentMediaFile?.Highlights.ElementAtOrDefault(value); }
        }

        public Highlight CurrentHighlight
        {
            get { return currentHighlight; }
            set
            {
                int newCurrentHighlightIndex = CurrentMediaFile?.Highlights.IndexOf(value) ?? -1;

                if (value == currentHighlight) return;

                CurrentHighlightChangedEventArgs args = new CurrentHighlightChangedEventArgs
                    (CurrentHighlightIndex, newCurrentHighlightIndex, currentHighlight, value);

                currentHighlight = value;
                if (currentHighlight != null && MediaElement != null) MediaElement.Position = currentHighlight.Begin;

                CurrentHighlightChanged?.Invoke(this, args);
            }
        }

        public MediaPlayManager()
        {
            isGoingForward = true;
            currentMediaFileIndex = -1;
            currentMediaFile = null;
            mediaFiles = new ObservableCollection<MediaFile>();
            mediaFiles.CollectionChanged += MediaFiles_CollectionChanged;
        }

        private void MediaFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int newCurrentMediaFileIndex = mediaFiles.IndexOf(CurrentMediaFile);

            if (newCurrentMediaFileIndex != -1) currentMediaFileIndex = newCurrentMediaFileIndex;
            else CurrentMediaFileIndex = CurrentMediaFileIndex;
        }

        private void PreviousMediaButton_ButtonStateChanged(object sender, ButtonStateChangedEventArgs<PreviousMediaState> args)
        {
            MediaFile newCurrentMediaFile;
            Highlight newCurrentHighlight;

            GetPlayType().PreviousMedia(out newCurrentMediaFile, out newCurrentHighlight);

            CurrentMediaFile = newCurrentMediaFile;
            CurrentHighlight = newCurrentHighlight;
        }

        private void PreviousHighlightButton_ButtonStateChanged(object sender, ButtonStateChangedEventArgs<PreviousHighlightState> args)
        {
            MediaFile newCurrentMediaFile;
            Highlight newCurrentHighlight;

            GetPlayType().PreviousHighlight(out newCurrentMediaFile, out newCurrentHighlight);

            CurrentMediaFile = newCurrentMediaFile;
            CurrentHighlight = newCurrentHighlight;
        }

        private void PlayPauseButton_ButtonStateChanging(object sender, ButtonStateChangingEventArgs<PlayPauseState> args)
        {
            if (CurrentMediaFile == null) args.NewValue = PlayPauseState.Play;
        }

        private void PlayPauseButton_ButtonStateChanged(object sender, ButtonStateChangedEventArgs<PlayPauseState> args)
        {
            if (MediaElement == null) return;

            MediaElement.LoadedBehavior = PlayPauseButton.CurrentState == PlayPauseState.Play ? MediaState.Pause : MediaState.Play;
        }

        private void NextHighlightButton_ButtonStateChanged(object sender, ButtonStateChangedEventArgs<NextHighlightState> args)
        {
            MediaFile newCurrentMediaFile;
            Highlight newCurrentHighlight;

            GetPlayType().NextHighlight(out newCurrentMediaFile, out newCurrentHighlight);

            CurrentMediaFile = newCurrentMediaFile;
            CurrentHighlight = newCurrentHighlight;
        }

        private void NextMediaButton_ButtonStateChanged(object sender, ButtonStateChangedEventArgs<NextMediaState> args)
        {
            MediaFile newCurrentMediaFile;
            Highlight newCurrentHighlight;

            GetPlayType().NextMedia(out newCurrentMediaFile, out newCurrentHighlight);

            CurrentMediaFile = newCurrentMediaFile;
            CurrentHighlight = newCurrentHighlight;
        }

        private void ShuffleButton_ButtonStateChanged(object sender, ButtonStateChangedEventArgs<ShuffleState> args)
        {

        }

        private void LoopButton_ButtonStateChanged(object sender, ButtonStateChangedEventArgs<LoopState> args)
        {
        }

        private void PlayTypeButton_ButtonStateChanged(object sender, ButtonStateChangedEventArgs<PlayTypeState> args)
        {
            if (args.NewValue == PlayTypeState.Highlights && CurrentHighlight == null)
            {
                MediaFile newCurrentMediaFile;
                Highlight newCurrentHighlight;

                GetPlayType().NextHighlight(out newCurrentMediaFile, out newCurrentHighlight);

                CurrentMediaFile = newCurrentMediaFile;
                CurrentHighlight = newCurrentHighlight;
            }
        }

        private void HighlightButton_ButtonStateChanged(object sender, ButtonStateChangedEventArgs<HighlightState> args)
        {
            if (MediaElement == null || CurrentMediaFile == null) return;


        }

        private void Slider_MediaPositionChanged(MediaPositionSlider sender, MediaPositionChangedEventArgs args)
        {
            if (MediaElement != null) MediaElement.Position = args.NewPosition;
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaElement.Position = CurrentHighlight?.Begin ?? new TimeSpan();
        }

        private void MediaElement_MediaFailed(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
        }

        private void Highlights_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            CurrentHighlight = CurrentMediaFile.Highlights.ElementAtOrDefault(CurrentHighlightIndex);
        }

        private int GetCurrentHighlightIndex()
        {
            if (MediaElement == null || CurrentMediaFile == null) return -1;

            TimeSpan position = MediaElement.Position;

            for (int i = 0; i < CurrentMediaFile.Highlights.Count; i++)
            {
                if (CurrentMediaFile.Highlights[i].Begin >= position) return i - 1;
            }

            return -1;
        }

        private IPlayType GetPlayType()
        {
            switch (PlayTypeButton?.CurrentState)
            {
                case PlayTypeState.Highlights:
                    return new HighlightPlayType(this);

                default:
                    return new MediaPlayType(this);
            }
        }
    }
}
