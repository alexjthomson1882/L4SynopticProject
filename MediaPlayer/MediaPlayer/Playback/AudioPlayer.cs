using MusicPlayer.Media;

using System;

using Windows.Media.Core;
using Windows.Media.Playback;

namespace MusicPlayer.Playback {

    /// <summary>
    /// Responsible for playing an <see cref="AudioMedia"/> instance on the host system. Acts as a wrapper around a <see cref="MediaPlayer"/> instance.
    /// </summary>
    public sealed class AudioPlayer {

        #region variable

        /// <summary>
        /// <see cref="AudioMedia"/> currently playing.
        /// </summary>
        private AudioMedia currentMedia;

        /// <summary>
        /// Volume that audio should be played at. This is a linear scale from <c>0.0</c> to <c>1.0</c>.
        /// </summary>
        /// <seealso cref="Volume"/>
        private double volume;

        /// <summary>
        /// Tracks if the <see cref="AudioPlayer"/> should be muted.
        /// </summary>
        /// <seealso cref="IsMuted"/>
        private bool isMuted;

        /// <summary>
        /// <see cref="MediaPlayer"/> used to play audio.
        /// </summary>
        private MediaPlayer mediaPlayer;

        #endregion

        #region property

        /// <summary>
        /// Currently mounted <see cref="AudioMedia"/> that is being managed by the <see cref="AudioPlayer"/>.
        /// </summary>
        public AudioMedia CurrentMedia => currentMedia;

        /// <summary>
        /// Current volume of the <see cref="AudioPlayer"/>.
        /// </summary>
        public double Volume {
            get => isMuted ? 0.0f : volume;
            set {
                if (value <= 0.0f) {
                    isMuted = true;
                    //volume = 0.0f;
                } else {
                    isMuted = false;
                    if (value > 1.0f) {
                        volume = 1.0f;
                    } else {
                        volume = value;
                    }
                }
                mediaPlayer.Volume = Volume;
            }
        }

        /// <summary>
        /// Current muted state of the <see cref="AudioPlayer"/>.
        /// </summary>
        public bool IsMuted {
            get => isMuted;
            set {
                isMuted = value;
                mediaPlayer.Volume = Volume;
            }
        }

        /// <summary>
        /// <c>true</c> if the <see cref="AudioPlayer"/> currently has media mounted.
        /// </summary>
        /// <seealso cref="CurrentMedia"/>
        public bool HasMedia => currentMedia != null;

        /// <summary>
        /// <c>true</c> while the <see cref="AudioPlayer"/> is playing media.
        /// </summary>
        public bool IsPlaying => mediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;

        /// <summary>
        /// Current playback position (in seconds).
        /// </summary>
        /// <seealso cref="PlaybackDuration"/>
        public double PlaybackPosition {
            get => mediaPlayer.PlaybackSession.Position.TotalSeconds;
            set {
                // clamp seek position:
                if (value < 0.0) {
                    value = 0.0;
                } else {
                    double duration = PlaybackDuration;
                    if (value > duration) {
                        value = duration;
                    }
                }
                // set seek position:
                mediaPlayer.PlaybackSession.Position = TimeSpan.FromSeconds(value);
            }
        }

        /// <summary>
        /// Duration of the <see cref="CurrentMedia"/> in seconds.
        /// </summary>
        /// <seealso cref="PlaybackPosition"/>
        public double PlaybackDuration => currentMedia?.Duration.TotalSeconds ?? 0.0;

        #endregion

        #region events

        /// <summary>
        /// Invoked when a new <see cref="CurrentMedia"/> is mounted.
        /// </summary>
        public event Action<AudioMedia> OnMediaMounted;

        /// <summary>
        /// Invoked when the current <see cref="CurrentMedia"/> is unmounted.
        /// </summary>
        /// <remarks>
        /// This is always invoked before <see cref="OnMediaMounted"/> is invoked if new media is replacing the <see cref="CurrentMedia"/>.
        /// </remarks>
        public event Action<AudioMedia> OnMediaUnmounted;

        /// <summary>
        /// Invoked when playback for the <see cref="CurrentMedia"/> starts / resumes.
        /// </summary>
        public event Action<AudioMedia> OnMediaPlaybackStart;

        /// <summary>
        /// Invoked when playback for the <see cref="CurrentMedia"/> stops / pauses.
        /// </summary>
        public event Action<AudioMedia> OnMediaPlaybackStop;

        /// <summary>
        /// Invoked every time the <see cref="CurrentMedia"/> <see cref="PlaybackPosition"/> changes / updates.
        /// </summary>
        public event Action<AudioMedia, double, double> OnMediaPlaybackPositionChanged;

        /// <summary>
        /// Invoked when playback for the <see cref="CurrentMedia"/> has completed.
        /// </summary>
        public event Action OnMediaPlaybackComplete;

        #endregion

        #region constructor

        internal AudioPlayer() {
            // initialise class:
            currentMedia = null;
            volume = 1.0;
            isMuted = false;
            // create media player:
            mediaPlayer = new MediaPlayer {
                AudioCategory = MediaPlayerAudioCategory.Media
            };
            mediaPlayer.PlaybackSession.PositionChanged += PlaybackSessionPositionChanged;
            mediaPlayer.PlaybackSession.PlaybackStateChanged += PlaybackSessionStateChanged;
        }

        #endregion

        #region destructor

        ~AudioPlayer() {
            // dispose of media player:
            mediaPlayer?.Dispose();
        }

        #endregion

        #region logic

        #region PlaybackSessionPositionChanged

        private void PlaybackSessionPositionChanged(MediaPlaybackSession sender, object args) {
            App.QueueRunAsync(() => { OnMediaPlaybackPositionChanged?.Invoke(currentMedia, PlaybackPosition, PlaybackDuration); });
        }

        #endregion

        #region PlaybackSessionStateChanged

        private void PlaybackSessionStateChanged(MediaPlaybackSession sender, object args) {
            if (mediaPlayer.Source == null) return;
            MediaPlaybackSession session = mediaPlayer.PlaybackSession;
            if (session.PlaybackState == MediaPlaybackState.Paused && session.Position >= session.NaturalDuration) { // complete
                OnMediaPlaybackComplete?.Invoke();
            }
        }

        #endregion

        #region MountNewMedia

        /// <summary>
        /// Sets the <see cref="currentMedia"/> to the <paramref name="nextMedia"/>.
        /// </summary>
        private void MountNewMedia(AudioMedia nextMedia) {
            // unmount current media:
            if (currentMedia != null) {
                App.QueueRunAsync(() => { OnMediaUnmounted?.Invoke(currentMedia); });
            }
            // mount new media:
            currentMedia = nextMedia;
            if (currentMedia != null) {
                App.QueueRunAsync(() => { OnMediaMounted?.Invoke(currentMedia); });
            }
            MountCurrentMediaToPlayer();
        }

        #endregion

        #region MountCurrentMediaToPlayer

        /// <summary>
        /// Mounts the <see cref="currentMedia"/> to the <see cref="mediaPlayer"/>.
        /// </summary>
        private void MountCurrentMediaToPlayer() {
            // check if any media is currently mounted:
            if (currentMedia == null) {
                mediaPlayer.Source = null;
                return;
            }
            // find the uri for the current media:
            string currentMediaPath = currentMedia.Path;
            Uri currentMediaUri = new Uri(currentMediaPath);
            // create and mount a media source based on the uri of the current media:
            MediaSource mediaSource = MediaSource.CreateFromUri(currentMediaUri);
            mediaPlayer.Source = mediaSource;
        }

        #endregion

        #region Play

        /// <summary>
        /// Begins / resumes playback of the current media.
        /// </summary>
        public void Play() {
            if (currentMedia != null) { // there is currently media mounted
                mediaPlayer.Play(); // resume playback of current media
                App.QueueRunAsync(() => { OnMediaPlaybackStart?.Invoke(currentMedia); });
            } else { // there is not any media currently mounted
                mediaPlayer.Pause(); // stop any playback
                mediaPlayer.Source = null; // remove the media source
            }
        }

        /// <summary>
        /// Starts playing the specified <paramref name="audio"/>.
        /// </summary>
        public void Play(in AudioMedia audio) {
            if (audio == null) Stop();
            MountNewMedia(audio);
            Play();
        }

        #endregion

        #region Stop

        /// <summary>
        /// Stops the current audio being played.
        /// </summary>
        public void Stop() {
            if (currentMedia != null) {
                App.QueueRunAsync(() => { OnMediaPlaybackStop?.Invoke(currentMedia); });
            }
            mediaPlayer.Pause(); // stop playback of current media
            MountNewMedia(null);
        }

        #endregion

        #region Pause

        /// <summary>
        /// Pauses the current audio being played.
        /// </summary>
        public void Pause() {
            if (currentMedia != null) {
                App.QueueRunAsync(() => { OnMediaPlaybackStop?.Invoke(currentMedia); });
            }
            mediaPlayer.Pause();
        }

        #endregion

        #endregion

    }

}
