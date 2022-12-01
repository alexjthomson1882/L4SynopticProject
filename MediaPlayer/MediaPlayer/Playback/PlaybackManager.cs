using MusicPlayer.Media;

using System;
using System.Collections.Generic;

namespace MusicPlayer.Playback {

    /// <summary>
    /// Handles and manages audio playback.
    /// </summary>
    public sealed class PlaybackManager {

        #region variable

        /// <summary>
        /// <see cref="AudioPlayer"/> used to play <see cref="AudioMedia"/> by the <see cref="PlaybackManager"/>.
        /// </summary>
        private readonly AudioPlayer audioPlayer;

        /// <summary>
        /// <see cref="IMediaPicker"/> used to pick which <see cref="AudioMedia"/> from a list.
        /// </summary>
        private IMediaPicker mediaPicker;

        private bool shuffle;

        private bool repeat;

        #endregion

        #region property

        public bool IsPlaying {
            get => audioPlayer.IsPlaying;
        }

        public bool HasMedia {
            get => mediaPicker != null && mediaPicker.Current != null;
        }

        public bool IsMuted {
            get => audioPlayer.IsMuted;
            set => audioPlayer.IsMuted = value;
        }

        public bool Shuffle {
            get => shuffle;
            set {
                if (value == shuffle) return; // already has target value
                shuffle = value; // update state
                RegenerateMediaPicker();
            }
        }

        public bool Repeat {
            get => repeat;
            set {
                if (value == repeat) return; // already has target value
                repeat = value; // update state
                RegenerateMediaPicker();
            }
        }

        public double Volume {
            get => audioPlayer.Volume;
            set => audioPlayer.Volume = value;
        }

        public AudioMedia CurrentMedia {
            get => mediaPicker?.Current;
        }

        public double PlaybackPosition {
            get => audioPlayer.PlaybackPosition;
            set => audioPlayer.PlaybackPosition = value;
        }

        public double PlaybackDuration {
            get => audioPlayer.PlaybackDuration;
        }

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

        #endregion

        #region constructor

        internal PlaybackManager() {
            // create audio player:
            audioPlayer = new AudioPlayer();
            audioPlayer.OnMediaMounted += AudioPlayer_OnMediaMounted;
            audioPlayer.OnMediaUnmounted += AudioPlayer_OnMediaUnmounted; ;
            audioPlayer.OnMediaPlaybackStart += AudioPlayer_OnMediaPlaybackStart; ;
            audioPlayer.OnMediaPlaybackStop += AudioPlayer_OnMediaPlaybackStop; ;
            audioPlayer.OnMediaPlaybackPositionChanged += AudioPlayer_OnMediaPlaybackPositionChanged; ;
            // initialise media picker variables:
            mediaPicker = null;
            shuffle = false;
            repeat = false;
        }

        #endregion

        #region logic

        #region Play

        /// <summary>
        /// Play <paramref name="audio"/> immediatly. This will add the <paramref name="audio"/> to the <see cref="audioSchedule"/> ahead of any
        /// <see cref="CurrentMedia"/>.
        /// </summary>
        public void Play(in AudioMedia audio) {
            // change media picker:
            ChangeMediaPicker(
                new List<AudioMedia> { audio }, // audio list
                0, // start index
                false, // shuffle
                repeat ? MediaPickerRepeatMode.RepeatTrack : MediaPickerRepeatMode.None // repeat mode
            );
        }

        public void Play(in IList<AudioMedia> audioList, in int startIndex) {
            if (audioList == null) throw new ArgumentNullException(nameof(audioList));
            int audioCount = audioList.Count;
            if (audioCount == 0) { // no tracks
                ChangeMediaPicker(null, 0, false, MediaPickerRepeatMode.None);
            } else if (audioCount == 1) { // single track
                ChangeMediaPicker(
                    audioList, // audio list
                    0, // start index
                    false, // shuffle
                    repeat ? MediaPickerRepeatMode.RepeatTrack : MediaPickerRepeatMode.None // repeat mode
                );
            } else { // multiple tracks
                ChangeMediaPicker(
                    audioList, // audio list
                    startIndex, // start index
                    shuffle, // shuffle
                    repeat ? MediaPickerRepeatMode.RepeatAll : MediaPickerRepeatMode.None // repeat mode
                );
            }
        }

        #endregion

        #region RegenerateMediaPicker

        private void RegenerateMediaPicker() {
            if (mediaPicker != null && mediaPicker.Current != null) {
                IList<AudioMedia> mediaList = mediaPicker.MediaList;
                ChangeMediaPicker(
                    mediaList,
                    mediaList.IndexOf(mediaPicker.Current),
                    shuffle,
                    !repeat ? MediaPickerRepeatMode.None : mediaList.Count == 1 ? MediaPickerRepeatMode.RepeatTrack : MediaPickerRepeatMode.RepeatAll
                );
            } else {
                ChangeMediaPicker(null, 0, false, MediaPickerRepeatMode.None);
            }
        }

        #endregion

        #region ChangeMediaPicker

        /// <summary>
        /// Changes the current <see cref="mediaPicker"/> based on a set of inputs.
        /// </summary>
        private void ChangeMediaPicker(in IList<AudioMedia> mediaList, in int startIndex, in bool shuffle, in MediaPickerRepeatMode repeatMode) {
            if (mediaList != null) {
                mediaPicker = MediaPickerFactory.CreateMediaPicker(
                    mediaList,
                    startIndex,
                    shuffle,
                    repeatMode
                );
            } else {
                mediaPicker = null; // clear the media picker
            }
            UpdateAudioPlayer();
        }

        #endregion

        #region UpdateAudioPlayer

        /// <summary>
        /// Updates the currently playing <see cref="AudioMedia"/> in the <see cref="audioPlayer"/> based on the current state of the <see cref="mediaPicker"/>.
        /// </summary>
        private bool UpdateAudioPlayer() {
            // null check:
            if (mediaPicker == null) {
                audioPlayer.Stop();
                return false;
            }
            // get current media:
            AudioMedia currentMedia = mediaPicker.Current;
            if (currentMedia == null) {
                audioPlayer.Stop();
                return false;
            }
            // play current media:
            if (audioPlayer.CurrentMedia != currentMedia) {
                audioPlayer.Play(currentMedia);
            }
            return true;
        }

        #endregion

        #region Resume

        public void Resume() {
            audioPlayer.Play();
        }

        #endregion

        #region Pause

        public void Pause() {
            audioPlayer.Pause();
        }

        #endregion

        #region Next

        public bool Next() {
            if (mediaPicker != null && mediaPicker.MoveNext()) {
                return UpdateAudioPlayer();
            }
            audioPlayer.Stop();
            return false;
        }

        #endregion

        #region Last

        public bool Last() {
            if (mediaPicker != null && mediaPicker.MoveLast()) {
                return UpdateAudioPlayer();
            }
            audioPlayer.Stop();
            return false;
        }

        #endregion

        #region AudioPlayer

        private void AudioPlayer_OnMediaMounted(AudioMedia media) {
            OnMediaMounted?.Invoke(media);
        }

        private void AudioPlayer_OnMediaUnmounted(AudioMedia media) {
            OnMediaUnmounted?.Invoke(media);
        }

        private void AudioPlayer_OnMediaPlaybackStart(AudioMedia media) {
            OnMediaPlaybackStart?.Invoke(media);
        }

        private void AudioPlayer_OnMediaPlaybackStop(AudioMedia media) {
            OnMediaPlaybackStop?.Invoke(media);
        }

        private void AudioPlayer_OnMediaPlaybackPositionChanged(AudioMedia media, double position, double duration) {
            OnMediaPlaybackPositionChanged?.Invoke(media, position, duration);
        }

        #endregion

        #endregion

    }

}