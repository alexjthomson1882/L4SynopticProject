using System;
using System.Collections.Generic;

namespace MediaPlayer.Media {

    public sealed class Playlist {

        #region variable

        private readonly int id;

        private string name;

        private Dictionary<int, AudioMedia> media;

        #endregion

        #region constructor

        internal Playlist(in int id, in string name, in Dictionary<int, AudioMedia> media) {
            this.id = id;
            this.name = name;
            this.media = media;
        }

        #endregion

    }

}
