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
            // initialise component:
            InitializeComponent();
            Loaded += OnLoaded;
            DataContext = this;
            // get audio player and media manager:
            App application = Application.Current as App;
            playbackManager = application.PlaybackManager;
            mediaManager = application.MediaManager;
            // get media:
            UpdateMediaList();
        }

        #endregion

        #region logic

        #region OnLoaded

        private void OnLoaded(object sender, RoutedEventArgs e) {
            UpdateMediaList();
        }

        #endregion

        #region UpdateMediaList

        private void UpdateMediaList() {
            MediaList = new ObservableCollection<AudioMedia>(mediaManager.GetMediaList());
            MediaDataGrid.ItemsSource = MediaList;
        }

        #endregion

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

        #region RemoveFromLibrary

        private void RemoveFromLibrary_Click(object sender, RoutedEventArgs e) {
            AudioMedia audioMedia = (sender as FrameworkElement).DataContext as AudioMedia;
            mediaManager.RemoveAudioMediaFromLibrary(audioMedia);
            UpdateMediaList();
        }

        #endregion

        #endregion

    }
}
