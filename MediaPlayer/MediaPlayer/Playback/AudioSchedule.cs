using MusicPlayer.Media;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayer.Playback {

    /// <summary>
    /// Manages a single audio schedule.
    /// </summary>
    public sealed class AudioSchedule {

        #region constant

        /// <summary>
        /// Default size of the <see cref="history"/> stack.
        /// </summary>
        public const int DefaultHistoryStackSize = 128;

        #endregion

        #region variable

        /// <summary>
        /// Original schedule.
        /// </summary>
        private readonly List<AudioScheduleInfo> schedule;

        /// <summary>
        /// History queue that tracks audio that has been played.
        /// </summary>
        private readonly Queue<AudioMedia> history;

        /// <summary>
        /// Maxium number of elements that can be contained within the <see cref="history"/> queue.
        /// </summary>
        private readonly int historyCapacity;

        /// <summary>
        /// Current <see cref="AudioScheduleInfo"/> being played.
        /// </summary>
        private AudioScheduleInfo current;

        /// <summary>
        /// Tracks if the <see cref="schedule"/> playback should be shuffled.
        /// </summary>
        private bool shuffle;

        /// <summary>
        /// Tracks the repeat mode for the <see cref="AudioSchedule"/>.
        /// </summary>
        private PlaybackRepeatMode repeatMode;

        /// <summary>
        /// Current layer index. This is used to identify audio that has not yet been scheduled in the current schedule.
        /// </summary>
        private int currentScheduleLayer;

        /// <summary>
        /// Current index within the <see cref="history"/> queue.
        /// </summary>
        /// <remarks>
        /// If this index is <c>-1</c>, it means the <see cref="current"/> audio does not reference anything within the
        /// <see cref="history"/> queue.
        /// </remarks>
        private int currentHistoryIndex;

        #endregion

        #region property

        public bool HasMedia {
            get => current != null;
        }

        public AudioMedia CurrentMedia {
            get => current?.AudioMedia;
        }

        public bool Shuffle {
            get => shuffle;
            set => shuffle = value;
        }

        public PlaybackRepeatMode RepeatMode {
            get => repeatMode;
            set => repeatMode = value;
        }

        #endregion

        #region constructor

        internal AudioSchedule(int historyStackSize = DefaultHistoryStackSize) {
            // validate history stack size:
            if (historyStackSize < 0) historyStackSize = DefaultHistoryStackSize;
            // create schedule and history objects:
            schedule = new List<AudioScheduleInfo>();
            history = new Queue<AudioMedia>(historyStackSize);
            this.historyCapacity = historyStackSize;
            // create default state:
            current = null;
            shuffle = false;
            repeatMode = PlaybackRepeatMode.None;
            currentScheduleLayer = int.MinValue + 1;
            currentHistoryIndex = -1;
        }

        #endregion

        #region logic

        #region Clear

        /// <summary>
        /// Clears the current schedule.
        /// </summary>
        public void Clear() {
            schedule.Clear();
            currentScheduleLayer = int.MinValue + 1;
        }

        #endregion

        #region Next

        public AudioMedia Next() {
            // decrement history index:
            if (currentHistoryIndex != -1 && currentHistoryIndex-- < 0) currentHistoryIndex = -1;
            // check if currently referencing historical media:
            if (currentHistoryIndex != -1) {
                AudioMedia nextMedia = GetHistoryAtIndex(currentHistoryIndex);
                AudioScheduleInfo scheduleInfo = new AudioScheduleInfo(nextMedia, int.MaxValue);
                current = scheduleInfo;
                return nextMedia;
            }
            // return next media:
            current = PickNext();
            return current?.AudioMedia;
        }

        #endregion

        #region Last

        public AudioMedia Last() {
            // get history count:
            int historyCount = history.Count;
            if (historyCount == 0) {
                currentHistoryIndex = -1;
                return current?.AudioMedia;
            }
            // calculate history index:
            if (currentHistoryIndex == -1) { // not in history yet
                currentHistoryIndex = 0;
            } else if (currentHistoryIndex < historyCount - 1) {
                currentHistoryIndex++;
            } else {
                currentHistoryIndex = historyCount - 1;
            }
            // get history at index:
            AudioMedia lastMedia = GetHistoryAtIndex(currentHistoryIndex);
            AudioScheduleInfo scheduleInfo = new AudioScheduleInfo(lastMedia, int.MaxValue);
            current = scheduleInfo;
            return lastMedia;
        }

        #endregion

        #region GetHistoryAtIndex

        /// <summary>
        /// Expensive operation that converts the <see cref="history"/> queue to an array and finds an <see cref="AudioMedia"/> instance
        /// at a specified <paramref name="index"/>.
        /// </summary>
        private AudioMedia GetHistoryAtIndex(int index) {
            // get history count:
            int historyLength = history.Count;
            if (historyLength == 0) return null;
            // get history array:
            AudioMedia[] historyArray = history.ToArray();
            // clamp index:
            if (index < 0) index = 0;
            else if (index >= historyLength) index = historyLength - 1;
            return historyArray[index];
        }

        #endregion

        #region EnqueueAudio

        public void EnqueueAudio(in AudioMedia audio) {
            if (audio == null) throw new ArgumentNullException(nameof(audio));
            AudioScheduleInfo info = new AudioScheduleInfo(audio, currentScheduleLayer - 1);
            schedule.Add(info); // add the new audio
            if (current == null) {
                info.ScheduleLayer = currentScheduleLayer;
                current = info;
            }
        }

        #endregion

        #region PlayImmediatly

        /// <summary>
        /// Clears the current <see cref="schedule"/> and adds the <paramref name="audio"/> to a new schedule.
        /// </summary>
        public void PlayImmediatly(in AudioMedia audio) {
            if (audio == null) throw new ArgumentNullException(nameof(audio));
            schedule.Clear(); // clear the current schedule
            AudioScheduleInfo info = new AudioScheduleInfo(audio, currentScheduleLayer);
            schedule.Add(info);
            current = info;
        }

        #endregion

        #region AddToHistory

        /// <summary>
        /// Adds the specified <paramref name="audio"/> to the <see cref="history"/> queue.
        /// </summary>
        private void AddToHistory(in AudioMedia audio) {
            if (audio == null) throw new ArgumentNullException(nameof(audio));
            // maintain history queue capacity:
            int historyIndexOffset = 1; // offset required to add to the history index
            while (history.Count >= historyCapacity) {
                history.Dequeue();
                historyIndexOffset++;
            }
            // update current history index:
            if (currentHistoryIndex != -1) {
                currentHistoryIndex += historyIndexOffset;
                if (currentHistoryIndex < 0) {
                    currentHistoryIndex = 0;
                }
            }
            // enqueue new audio to history:
            history.Enqueue(audio);
        }

        #endregion

        #region PickNext

        /// <summary>
        /// Picks the <see cref="next"/> <see cref="AudioScheduleInfo"/> to be scheduled.
        /// </summary>
        private AudioScheduleInfo PickNext() {
            // check if schedule is empty:
            if (schedule.Count == 0) return null;
            // next audio to be played:
            AudioScheduleInfo audio;
            // check for track repeat mode:
            if (repeatMode == PlaybackRepeatMode.RepeatTrackOnce) {
                // get the current audio track to repeat once:
                audio = PickNextRepeat(0);
                // return the current track:
                if (audio == null) return null; // no track found to repeat
                audio.RepeatCount = 1; // track has been repeated once
                return audio;
            } else if (repeatMode == PlaybackRepeatMode.RepeatTrack) {
                // get the current audio track to repeat infinitely:
                audio = PickNextRepeat(int.MaxValue);
                // return the current track:
                if (audio == null) return null; // no track found to repeat
                audio.RepeatCount++; // track has been repeated again
                return audio;
            }
            // apply shuffle:
            if (shuffle) {
                audio = PickNextShuffle(int.MaxValue);
                if (audio == null && repeatMode == PlaybackRepeatMode.RepeatSchedule) {
                    currentScheduleLayer++;
                    audio = PickNextShuffle(int.MaxValue);
                }
                return audio;
            }
            // apply standard play:
            audio = PickNextStandard();
            if (audio == null && repeatMode == PlaybackRepeatMode.RepeatSchedule) {
                currentScheduleLayer++;
                audio = PickNextStandard();
            }
            return audio;
        }

        #endregion

        #region PickNextRepeat

        /// <summary>
        /// Picks the next <see cref="AudioScheduleInfo"/> based on a <see cref="currentScheduleLayer"/> and <paramref name="maxRepeatCount"/>.
        /// </summary>
        /// <param name="maxRepeatCount">
        /// Maximum number of repeats (inclusive) that can be present on a <see cref="AudioScheduleInfo"/> on the current <see cref="currentScheduleLayer"/>.
        /// </param>
        /// <returns>
        /// Returns an <see cref="AudioScheduleInfo"/> instance or <c>null</c> if nothing could be found.
        /// </returns>
        private AudioScheduleInfo PickNextRepeat(in int maxRepeatCount) {
            if (schedule.Count == 0) return null;
            AudioScheduleInfo audio = current;
            if (audio != null && audio.RepeatCount <= maxRepeatCount) {
                return audio;
            }
            // find the next track via shuffle or standard play:
            if (shuffle) {
                // get next shuffled track:
                audio = PickNextShuffle(maxRepeatCount);
                // no track was found, try again on the next schedule layer:
                if (audio == null) {
                    currentScheduleLayer++;
                    audio = PickNextShuffle(maxRepeatCount);
                }
            } else {
                // get the next standard track:
                audio = PickNextStandard();
            }
            // return found audio (if any):
            return audio;
        }

        #endregion

        #region PickNextShuffle

        /// <summary>
        /// Picks the next <see cref="AudioScheduleInfo"/> at random based on audio that has not yet been played on the current <see cref="currentScheduleLayer"/>.
        /// </summary>
        /// <param name="maxRepeatCount">
        /// Maximum number of repeats (inclusive) that can be present on a <see cref="AudioScheduleInfo"/> on the current <see cref="currentScheduleLayer"/>.
        /// </param>
        /// <returns>
        /// Returns an <see cref="AudioScheduleInfo"/> instance or <c>null</c> if nothing could be found.
        /// </returns>
        private AudioScheduleInfo PickNextShuffle(int maxRepeatCount) {
            if (schedule.Count == 0) return null;
            Random random = new Random();
            return schedule.Where(i => i.ScheduleLayer <= currentScheduleLayer && i.RepeatCount < maxRepeatCount).OrderBy(i => random.Next()).First();
        }

        #endregion

        #region PickNextStandard

        /// <summary>
        /// Picks the sequentially next <see cref="AudioScheduleInfo"/> for the current <see cref="currentScheduleLayer"/>.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="AudioScheduleInfo"/> instance or <c>null</c> if nothing could be found.
        /// </returns>
        private AudioScheduleInfo PickNextStandard() {
            if (schedule.Count == 0) return null;
            // iterate the schedule from front to back:
            for (int i = 0; i < schedule.Count; i++) {
                // get the current track:
                AudioScheduleInfo track = schedule[i];
                // check if the track is not yet on the current schedule layer:
                if (track != null && track.ScheduleLayer < currentScheduleLayer) {
                    // update the tracks schedule layer:
                    track.ScheduleLayer = currentScheduleLayer;
                    // return the track:
                    return track;
                }
            }
            // no track was found:
            return null;
        }

        #endregion

        #endregion

    }

}