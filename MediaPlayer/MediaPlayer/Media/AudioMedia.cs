using System;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MusicPlayer.Media {

    public sealed class AudioMedia {

        #region variable

        private readonly int id;

        private readonly string name;

        private readonly string path;

        private readonly DateTime dateAdded;

        private string title;
        private string album;
        private string artist;

        #endregion

        #region property

        public int Id => id;
        public string Path => path;
        public string Title {
            get => title;
            private set => title = string.IsNullOrWhiteSpace(value) ? name : value;
        }
        public string Album {
            get => album;
            private set => album = string.IsNullOrWhiteSpace(value) ? "-" : value;
        }
        public string Artist {
            get => artist;
            private set => artist = string.IsNullOrWhiteSpace(value) ? "-" : value;
        }
        public DateTime DateAdded => dateAdded;
        public string DateAddedFormatted => dateAdded.ToString(@"hh\:mm\:ss dd-MM-yyyy");
        public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
        public string DurationFormatted {
            get {
                TimeSpan duration = Duration;
                return duration <= TimeSpan.Zero ? "-" : Duration.ToString(@"mm\:ss");
            }
        }

        #endregion

        #region constructor

        internal AudioMedia(in int id, in string name, in string path, in DateTime dateAdded) {
            this.id = id;
            this.name = name;
            this.path = path;
            this.dateAdded = dateAdded;
            Title = name;
        }

        #endregion

        #region logic

        #region GetStorageFile

        public async Task<StorageFile> GetStorageFile() {
            string folderPath = Directory.GetParent(path).FullName;
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            return await folder.GetFileAsync(System.IO.Path.GetFileName(path));
        }

        #endregion

        #region Reload

        public async void Reload() {
            StorageFile file = await GetStorageFile();
            MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
            Title = musicProperties.Title;
            Album = musicProperties.Album;
            Artist = musicProperties.Artist;
            Duration = musicProperties.Duration;
        }

        #endregion

        #region ToString

        public sealed override string ToString() => Title;

        #endregion

        #endregion

    }

}
