using HighlightLib;
using HighlightPlayer.Controls;
using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace HighlightPlayer
{
    class ViewModel : INotifyPropertyChanged
    {
        private MediaFileCollection mediaFiles;
        private LoopState loopType;
        private SwitchViewState viewType;
        private ShowHighlightsState showHighlights;

        public HighlightTimeLineManager Manager { get; private set; }

        public ShuffleState ShuffleType
        {
            get
            {
                if (mediaFiles is ShuffleOneMediaFileCollection) return ShuffleState.OneTime;
                if (mediaFiles is ShuffleCompleteMediaFileCollection) return ShuffleState.Complete;
                return ShuffleState.Off;
            }

            set
            {
                if (value == ShuffleType) return;

                if (value == ShuffleState.Off) MediaFiles = new MediaFileCollection(mediaFiles);
                else if (value == ShuffleState.OneTime) MediaFiles = new ShuffleOneMediaFileCollection(mediaFiles);
                else MediaFiles = new ShuffleCompleteMediaFileCollection(mediaFiles);
            }
        }

        public MediaFileCollection MediaFiles
        {
            get { return mediaFiles; }
            set
            {
                if (value == mediaFiles) return;

                mediaFiles = value;
                OnPropertyChanged("MediaFiles");
                OnPropertyChanged("ShuffleType");
            }
        }

        public LoopState LoopType
        {
            get { return loopType; }
            set
            {
                if (value == loopType) return;

                loopType = value;
                OnPropertyChanged("LoopType");
            }
        }
       
        public SwitchViewState ViewType
        {
            get { return viewType; }
            set
            {
                if (value == viewType) return;

                viewType = value;
                OnPropertyChanged("ViewType");
            }
        }

        public ShowHighlightsState ShowHighlights
        {
            get { return showHighlights; }
            set
            {
                if (value == showHighlights) return;

                showHighlights = value;
                OnPropertyChanged("ShowHighlights");
            }
        }

        public ViewModel(MediaElement player)
        {
            Manager = new HighlightTimeLineManager(player);
            MediaFiles = new MediaFileCollection();

            ShuffleType = ShuffleState.Off;
            LoopType = LoopState.Off;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
