using MediaPlayer.Data;

using Microsoft.Data.Sqlite;

using System;
using System.Collections.Generic;
using System.IO;

namespace MediaPlayer.Media {

    public sealed class MediaManager {

        #region constant

        /// <summary>
        /// Supported file extensions.
        /// </summary>
        public static HashSet<string> SupportedFileExtensions = new HashSet<string>() {
            ".mp3",
            ".wav",
            ".ogg"
        };

        #endregion

        #region variables

        public readonly Dictionary<int, MediaLocation> trackedMediaLocations;

        public readonly Dictionary<int, AudioMedia> trackedMedia;

        public readonly Dictionary<int, Playlist> trackedPlaylists;

        #endregion

        #region property

        #endregion

        #region constructor

        internal MediaManager() {
            // initialise class:
            trackedMediaLocations = new Dictionary<int, MediaLocation>();
            trackedMedia = new Dictionary<int, AudioMedia>();
            trackedPlaylists = new Dictionary<int, Playlist>();
            // reload:
            ReloadMediaLocations();
            ReloadLibrary();
            ReloadPlaylists();
        }

        #endregion

        #region logic

        #region ReloadMediaLocations

        /// <summary>
        /// Reloads the tracked media locations from the media database.
        /// </summary>
        public void ReloadMediaLocations() {
            // create a list of tracked media locations:
            List<int> trackedLocations = new List<int>();
            foreach (int id in trackedMediaLocations.Keys) { trackedLocations.Add(id); }
            // connect to database:
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                // create transaction:
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    // query the media locations:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = "SELECT * FROM MediaLocations";
                        SqliteDataReader query = command.ExecuteReader();
                        // process query result set:
                        while (query.Read()) {
                            // read media location data:
                            int id = query.GetInt32(0);
                            string path = query.GetString(1);
                            // update media location object:
                            if (trackedMediaLocations.TryGetValue(id, out MediaLocation mediaLocation)) {
                                mediaLocation.Path = path;
                                trackedLocations.Remove(id);
                            } else {
                                mediaLocation = new MediaLocation(
                                    id,
                                    path
                                );
                                trackedMediaLocations[id] = mediaLocation;
                            }
                        }
                    }
                }
            }
            // remove untracked media locations:
            for (int i = trackedLocations.Count - 1; i >= 0; i--) {
                int id = trackedLocations[i];
                if (trackedMediaLocations.Remove(id, out MediaLocation mediaLocation)) {
                    mediaLocation.Dispose();
                }
            }
        }

        #endregion

        #region ReloadLibrary

        /// <summary>
        /// Reloads all of the media in the media database.
        /// </summary>
        public void ReloadLibrary() {
            // find the paths to media on the system:
            List<string> mediaPaths = new List<string>();
            foreach (MediaLocation mediaLocation in trackedMediaLocations.Values) {
                foreach (string file in Directory.EnumerateFiles(mediaLocation.Path, "*.*", SearchOption.AllDirectories)) {
                    // check file extension:
                    string fileExtension = Path.GetExtension(file);
                    if (!SupportedFileExtensions.Contains(fileExtension)) continue;
                    // add to media paths:
                    mediaPaths.Add(Path.GetFullPath(file).Replace('\\', '/'));
                }
            }
            // connect to database:
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                // create transaction:
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    // remove tracked media from list:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = "SELECT MediaPath FROM Media";
                        SqliteDataReader query = command.ExecuteReader();
                        while (query.Read()) {
                            string mediaPath = query.GetString(0);
                            if (mediaPaths.Contains(mediaPath)) {
                                mediaPaths.Remove(mediaPath);
                            }
                        }
                    }
                    // create sqlite bulk insert command:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = @"INSERT INTO Media VALUES (NULL, $value, NULL)";
                        SqliteParameter parameter = command.CreateParameter();
                        for (int i = 0; i < mediaPaths.Count; i++) {
                            parameter.Value = mediaPaths[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    // load all tracked media:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = "SELECT * FROM Media";
                        SqliteDataReader query = command.ExecuteReader();
                        while (query.Read()) {
                            // get media data:
                            int id = query.GetInt32(0);
                            string name = query.GetString(1);
                            string path = query.GetString(2);
                            // track media:
                            if (trackedMedia.TryGetValue(id, out AudioMedia media)) {
                                media.Name = name;
                                media.Path = path;
                            } else {
                                trackedMedia[id] = new AudioMedia(
                                    id, name, path
                                );
                            }
                        }
                    }
                    // commit transaction:
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region ReloadPlaylists

        public void ReloadPlaylists() {
            // clear existing playlists:
            trackedPlaylists.Clear();

            // connect to database:
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                // create transaction:
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    // query playlists
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = "SELECT * FROM Playlists";
                        SqliteDataReader query = command.ExecuteReader();
                        // create playlists:
                        while (query.Read()) {
                            // get playlist info:
                            int id = query.GetInt32(0);
                            string playlistName = query.GetString(1);
                            // create media dictionary:
                            Dictionary<int, AudioMedia> mediaList = new Dictionary<int, AudioMedia>();
                            // query database for media:
                            using (SqliteCommand mediaCommand = connection.CreateCommand()) {
                                mediaCommand.CommandText = "SELECT MediaId FROM PlaylistMediaMap WHERE PlaylistId={id}";
                                mediaCommand.Parameters.AddWithValue("id", id);
                                SqliteDataReader mediaQuery = mediaCommand.ExecuteReader();
                                while (mediaQuery.Read()) {
                                    id = mediaQuery.GetInt32(0);
                                    if (trackedMedia.TryGetValue(id, out AudioMedia media)) {
                                        mediaList[id] = media;
                                    }
                                }
                            }
                            // create playlist object:
                            Playlist playlist = new Playlist(id, playlistName, mediaList);
                            // track playlist:
                            trackedPlaylists[id] = playlist;
                        }
                    }
                    // commit transaction:
                    transaction.Commit();
                }
            }
        }

        #endregion

        #endregion

    }

}
