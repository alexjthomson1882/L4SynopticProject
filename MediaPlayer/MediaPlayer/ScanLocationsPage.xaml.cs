using Microsoft.Data.Sqlite;

using MusicPlayer.Data;
using MusicPlayer.Media;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace MusicPlayer {

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScanLocationsPage : Page {

        #region variable

        private MediaManager mediaManager = null;

        #endregion

        #region property

        public ObservableCollection<MediaLocation> LocationsList { get; private set; }

        #endregion

        #region constructor

        public ScanLocationsPage() {
            InitializeComponent();
            // get audio player:
            App application = Application.Current as App;
            if (application != null) {
                mediaManager = application.MediaManager;
            }
            // get media:
            LocationsList = new ObservableCollection<MediaLocation>(mediaManager.GetMediaLocationsList());
        }

        #endregion

        #region logic

        private void Button_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

    }
}
