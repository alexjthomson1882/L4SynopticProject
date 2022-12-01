using MusicPlayer.Media;

using System;

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
        /// <see cref="AudioSchedule"/> used to schedule audio for the <see cref="audioPlayer"/> to play.
        /// </summary>
        private readonly AudioSchedule audioSchedule;

        #endregion

        #region property

        public bool IsPlaying {
            get => audioPlayer.IsPlaying;
        }

        public bool HasMedia {
            get => audioSchedule.HasMedia;
        }

        public bool IsMuted {
            get => audioPlayer.IsMuted;
            set => audioPlayer.IsMuted = value;
        }

        public double Volume {
            get => audioPlayer.Volume;
            set => audioPlayer.Volume = value;
        }

        public AudioMedia CurrentMedia {
            get => audioSchedule.CurrentMedia;
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
            // create audio schedule:
            audioSchedule = new AudioSchedule();
        }

        #endregion

        #region logic

        #region Play

        /// <summary>
        /// Play <paramref name="audio"/> immediatly. This will add the <paramref name="audio"/> to the <see cref="audioSchedule"/> ahead of any
        /// <see cref="CurrentMedia"/>.
        /// </summary>
        /// <param name="audio"></param>
        public void Play(in AudioMedia audio) {
            bool wasPlaying = audioPlayer.IsPlaying;
            audioSchedule.PlayImmediatly(audio);
            audioPlayer.Play(audio);
            if (wasPlaying) {
                audioPlayer.Play();
            } else {
                audioPlayer.Pause();
            }
        }

        #endregion

        #region ClearSchedule

        public void ClearSchedule() {
            audioSchedule.Clear();
        }

        #endregion

        #region Resume

        public void Resume() {
            if (!audioPlayer.HasMedia) {
                AudioMedia audioMedia = audioSchedule.CurrentMedia;
                if (audioMedia != null) {
                    audioPlayer.Play(audioMedia);
                }
            }
            audioPlayer.Play();
        }

        #endregion

        #region Pause

        public void Pause() {
            audioPlayer.Pause();
        }

        #endregion

        #region Next

        public void Next() {
            AudioMedia audioMedia = audioSchedule.Next();
            audioPlayer.Play(audioMedia);
        }

        #endregion

        #region Last

        public void Last() {
            AudioMedia audioMedia = audioSchedule.Last();
            audioPlayer.Play(audioMedia);
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