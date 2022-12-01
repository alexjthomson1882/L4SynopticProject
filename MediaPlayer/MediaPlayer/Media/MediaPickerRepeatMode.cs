namespace MusicPlayer.Media {
    
    /// <summary>
    /// Describes the type of repeat to apply to an <see cref="IMediaPicker"/> instance.
    /// </summary>
    public enum MediaPickerRepeatMode {

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
        /// The current list of media is repeated. This will not respect the original order of songs in shuffle mode but will repeat each song.
        /// </summary>
        RepeatAll = 3,

    }

}
