using MusicPlayer.Utility;

using System;
using System.Collections.Generic;

namespace MusicPlayer.Media {

    public sealed class ShuffleMediaPicker : IMediaPicker {

        #region variable

        /// <summary>
        /// <see cref="IList{T}"/> of <see cref="AudioMedia"/> to loop.
        /// </summary>
        private readonly IList<AudioMedia> mediaList;

        /// <summary>
        /// Pre-shuffled version of the <see cref="mediaList"/>.
        /// </summary>
        private readonly IList<AudioMedia> shuffledList;

        /// <summary>
        /// Should the <see cref="mediaList"/> be repeated or not.
        /// </summary>
        private readonly bool repeat;

        /// <summary>
        /// Current index within the <see cref="mediaList"/>.
        /// </summary>
        private int currentIndex;

        #endregion

        #region property

        /// <inheritdoc cref="mediaList"/>
        public IList<AudioMedia> MediaList => mediaList;

        /// <summary>
        /// Current <see cref="AudioMedia"/> selected by the <see cref="ShuffleMediaPicker"/>.
        /// </summary>
        public AudioMedia Current => shuffledList[currentIndex];

        #endregion

        #region constructor

        internal ShuffleMediaPicker(in IList<AudioMedia> mediaList, in int startIndex, in bool repeat) {
            if (mediaList == null) throw new ArgumentNullException(nameof(mediaList));
            if (startIndex < 0 || startIndex >= mediaList.Count) throw new ArgumentOutOfRangeException(nameof(startIndex));
            this.mediaList = mediaList;
            shuffledList = new List<AudioMedia>(mediaList);
            ListUtility.Shuffle(shuffledList);
            AudioMedia target = mediaList[startIndex];
            currentIndex = shuffledList.IndexOf(target);
            this.repeat = repeat;
        }

        #endregion

        #region logic

        public bool MoveLast() {
            if (currentIndex > 0 && currentIndex <= shuffledList.Count) { // no need to loop, move back one space
                currentIndex--;
                return true;
            } else if (repeat) { // loop
                currentIndex = shuffledList.Count - 1;
                return true;
            } else { // do not loop (unable), no where left to move to (stuck at start)
                currentIndex = 0;
                return false;
            }
        }

        public bool MoveNext() {
            if (currentIndex >= 0 && currentIndex < shuffledList.Count - 1) { // no need to loop, move forwards one space
                currentIndex++;
                return true;
            } else if (repeat) { // loop
                currentIndex = 0;
                return true;
            } else { // do not loop (unable), no where left to move to (stuck at end)
                currentIndex = MediaList.Count - 1;
                return false;
            }
        }

        #endregion

    }

}