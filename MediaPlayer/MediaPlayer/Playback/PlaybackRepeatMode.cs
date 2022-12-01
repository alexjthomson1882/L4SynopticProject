namespace MusicPlayer.Playback {
    
    /// <summary>
    /// Describes the type of repeat to apply to an <see cref="AudioSchedule"/>.
    /// </summary>
    public enum PlaybackRepeatMode {

        /// <summary>
        /// No repeating is applied.
        /// </summary>
        None = 0,

        /// <summary>
        /// The current track is repeated once.
        /// </summary>
        RepeatTrackOnce = 1,

        /// <summary>
        /// The current track is repeated infinitely.
        /// </summary>
        RepeatTrack = 2,

        /// <summary>
        /// The current schedule is repeated. This will not respect the original order of songs in shuffle mode but will repeat each song.
        /// </summary>
        RepeatSchedule = 3,

    }

}
