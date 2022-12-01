using MusicPlayer.Media;
using MusicPlayer.Playback;

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace MusicPlayer {

    /// <summary>
    /// Main window content. The <see cref="MainPage"/> defines the basic layout for the entire application.
    /// </summary>
    public sealed partial class MainPage : Page {

        #region variable

        /// <summary>
        /// <see cref="AudioPlayer"/> used for playing audio.
        /// </summary>
        private readonly PlaybackManager playbackManager;

        /// <summary>
        /// <see cref="MediaManager"/> used for managing media.
        /// </summary>
        private readonly MediaManager mediaManager;

        /// <summary>
        /// Tracks if the <see cref="SeekSlider"/> is being manually controlled.
        /// </summary>
        private bool manualSeekSliderManipulation = false;

        /// <summary>
        /// Tracks if the <see cref="VolumeSlider"/> is being manually controlled.
        /// </summary>
        private bool manualVolumeSliderManipulation = false;

        #endregion

        #region constructor

        public MainPage() {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            // get audio player and media manager:
            App application = Application.Current as App;
            playbackManager = application.PlaybackManager;
            mediaManager = application.MediaManager;
            // bind event callbacks:
            playbackManager.OnMediaMounted += AudioPlayer_OnMediaMounted;
            playbackManager.OnMediaUnmounted += AudioPlayer_OnMediaUnmounted;
            playbackManager.OnMediaPlaybackStart += AudioPlayer_OnMediaPlaybackStart;
            playbackManager.OnMediaPlaybackStop += AudioPlayer_OnMediaPlaybackStop;
            playbackManager.OnMediaPlaybackPositionChanged += AudioPlayer_OnMediaPlaybackPositionChanged;
        }

        #endregion

        #region logic

        #region MainPage

        private void MainPage_Loaded(object sender, RoutedEventArgs e) {
            SetPlaybackPosition(0.0, 0.0);
        }

        #endregion

        #region SetMediaInfo

        /// <summary>
        /// Sets / updates the current media information with a <paramref name="title"/> and an <paramref name="artist"/>.
        /// </summary>
        private void SetMediaInfo(in string title, in string artist) {
            MediaTitle.Text = title ?? string.Empty;
            MediaArtist.Text = artist ?? string.Empty;
        }

        #endregion

        #region SetPlaybackPosition

        /// <summary>
        /// Sets / updates the playback position of the current media with a <paramref name="position"/> and <paramref name="duration"/>.
        /// </summary>
        /// <param name="position">Number of seconds into the current media.</param>
        /// <param name="duration">Duration of the current media in seconds.</param>
        private void SetPlaybackPosition(in double position, in double duration) {
            // update seek slider:
            if (!manualSeekSliderManipulation) {
                SeekSlider.Minimum = 0.0;
                SeekSlider.Maximum = duration;
                SeekSlider.Value = position;
            }
            // update seek text:
            SeekTime.Text = TimeSpan.FromSeconds(position).ToString(@"mm\:ss");
            SeekDuration.Text = TimeSpan.FromSeconds(duration).ToString(@"mm\:ss");
        }

        #endregion

        #region SeekSlider

        private void SeekSlider_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) {
            manualSeekSliderManipulation = true;
        }

        private void SeekSlider_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            manualSeekSliderManipulation = false;
            playbackManager.PlaybackPosition = SeekSlider.Value;
        }

        private void SeekSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) {
            if (manualSeekSliderManipulation) {
                SetPlaybackPosition(SeekSlider.Value, playbackManager.PlaybackDuration);
            } else {
                playbackManager.PlaybackPosition = SeekSlider.Value;
            }
        }

        #endregion

        #region AudioPlayer

        private void AudioPlayer_OnMediaMounted(AudioMedia media) {
            SetMediaInfo(media.Title, media.Artist);
        }

        private void AudioPlayer_OnMediaUnmounted(AudioMedia media) {
            SetMediaInfo(null, null);
        }

        private void AudioPlayer_OnMediaPlaybackStart(AudioMedia media) {
            MediaPlayText.Text = "\xEC72";
        }

        private void AudioPlayer_OnMediaPlaybackStop(AudioMedia media) {
            MediaPlayText.Text = "\xEC74";
        }

        private void AudioPlayer_OnMediaPlaybackPositionChanged(AudioMedia media, double position, double duration) {
            // check if slider is being manually manipulated:
            if (manualSeekSliderManipulation) return;
            // update slider position:
            SetPlaybackPosition(position, duration);
        }

        #endregion

        #region PageContent

        private void PageContent_Loaded(object sender, RoutedEventArgs e) {
            // update controls:
            UpdateVolumeControls();
            // navigate to home page:
            NavigateTo<LibraryPage>();
        }

        #endregion

        #region NavigateTo

        /// <summary>
        /// Navigates to a <see cref="Page"/> of type <typeparamref name="T"/>.
        /// </summary>
        private void NavigateTo<T>(in object parameter = null) where T : Page => ContentFrame.Navigate(typeof(T), parameter);

        #endregion

        #region LibraryButton

        private void LibraryButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<LibraryPage>();
        }

        #endregion

        #region Playlists

        private void PlaylistsButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<PlaylistListPage>();
        }

        #endregion

        #region CreatePlaylist

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e) {
            Playlist playlist = mediaManager.CreatePlaylist("New Playlist");
            NavigateTo<PlaylistPage>(playlist);
        }

        #endregion

        #region ScanLocations

        private void ScanLocationsButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<ScanLocationsPage>();
        }

        #endregion

        #region MuteButton

        private void MuteButton_Click(object sender, RoutedEventArgs e) {
            if (playbackManager == null) return;
            playbackManager.IsMuted = !playbackManager.IsMuted;
            UpdateVolumeControls();
        }

        #endregion

        #region VolumeSlider

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) {
            if (!manualVolumeSliderManipulation) {
                playbackManager.Volume = VolumeSlider.Value * 0.01;
                UpdateVolumeControls();
            }
        }

        private void VolumeSlider_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) {
            manualVolumeSliderManipulation = true;
        }

        private void VolumeSlider_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            manualVolumeSliderManipulation = false;
            playbackManager.Volume = VolumeSlider.Value * 0.01;
            UpdateVolumeControls();
        }

        #endregion

        #region UpdateVolumeControls

        private void UpdateVolumeControls() {
            if (playbackManager == null) return;
            VolumeSlider.Value = playbackManager.Volume * 100.0;
            MuteButtonText.Text = playbackManager.IsMuted ? "\xECB8" : "\xECB6";
        }

        #endregion

        #region MediaPlayButton

        private void MediaPlayButton_Click(object sender, RoutedEventArgs e) {
            bool hasMedia = playbackManager.HasMedia;
            bool nextPlayState = !playbackManager.IsPlaying;
            if (!hasMedia && nextPlayState) nextPlayState = false;
            if (nextPlayState) {
                playbackManager.Resume();
            } else {
                playbackManager.Pause();
            }
        }

        #endregion

        #region MediaPreviousButton

        private void MediaPreviousButton_Click(object sender, RoutedEventArgs e) {
            playbackManager.Last();
        }

        #endregion

        #region MediaNextButton

        private void MediaNextButton_Click(object sender, RoutedEventArgs e) {
            playbackManager.Next();
        }

        #endregion

        #region MediaShuffleButton

        private void MediaShuffleButton_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #region MediaRepeatButton

        private void MediaRepeatButton_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #endregion

    }
}
