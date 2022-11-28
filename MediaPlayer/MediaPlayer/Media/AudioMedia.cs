using System;

namespace MediaPlayer.Media {

    public sealed class AudioMedia {

        #region variable

        private readonly int id;

        private string name;

        private string path;

        #endregion

        #region property

        public string Name {
            get => name;
            set => name = value;
        }

        public string Path {
            get => path;
            set => path = value;
        }

        #endregion

        #region constructor

        internal AudioMedia(in int id, in string name, in string path) {
            this.id = id;
            this.name = name;
            this.path = path;
        }

        #endregion

    }

}
