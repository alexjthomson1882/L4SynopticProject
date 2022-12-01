using Microsoft.Toolkit.Uwp.UI.Controls;

using MusicPlayer.Media;

using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace MusicPlayer.Utility {

    public static class MediaDataGridUtility {

        #region HandleSortMediaDataGrid

        public static void HandleSortMediaDataGrid(in DataGrid dataGrid, in DataGridColumn column, in IList<AudioMedia> list) {
            // get column tag:
            string columnTag = column.Tag.ToString();
            // calculate sort direction:
            DataGridSortDirection? sortDirection;
            switch (column.SortDirection) {
                case DataGridSortDirection.Ascending: sortDirection = DataGridSortDirection.Descending; break;
                case DataGridSortDirection.Descending: sortDirection = null; break;
                default: sortDirection = DataGridSortDirection.Ascending; break;
            };
            // sort columns:
            switch (columnTag) {
                case "TrackName": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.Title ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.Title descending
                            select media
                        );
                    } else {
                        dataGrid.ItemsSource = list;
                    }
                    column.SortDirection = sortDirection;
                    break;
                }
                case "AlbumName": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.Album ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.Album descending
                            select media
                        );
                    } else {
                        dataGrid.ItemsSource = list;
                    }
                    column.SortDirection = sortDirection;
                    break;
                }
                case "ArtistName": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.Artist ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.Artist descending
                            select media
                        );
                    } else {
                        dataGrid.ItemsSource = list;
                    }
                    column.SortDirection = sortDirection;
                    break;
                }
                case "Duration": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.Duration ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.Duration descending
                            select media
                        );
                    } else {
                        dataGrid.ItemsSource = list;
                    }
                    column.SortDirection = sortDirection;
                    break;
                }
                case "DateAdded": {
                    if (sortDirection == DataGridSortDirection.Ascending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.DateAdded ascending
                            select media
                        );
                    } else if (sortDirection == DataGridSortDirection.Descending) {
                        dataGrid.ItemsSource = new ObservableCollection<AudioMedia>(
                            from media in list
                            orderby media.DateAdded descending
                            select media
                        );
                    } else {
                        dataGrid.ItemsSource = list;
                    }
                    column.SortDirection = sortDirection;
                    break;
                }
            }
        }

        #endregion

        #region HandleSearchMediaDataGrid

        public static void HandleSearchMediaDataGrid(in DataGrid dataGrid, in IList<AudioMedia> mediaList, in string searchQuery) {
            if (string.IsNullOrWhiteSpace(searchQuery)) {
                dataGrid.ItemsSource = mediaList;
            } else {
                ObservableCollection<AudioMedia> searchedMedia = SearchMedia(mediaList, searchQuery);
                dataGrid.ItemsSource = searchedMedia;
            }
        }

        #endregion

        #region SearchMedia

        private static ObservableCollection<AudioMedia> SearchMedia(IList<AudioMedia> list, string query) {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (list == null) throw new ArgumentNullException(nameof(list));
            return new ObservableCollection<AudioMedia>(
                (
                    from item in list
                    where (
                        item.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                        || item.Artist.Contains(query, StringComparison.OrdinalIgnoreCase)
                        || item.Artist.Contains(query, StringComparison.OrdinalIgnoreCase)
                    )
                    select item
                ).ToArray()
            );
        }

        #endregion

    }

}
