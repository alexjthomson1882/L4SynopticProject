using System.Collections.Generic;

namespace MusicPlayer.Media {

    /// <summary>
    /// Implements basic functions to pick media from a source.
    /// </summary>
    public interface IMediaPicker {

        #region property

        IList<AudioMedia> MediaList { get; }

        AudioMedia Current { get; }

        #endregion

        #region logic

        bool MoveNext();

        bool MoveLast();

        #endregion

    }

}