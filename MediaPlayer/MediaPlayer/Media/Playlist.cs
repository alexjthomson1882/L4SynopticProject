using System;
using System.Collections.Generic;

namespace MusicPlayer.Media {

    public sealed class Playlist {

        #region variable

        private readonly int id;

        private string name;

        private Dictionary<int, AudioMedia> mediaDictionary;

        private readonly DateTime dateAdded;

        #endregion

        #region property

        public int Id {
            get => id;
        }

        public string Name {
            get => name;
            set => name = value;
        }

        public int TotalItems {
            get => mediaDictionary.Count;
        }

        public DateTime DateAdded {
            get => dateAdded;
        }

        public string DateAddedFormatted {
            get => dateAdded.ToString(@"hh\:mm\:ss dd-MM-yyyy");
        }

        #endregion

        #region constructor

        internal Playlist(in int id, in string name, in DateTime dateAdded, in Dictionary<int, AudioMedia> media) {
            this.id = id;
            this.name = name;
            this.dateAdded = dateAdded;
            this.mediaDictionary = media;
        }

        #endregion

        #region logic

        public List<AudioMedia> GetAudioMediaList() => new List<AudioMedia>(mediaDictionary.Values);

        public void Register(in AudioMedia media) {
            if (media == null) throw new ArgumentNullException(nameof(media));
            mediaDictionary[media.Id] = media;
        }

        public void Unregister(in AudioMedia media) {
            if (media == null) throw new ArgumentNullException(nameof(media));
            mediaDictionary.Remove(media.Id);
        }

        public bool Contains(in AudioMedia media) {
            if (media == null) throw new ArgumentNullException(nameof(media));
            return mediaDictionary.ContainsKey(media.Id);
        }

        #endregion

    }

}
