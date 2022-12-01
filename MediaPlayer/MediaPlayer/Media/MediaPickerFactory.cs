using System;
using System.Collections.Generic;

namespace MusicPlayer.Media {

    /// <summary>
    /// Creates <see cref="IMediaPicker"/> instances based on a set of requirements.
    /// </summary>
    public static class MediaPickerFactory {

        #region logic

        public static IMediaPicker CreateMediaPicker(in IList<AudioMedia> mediaList, in int startIndex, in bool shuffle, in MediaPickerRepeatMode playbackRepeatMode) {
            if (mediaList == null) throw new ArgumentNullException(nameof(mediaList));
            if (startIndex < 0 || startIndex >= mediaList.Count) throw new ArgumentOutOfRangeException(nameof(startIndex));
            switch (playbackRepeatMode) {
                case MediaPickerRepeatMode.RepeatTrack: {
                    return new SingleMediaPicker(mediaList[startIndex], true);
                }
                case MediaPickerRepeatMode.RepeatAll: {
                    if (shuffle) {
                        return new ShuffleMediaPicker(mediaList, startIndex, true);
                    } else {
                        return new SequentialMediaPicker(mediaList, startIndex, true);
                    }
                }
                case MediaPickerRepeatMode.None: {
                    if (shuffle) {
                        return new ShuffleMediaPicker(mediaList, startIndex, false);
                    } else {
                        return new SequentialMediaPicker(mediaList, startIndex, false);
                    }
                }
                default: throw new NotSupportedException(playbackRepeatMode.ToString());
            }
        }

        #endregion

    }

}