using MusicPlayer.Media;

using System;

using Windows.Media.Core;
using Windows.Media.Playback;

namespace MusicPlayer {

    /// <summary>
    /// Plays audio.
    /// </summary>
    public sealed class AudioPlayer {

        #region variable

        /// <summary>
        /// Volume that audio should be played at. This is a linear scale from <c>0.0</c> to <c>1.0</c>.
        /// </summary>
        /// <seealso cref="Volume"/>
        private float volume = 1.0f;

        /// <summary>
        /// Tracks if the <see cref="AudioPlayer"/> should be muted.
        /// </summary>
        /// <seealso cref="IsMuted"/>
        private bool isMuted = false;

        /// <summary>
        /// <see cref="MediaPlayer"/> used to play audio.
        /// </summary>
        private MediaPlayer mediaPlayer = null;

        #endregion

        #region property

        public float Volume {
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

        public bool IsMuted {
            get => isMuted;
            set {
                isMuted = value;
                mediaPlayer.Volume = Volume;
            }
        }

        #endregion

        #region constructor

        internal AudioPlayer() {
            mediaPlayer = new MediaPlayer {
                AudioCategory = MediaPlayerAudioCategory.Media
            };
        }

        #endregion

        #region destructor

        ~AudioPlayer() {
            mediaPlayer?.Dispose();
        }

        #endregion

        #region logic

        #region Play

        public void Play() {
            if (mediaPlayer.Source == null) {
                Stop();
            } else {
                mediaPlayer.Play();
            }
        }

        /// <summary>
        /// Starts playing the specified <paramref name="audio"/>.
        /// </summary>
        public void Play(in AudioMedia audio) {
            if (audio == null) Stop();
            mediaPlayer.Pause();
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri(audio.Path));
            mediaPlayer.Play();
        }

        #endregion

        #region Stop

        /// <summary>
        /// Stops the current audio being played.
        /// </summary>
        public void Stop() {
            mediaPlayer.Pause();
            mediaPlayer.Source = null;
        }

        #endregion

        #region Pause

        public void Pause() {
            mediaPlayer.Pause();
        }

        #endregion

        #endregion

    }

}
