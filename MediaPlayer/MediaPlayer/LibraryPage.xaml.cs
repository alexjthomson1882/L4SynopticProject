using MusicPlayer.Media;
using MusicPlayer.Playback;

using System.Collections.ObjectModel;

using Microsoft.Toolkit.Uwp.UI.Controls;

using System.Linq;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Navigation;
using MusicPlayer.Utility;

namespace MusicPlayer {

    public sealed partial class LibraryPage : Page {

        #region variable

        private PlaybackManager playbackManager = null;

        private MediaManager mediaManager = null;

        #endregion

        #region property

        public ObservableCollection<AudioMedia> MediaList { get; private set; }

        #endregion

        #region constructor

        public LibraryPage() {
            // get audio player and media manager:
            App application = Application.Current as App;
            playbackManager = application.PlaybackManager;
            mediaManager = application.MediaManager;
            // get media:
            MediaList = new ObservableCollection<AudioMedia>(mediaManager.GetMediaList());
            // initialise component:
            InitializeComponent();
            DataContext = this;
        }

        #endregion

        #region logic

        #region MediaDataGrid

        private void MediaDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!(sender is DataGrid dataGrid) || !(dataGrid.SelectedItem is AudioMedia selectedMedia)) return;
            playbackManager.Play(MediaList, MediaList.IndexOf(selectedMedia));
            playbackManager.Resume();
        }

        private void MediaDataGrid_Sorting(object sender, DataGridColumnEventArgs e) {
            MediaDataGridUtility.HandleSortMediaDataGrid(MediaDataGrid, e.Column, MediaList);
        }

        #endregion

        #endregion

    }
}
