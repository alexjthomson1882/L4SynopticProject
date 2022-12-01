using MusicPlayer.Media;

using System;

namespace MusicPlayer.Playback {

    /// <summary>
    /// Tracks scheduling information for a single <see cref="AudioMedia"/> instance.
    /// </summary>
    public sealed class AudioScheduleInfo {

        #region variable

        /// <summary>
        /// <see cref="AudioMedia"/> being tracked.
        /// </summary>
        private readonly AudioMedia audioMedia;

        /// <summary>
        /// Used to identify the layer that the <see cref="audioMedia"/> is on in the current scehdule. This is usually iterated when the <see cref="audioMedia"/> has been
        /// played. Then media that has not yet been played in the current schedule can be identified.
        /// </summary>
        private int scheduleLayer = 0;

        /// <summary>
        /// Number of repeats that the current track has made in the current <see cref="scheduleLayer"/>.
        /// </summary>
        private int repeatCount = 0;

        #endregion

        #region property

        /// <summary>
        /// <see cref="AudioMedia"/> being tracked.
        /// </summary>
        public AudioMedia AudioMedia => audioMedia;

        /// <summary>
        /// Current schedule layer assigned to the <see cref="AudioMedia"/>.
        /// </summary>
        public int ScheduleLayer {
            get => scheduleLayer;
            set {
                if (scheduleLayer == value) return;
                scheduleLayer = value;
                repeatCount = 0;
            }
        }

        /// <summary>
        /// Total number of times that the <see cref="AudioMedia"/> has been repeated in the current <see cref="ScheduleLayer"/>.
        /// </summary>
        /// <remarks>
        /// This is reset to <c>0</c> when the <see cref="ScheduleLayer"/> is changed.
        /// </remarks>
        public int RepeatCount {
            get => repeatCount;
            set => repeatCount = value;
        }

        #endregion

        #region constructor

        internal AudioScheduleInfo(in AudioMedia audioMedia, in int scheduleLayer) {
            if (audioMedia == null) throw new ArgumentNullException(nameof(audioMedia));
            this.audioMedia = audioMedia;
            this.scheduleLayer = scheduleLayer;
            repeatCount = 0;
        }

        #endregion

    }

}
