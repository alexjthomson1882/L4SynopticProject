using System;
using System.Collections.Generic;

namespace MusicPlayer.Media {

    /// <summary>
    /// Selects <see cref="AudioMedia"/> from an ordered list.
    /// </summary>
    public sealed class SequentialMediaPicker : IMediaPicker {

        #region variable

        /// <summary>
        /// <see cref="IList{T}"/> of <see cref="AudioMedia"/> to loop.
        /// </summary>
        private readonly IList<AudioMedia> mediaList;

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
        /// Current <see cref="AudioMedia"/> selected by the <see cref="SequentialMediaPicker"/>.
        /// </summary>
        public AudioMedia Current => mediaList[currentIndex];

        #endregion

        #region constructor

        internal SequentialMediaPicker(in IList<AudioMedia> mediaList, in int startIndex, in bool repeat) {
            if (mediaList == null) throw new ArgumentNullException(nameof(mediaList));
            if (startIndex < 0 || startIndex >= mediaList.Count) throw new ArgumentOutOfRangeException(nameof(startIndex));
            this.mediaList = mediaList;
            this.repeat = repeat;
            currentIndex = startIndex;
        }

        #endregion

        #region logic

        public bool MoveLast() {
            if (currentIndex > 0 && currentIndex <= mediaList.Count) { // no need to loop, move back one space
                currentIndex--;
                return true;
            } else if (repeat) { // loop
                currentIndex = mediaList.Count - 1;
                return true;
            } else { // do not loop (unable), no where left to move to (stuck at start)
                currentIndex = 0;
                return false;
            }
        }

        public bool MoveNext() {
            if (currentIndex >= 0 && currentIndex < mediaList.Count - 1) { // no need to loop, move forwards one space
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