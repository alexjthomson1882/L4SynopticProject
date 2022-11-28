using System;

namespace MediaPlayer.Media {

    public sealed class MediaLocation : IDisposable {

        #region variables

        private readonly int id;

        private string path;

        #endregion

        #region property

        public string Path {
            get => path;
            set => path = value;
        }

        #endregion

        #region constructor

        internal MediaLocation(in int id, in string path) {
            this.id = id;
            this.path = path;
        }

        #endregion

        #region logic

        #region Dispose

        public void Dispose() {
            
        }

        #endregion

        #endregion

    }

}
