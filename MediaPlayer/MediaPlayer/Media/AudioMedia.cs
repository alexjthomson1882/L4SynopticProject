using System;
using System.IO;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.FileProperties;

namespace MusicPlayer.Media {

    /// <summary>
    /// Manages and holds data for a single piece of audio media.
    /// </summary>
    public sealed class AudioMedia {

        #region variable

        /// <summary>
        /// Unique ID of the <see cref="AudioMedia"/> within the database.
        /// </summary>
        private readonly int id;
        
        /// <summary>
        /// Fallback name of the <see cref="AudioMedia"/> based on the file name.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Full path to the <see cref="AudioMedia"/> on the system.
        /// </summary>
        private readonly string path;

        /// <summary>
        /// <see cref="DateTime"/> that the <see cref="AudioMedia"/> was added to the database.
        /// </summary>
        private readonly DateTime dateAdded;

        /// <inheritdoc cref="Title"/>
        private string title;

        /// <inheritdoc cref="Album"/>
        private string album;

        /// <inheritdoc cref="Artist"/>
        private string artist;

        #endregion

        #region property

        /// <summary>
        /// Unique ID of the <see cref="AudioMedia"/> within the database.
        /// </summary>
        public int ID => id;

        /// <summary>
        /// Full path to the <see cref="AudioMedia"/> resource.
        /// </summary>
        public string Path => path;

        /// <summary>
        /// Title of the <see cref="AudioMedia"/>.
        /// </summary>
        public string Title {
            get => title;
            private set => title = string.IsNullOrWhiteSpace(value) ? name : value.Trim();
        }

        /// <summary>
        /// Album name of the <see cref="AudioMedia"/>.
        /// </summary>
        public string Album {
            get => album;
            private set => album = string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        /// <summary>
        /// Artist of the <see cref="AudioMedia"/>.
        /// </summary>
        public string Artist {
            get => artist;
            private set => artist = string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        /// <summary>
        /// <see cref="DateTime"/> that the <see cref="AudioMedia"/> was added to the audio database.
        /// </summary>
        public DateTime DateAdded => dateAdded;

        /// <summary>
        /// Formatted string version of the <see cref="DateAdded"/> property.
        /// </summary>
        public string DateAddedFormatted => dateAdded.ToString(@"hh\:mm\:ss dd-MM-yyyy");

        /// <summary>
        /// <see cref="TimeSpan"/> that describes how long the <see cref="AudioMedia"/> content is.
        /// </summary>
        public TimeSpan Duration { get; private set; } = TimeSpan.Zero;

        /// <summary>
        /// Formatted string version of the <see cref="Duration"/> property.
        /// </summary>
        public string DurationFormatted {
            get {
                TimeSpan duration = Duration;
                return duration <= TimeSpan.Zero ? "-" : Duration.ToString(@"mm\:ss");
            }
        }

        #endregion

        #region constructor

        internal AudioMedia(in int id, in string name, in string path, in DateTime dateAdded) {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (path == null) throw new ArgumentNullException(nameof(path));
            this.id = id;
            this.name = name.Trim();
            this.path = path;
            this.dateAdded = dateAdded;
            Title = name;
        }

        #endregion

        #region logic

        #region GetStorageFile

        /// <returns>
        /// Returns a reference to the <see cref="StorageFile"/> that this <see cref="AudioMedia"/> references.
        /// </returns>
        public async Task<StorageFile> GetStorageFile() {
            // get full path to directory containing the media file:
            string folderPath = Directory.GetParent(path).FullName;
            // get the folder object from the folder path:
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            if (folder == null) return null;
            // return the media file from within the folder:
            return await folder.GetFileAsync(System.IO.Path.GetFileName(path));
        }

        #endregion

        #region Reload

        /// <summary>
        /// Reloads the <see cref="AudioMedia"/> meta data.
        /// </summary>
        public async void Reload() {
            // get storage file:
            StorageFile file = await GetStorageFile();
            if (file == null || !file.IsAvailable) return; // failed to find the file
            // get music properties meta-data:
            MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
            if (musicProperties == null) return; // failed to find music properties
            // update music properties:
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
