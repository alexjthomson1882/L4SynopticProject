using System;
using System.Collections.Generic;

namespace MusicPlayer.Media {

    /// <summary>
    /// Media picker only capable of picking a single track.
    /// </summary>
    public sealed class SingleMediaPicker : IMediaPicker {

        #region variable

        private readonly AudioMedia audioMedia;

        private readonly bool repeat;

        #endregion

        #region property

        public IList<AudioMedia> MediaList { get; private set; }

        public AudioMedia Current => audioMedia;

        #endregion

        #region constructor

        internal SingleMediaPicker(in AudioMedia audioMedia, in bool repeat) {
            if (audioMedia == null) throw new ArgumentNullException(nameof(audioMedia));
            this.audioMedia = audioMedia;
            MediaList = new List<AudioMedia>(1);
            MediaList.Add(audioMedia);
            this.repeat = repeat;
        }

        #endregion

        #region logic

        public bool MoveLast() { return repeat; }

        public bool MoveNext() { return repeat; }

        #endregion

    }

}
