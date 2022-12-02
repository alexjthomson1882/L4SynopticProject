using Microsoft.Toolkit.Uwp.UI.Controls;

using MusicPlayer.Media;
using MusicPlayer.Playback;
using MusicPlayer.Utility;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MusicPlayer {

    public sealed partial class PlaylistPage : Page {

        #region variable

        private readonly MediaManager mediaManager;

        private readonly PlaybackManager playbackManager;

        private Playlist playlist;

        private bool isLoaded = false;

        #endregion

        #region property

        public ObservableCollection<AudioMedia> MediaList { get; private set; }

        public ObservableCollection<AudioMedia> LibraryMediaList { get; private set; }

        #endregion

        #region constructor

        public PlaylistPage() {
            InitializeComponent();
            Loaded += OnLoaded;
            // get media manager:
            App application = Application.Current as App;
            mediaManager = application.MediaManager;
            playbackManager = application.PlaybackManager;
            // initialise playlist and media list:
            playlist = null;
            MediaList = new ObservableCollection<AudioMedia>();
            UpdateLibrary();
        }

        #endregion

        #region logic

        #region OnLoaded

        private void OnLoaded(object sender, RoutedEventArgs e) {
            isLoaded = true;
            if (playlist != null) {
                UpdatePlaylist();
            }
        }

        #endregion

        #region OnNavigatedTo

        protected sealed override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            playlist = e.Parameter as Playlist;
            if (isLoaded && playlist != null) {
                UpdatePlaylist();
            }
        }

        #endregion

        #region UpdateLibrary

        private void UpdateLibrary() {
            if (LibraryMediaList == null) {
                LibraryMediaList = new ObservableCollection<AudioMedia>(mediaManager.GetMediaList());
            } else {
                LibraryMediaList.Clear();
                List<AudioMedia> list = mediaManager.GetMediaList();
                for (int i = 0; i < list.Count; i++) {
                    AudioMedia audioMedia = list[i];
                    LibraryMediaList.Add(audioMedia);
                }
            }
        }

        #endregion

        #region UpdatePlaylist

        private void UpdatePlaylist() {
            App.QueueRunAsync(() => {
                PlaylistNameTextBox.Text = playlist.Name;
            });
            // update audio media:
            MediaList.Clear();
            List<AudioMedia> audioMediaList = playlist.GetAudioMediaList();
            for (int i = 0; i < audioMediaList.Count; i++) {
                AudioMedia audioMedia = audioMediaList[i];
                MediaList.Add(audioMedia);
            }
        }

        #endregion

        #region PlaylistNameTextBox

        private void PlaylistNameTextBox_TextChanged(object sender, TextChangedEventArgs e) {
            // get playlist name:
            string playlistName = PlaylistNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(playlistName)) {
                playlistName = "Playlist";
                PlaylistNameTextBox.Text = playlistName;
            }
            // rename playlist:
            mediaManager.RenamePlaylist(playlist, playlistName);
        }

        #endregion

        #region DeletePlaylistButton

        private async void DeletePlaylistButton_Click(object sender, RoutedEventArgs e) {
            MessageDialog confirmDialog = new MessageDialog($"Are you sure you want to delete the `{playlist.Name}` playlist?", "Delete Playlist");
            confirmDialog.Commands.Add(new UICommand("Delete", null, true));
            confirmDialog.Commands.Add(new UICommand("Cancel", null, false));
            confirmDialog.DefaultCommandIndex = 0;
            confirmDialog.CancelCommandIndex = 1;
            IUICommand response = await confirmDialog.ShowAsync();
            if ((bool)response.Id) {
                // delete playlist:
                mediaManager.DeletePlaylist(playlist);
                Frame.Navigate(typeof(PlaylistListPage));
            }
        }

        #endregion

        #region AddToPlaylistButton

        private void AddToPlaylistButton_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #region MediaDataGrid

        private void MediaDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!(sender is DataGrid dataGrid) || !(dataGrid.SelectedItem is AudioMedia selectedMedia)) return;
            playbackManager.Play(MediaList, MediaList.IndexOf(selectedMedia));
        }

        private void MediaDataGrid_Sorting(object sender, DataGridColumnEventArgs e) {
            MediaDataGridUtility.HandleSortMediaDataGrid(MediaDataGrid, e.Column, MediaList);
        }

        #endregion

        #region LibraryDataGrid

        private void LibraryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (!(sender is DataGrid dataGrid) || !(dataGrid.SelectedItem is AudioMedia selectedMedia)) return;
            playbackManager.Play(LibraryMediaList, LibraryMediaList.IndexOf(selectedMedia));
        }

        private void LibraryDataGrid_Sorting(object sender, DataGridColumnEventArgs e) {
            MediaDataGridUtility.HandleSortMediaDataGrid(LibraryDataGrid, e.Column, LibraryMediaList);
        }

        #endregion

        #region AddToPlaylist

        private void AddToPlaylist_Click(object sender, RoutedEventArgs e) {
            AudioMedia audioMedia = (sender as FrameworkElement).DataContext as AudioMedia;
            mediaManager.AddAudioMediaToPlaylist(audioMedia, playlist);
            UpdatePlaylist();
        }

        #endregion

        #region RemoveFromPlaylist

        private void RemoveFromPlaylist_Click(object sender, RoutedEventArgs e) {
            AudioMedia audioMedia = (sender as FrameworkElement).DataContext as AudioMedia;
            mediaManager.RemoveAudioMediaFromPlaylist(audioMedia, playlist);
            UpdatePlaylist();
        }

        #endregion

        #region RemoveFromLibrary

        private void RemoveFromLibrary_Click(object sender, RoutedEventArgs e) {
            AudioMedia audioMedia = (sender as FrameworkElement).DataContext as AudioMedia;
            mediaManager.RemoveAudioMediaFromLibrary(audioMedia);
            UpdateLibrary();
            UpdatePlaylist();
        }

        #endregion

        #region PlaylistSearchBox

        private void PlaylistSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
                MediaDataGridUtility.HandleSearchMediaDataGrid(MediaDataGrid, MediaList, PlaylistSearchBox.Text);
            }
        }

        private void PlaylistSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            MediaDataGridUtility.HandleSearchMediaDataGrid(MediaDataGrid, MediaList, PlaylistSearchBox.Text);
        }

        #endregion

        #region LibrarySearchBox

        private void LibrarySearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
                MediaDataGridUtility.HandleSearchMediaDataGrid(LibraryDataGrid, LibraryMediaList, LibrarySearchBox.Text);
            }
        }

        private void LibrarySearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            MediaDataGridUtility.HandleSearchMediaDataGrid(LibraryDataGrid, LibraryMediaList, LibrarySearchBox.Text);
        }

        #endregion

        #endregion
    }

}
