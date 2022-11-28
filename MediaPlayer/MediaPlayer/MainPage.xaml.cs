using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MusicPlayer {

    /// <summary>
    /// Main window content. The <see cref="MainPage"/> defines the basic layout for the entire application.
    /// </summary>
    public sealed partial class MainPage : Page {

        #region constant

        #endregion

        #region variable

        private AudioPlayer audioPlayer;

        #endregion

        #region constructor

        public MainPage() {
            InitializeComponent();
            audioPlayer = null;
        }

        #endregion

        #region logic
        
        #region PageContent

        private void PageContent_Loaded(object sender, RoutedEventArgs e) {
            // get audio player:
            App application = Application.Current as App;
            if (application != null) {
                audioPlayer = application.AudioPlayer;
            }
            // update controls:
            UpdateVolumeControls();
            // navigate to home page:
            NavigateTo<HomePage>();
        }

        #endregion

        #region NavigateTo

        /// <summary>
        /// Navigates to a <see cref="Page"/> of type <typeparamref name="T"/>.
        /// </summary>
        private void NavigateTo<T>() where T : Page => ContentFrame.Navigate(typeof(T));

        #endregion

        #region HomeButton

        private void HomeButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<HomePage>();
        }

        #endregion

        #region SearchButton

        private void SearchButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<SearchPage>();
        }

        #endregion

        #region LibraryButton

        private void LibraryButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<LibraryPage>();
        }

        #endregion

        #region CreatePlaylist

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<ViewPlaylistPage>();
        }

        #endregion

        #region MuteButton

        private void MuteButton_Click(object sender, RoutedEventArgs e) {
            if (audioPlayer == null) return;
            audioPlayer.IsMuted = !audioPlayer.IsMuted;
            UpdateVolumeControls();
        }

        #endregion

        #region VolumeSlider

        private void VolumeSlider_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            if (audioPlayer == null) return;
            audioPlayer.Volume = (float)VolumeSlider.Value * 0.01f;
            UpdateVolumeControls();
        }

        #endregion

        #region UpdateVolumeControls

        private void UpdateVolumeControls() {
            if (audioPlayer == null) return;
            VolumeSlider.Value = audioPlayer.Volume * 100.0f;
            MuteButtonText.Text = audioPlayer.IsMuted ? "\xECB8" : "\xECB6";
        }

        #endregion

        #endregion

    }
}
