using MusicPlayer.Media;
using MusicPlayer.Playback;

using System.Collections.ObjectModel;

using Microsoft.Toolkit.Uwp.UI.Controls;

using System.Linq;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MusicPlayer {

    public sealed partial class PlaylistListPage : Page {

        #region variable

        private MediaManager mediaManager = null;

        #endregion

        #region property

        public ObservableCollection<Playlist> PlaylistList { get; private set; }

        #endregion

        #region constructor

        public PlaylistListPage() {
            InitializeComponent();
            // get media manager:
            App application = Application.Current as App;
            mediaManager = application.MediaManager;
            // refresh playlists:
            RefreshPlaylists();
        }

        #endregion

        #region logic

        #region RefreshPlaylists

        /// <summary>
        /// Refreshes the list of playlists.
        /// </summary>
        private void RefreshPlaylists() {
            PlaylistList = new ObservableCollection<Playlist>(mediaManager.GetPlaylistList());
        }

        #endregion

        #region MediaDataGrid

        private void MediaDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!(sender is DataGrid dataGrid) || !(dataGrid.SelectedItem is Playlist selectedPlaylist)) return;
            Frame.Navigate(typeof(PlaylistPage), selectedPlaylist);
        }

        private void MediaDataGrid_Sorting(object sender, DataGridColumnEventArgs e) {
            
        }

        #endregion

#endregion

    }

}
