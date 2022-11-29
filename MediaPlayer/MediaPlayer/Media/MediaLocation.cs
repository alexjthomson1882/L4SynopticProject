using System;

namespace MusicPlayer.Media {

    public sealed class MediaLocation : IDisposable {

        #region variables

        private readonly int id;

        private readonly string path;

        private readonly DateTime dateAdded;

        #endregion

        #region property

        public string Path => path;
        public DateTime DateAdded => dateAdded;
        public string DateAddedFormatted => dateAdded.ToString("hh:mm:ss dd-MM-yyyy");

        #endregion

        #region constructor

        internal MediaLocation(in int id, in string path, in DateTime dateAdded) {
            this.id = id;
            this.path = path;
            this.dateAdded = dateAdded;
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
