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
            playbackManager.Play(selectedMedia);
            playbackManager.Resume();
        }

        private void MediaDataGrid_Sorting(object sender, DataGridColumnEventArgs e) {
            // get column tag:
            string columnTag = e.Column.Tag.ToString();
            // calculate sort direction:
            DataGridSortDirection? sortDirection;
            switch (e.Column.SortDirection) {
                case DataGridSortDirection.Ascending: sortDirection = DataGridSortDirection.Descending; break;
                case DataGridSortDirection.Descending: sortDirection = null; break;
                default: sortDirection = DataGridSortDirection.Ascending; break;
            };
            // sort columns:
            switch (columnTag) {
                case "TrackName": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.Title ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.Title descending
                            select media
                        );
                    } else {
                        MediaDataGrid.ItemsSource = MediaList;
                    }
                    e.Column.SortDirection = sortDirection;
                    break;
                }
                case "AlbumName": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.Album ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.Album descending
                            select media
                        );
                    } else {
                        MediaDataGrid.ItemsSource = MediaList;
                    }
                    e.Column.SortDirection = sortDirection;
                    break;
                }
                case "ArtistName": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.Artist ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.Artist descending
                            select media
                        );
                    } else {
                        MediaDataGrid.ItemsSource = MediaList;
                    }
                    e.Column.SortDirection = sortDirection;
                    break;
                }
                case "Duration": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.Duration ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.Duration descending
                            select media
                        );
                    } else {
                        MediaDataGrid.ItemsSource = MediaList;
                    }
                    e.Column.SortDirection = sortDirection;
                    break;
                }
                case "DateAdded": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.DateAdded ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        MediaDataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in MediaList
                            orderby media.DateAdded descending
                            select media
                        );
                    } else {
                        MediaDataGrid.ItemsSource = MediaList;
                    }
                    e.Column.SortDirection = sortDirection;
                    break;
                }
            }
        }

        #endregion

        #endregion

    }
}
