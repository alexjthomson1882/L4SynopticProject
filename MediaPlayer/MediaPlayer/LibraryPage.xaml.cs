using MusicPlayer.Media;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Microsoft.Toolkit.Uwp.UI.Controls;

using System.Linq;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MusicPlayer {

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page {

        #region variable

        private AudioPlayer audioPlayer = null;

        private MediaManager mediaManager = null;

        #endregion

        #region property

        public ObservableCollection<AudioMedia> MediaList { get; private set; }

        #endregion

        #region constructor

        public LibraryPage() {
            InitializeComponent();
            // get audio player and media manager:
            App application = Application.Current as App;
            if (application != null) {
                audioPlayer = application.AudioPlayer;
                mediaManager = application.MediaManager;
            }
           // get media:
           MediaList = new ObservableCollection<AudioMedia>(mediaManager.GetMediaList());
        }

        #endregion

        #region logic

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
                    }
                    e.Column.SortDirection = sortDirection;
                    break;
                }
            }
        }

        private void MediaDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            DataGrid dataGrid = sender as DataGrid;
            if (dataGrid == null) return;
            AudioMedia selectedMedia = dataGrid.SelectedItem as AudioMedia;
            if (selectedMedia == null) return;
            audioPlayer.Play(selectedMedia);
        }

        #endregion

    }
}
