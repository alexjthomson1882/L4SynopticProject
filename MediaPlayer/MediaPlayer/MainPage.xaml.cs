using MusicPlayer.Media;

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

        private AudioPlayer audioPlayer;
        private MediaManager mediaManager;

        /// <summary>
        /// Tracks if the <see cref="SeekSlider"/> is being manually controlled.
        /// </summary>
        private bool manualSliderManipulation = false;

        #endregion

        #region constructor

        public MainPage() {
            InitializeComponent();
            Loaded += MainPage_Loaded;
            // get audio player and media manager:
            App application = Application.Current as App;
            if (application != null) {
                audioPlayer = application.AudioPlayer;
                mediaManager = application.MediaManager;
                mediaManager.Initialise();
                // bind event callbacks:
                audioPlayer.OnMediaMounted += AudioPlayer_OnMediaMounted;
                audioPlayer.OnMediaUnmounted += AudioPlayer_OnMediaUnmounted;
                audioPlayer.OnMediaPlaybackStart += AudioPlayer_OnMediaPlaybackStart;
                audioPlayer.OnMediaPlaybackStop += AudioPlayer_OnMediaPlaybackStop;
                audioPlayer.OnMediaPlaybackPositionChanged += AudioPlayer_OnMediaPlaybackPositionChanged;
            } else {
                audioPlayer = null;
                mediaManager = null;
            }
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
            if (!manualSliderManipulation) {
                SeekSlider.Minimum = 0.0;
                SeekSlider.Maximum = duration;
                SeekSlider.Value = position;
            }
            // update seek text:
            SeekTime.Text = TimeSpan.FromSeconds(position).ToString(@"mm\:ss");
            SeekDuration.Text = TimeSpan.FromSeconds(duration).ToString(@"mm\:ss");
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
            if (manualSliderManipulation) return;
            // update slider position:
            SetPlaybackPosition(position, duration);
        }

        #endregion

        #region PageContent

        private void PageContent_Loaded(object sender, RoutedEventArgs e) {
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

        #region Playlists

        private void PlaylistsButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<ViewPlaylistPage>();
        }

        #endregion

        #region CreatePlaylist

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<ViewPlaylistPage>();
        }

        #endregion

        #region ScanLocations

        private void ScanLocationsButton_Click(object sender, RoutedEventArgs e) {
            NavigateTo<ScanLocationsPage>();
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

        #region SeekSlider

        private void SeekSlider_Tapped(object sender, TappedRoutedEventArgs e) {
            SeekSlider_EmulatePress();
        }

        private void SeekSlider_PointerPressed(object sender, PointerRoutedEventArgs e) {
            SeekSlider_EmulatePress();
        }

        private void SeekSlider_EmulatePress() {
            manualSliderManipulation = true;
            SeekSlider.ValueChanged -= SeekSlider_ValueChanged;
            SeekSlider.ValueChanged += SeekSlider_ValueChanged;
            App.QueueRunAsync(() => { audioPlayer.PlaybackPosition = SeekSlider.Value; manualSliderManipulation = false; });
        }

        private void SeekSlider_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e) {
            manualSliderManipulation = true;
            SeekSlider.ValueChanged -= SeekSlider_ValueChanged;
            SeekSlider.ValueChanged += SeekSlider_ValueChanged;
        }

        private void SeekSlider_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            App.QueueRunAsync(() => { audioPlayer.PlaybackPosition = SeekSlider.Value; manualSliderManipulation = false; });
        }

        private void SeekSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) {
            if (manualSliderManipulation) {
                SetPlaybackPosition(SeekSlider.Value, audioPlayer.CurrentMediaDuration);
            } else {
                SeekSlider.ValueChanged -= SeekSlider_ValueChanged;
            }
        }

        #endregion

        #region MediaPlayButton

        private void MediaPlayButton_Click(object sender, RoutedEventArgs e) {
            bool hasMedia = audioPlayer.HasMedia;
            bool nextPlayState = !audioPlayer.IsPlaying;
            if (!hasMedia && nextPlayState) nextPlayState = false;
            if (nextPlayState) {
                audioPlayer.Play();
            } else {
                audioPlayer.Pause();
            }
        }

        #endregion

        #region MediaPreviousButton

        private void MediaPreviousButton_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #region MediaShuffleButton

        private void MediaShuffleButton_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #region MediaNextButton

        private void MediaNextButton_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #region MediaRepeatButton

        private void MediaRepeatButton_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #endregion

    }
}
