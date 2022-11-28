using System;

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
            }
        }

        public bool IsMuted {
            get => isMuted;
            set => isMuted = value;
        }

        #endregion

        #region constructor

        #endregion

        #region logic

        #endregion

    }

}
