﻿using MusicPlayer.Data;

using Microsoft.Data.Sqlite;

using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;

namespace MusicPlayer.Media {

    /// <summary>
    /// Manages interactions between the application and media database.
    /// </summary>
    public sealed class MediaManager {

        #region constant

        /// <summary>
        /// Supported file extensions.
        /// </summary>
        public static HashSet<string> SupportedFileExtensions = new HashSet<string>() {
            ".mp3", ".wav", ".ogg", ".flac", ".avi", ".wmv", ".m4a"
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
        }

        #endregion

        #region logic

        #region Initialise

        public async void Initialise() {
            StorageLibrary musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            foreach (StorageFolder folder in musicLibrary.Folders) {
                RegisterScanLocation(folder, false);
            }
            // reload:
            ReloadMediaLocations();
            ReloadLibrary();
            ReloadPlaylists();
        }

        #endregion

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
                        using (SqliteDataReader query = command.ExecuteReader()) {
                            // process query result set:
                            while (query.Read()) {
                                // read media location data:
                                int id = query.GetInt32(0);
                                string path = query.GetString(1);
                                DateTime dateAdded = query.GetDateTime(2);
                                // update media location object:
                                if (trackedMediaLocations.TryGetValue(id, out MediaLocation mediaLocation)) {
                                    trackedLocations.Remove(id);
                                } else {
                                    mediaLocation = new MediaLocation(
                                        id, path, dateAdded
                                    );
                                    trackedMediaLocations[id] = mediaLocation;
                                }
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
                try {
                    foreach (string file in Directory.EnumerateFiles(mediaLocation.Path, "*.*", SearchOption.AllDirectories)) {
                        // check file extension:
                        string fileExtension = Path.GetExtension(file);
                        if (!SupportedFileExtensions.Contains(fileExtension)) continue;
                        // add to media paths:
                        mediaPaths.Add(Path.GetFullPath(file).Replace('\\', '/'));
                    }
                } catch (UnauthorizedAccessException) {
                    continue;
                } catch (DirectoryNotFoundException) {
                    continue;
                } catch (FileNotFoundException) {
                    continue;
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
                        command.CommandText = @"INSERT INTO Media (MediaId, MediaName, MediaPath, DateAdded) VALUES (NULL, $name, $path, CURRENT_TIMESTAMP)";
                        SqliteParameter mediaName = command.CreateParameter();
                        mediaName.ParameterName = "name";
                        command.Parameters.Add(mediaName);
                        SqliteParameter mediaPath = command.CreateParameter();
                        mediaPath.ParameterName = "path";
                        command.Parameters.Add(mediaPath);
                        for (int i = 0; i < mediaPaths.Count; i++) {
                            string path = mediaPaths[i];
                            mediaName.Value = Path.GetFileNameWithoutExtension(path).Replace('_', ' ');
                            mediaPath.Value = path;
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
                            DateTime dateAdded = query.GetDateTime(3);
                            // track media:
                            if (!trackedMedia.TryGetValue(id, out AudioMedia media)) {
                                media = new AudioMedia(
                                    id, name, path, dateAdded
                                );
                                trackedMedia[id] = media;
                            }
                            media.Reload();
                        }
                    }
                    // commit transaction:
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region RemoveAudioMediaFromLibrary

        public void RemoveAudioMediaFromLibrary(in AudioMedia audioMedia) {
            if (audioMedia == null) throw new ArgumentNullException(nameof(audioMedia));
            int mediaId = audioMedia.ID;
            if (!trackedMedia.ContainsKey(mediaId)) return; // already not tracked
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = @"DELETE FROM Media WHERE MediaId=$mediaId";
                        command.Parameters.AddWithValue("mediaId", mediaId);
                        command.ExecuteNonQuery();
                        command.CommandText = @"DELETE FROM PlaylistMediaMap WHERE MediaId=$mediaId";
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    // remove from tracked media:
                    trackedMedia.Remove(mediaId);
                    // remove from tracked playlists:
                    foreach (Playlist playlist in trackedPlaylists.Values) {
                        playlist.Unregister(audioMedia);
                    }
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
                        using (SqliteDataReader query = command.ExecuteReader()) {
                            // create playlists:
                            while (query.Read()) {
                                // get playlist info:
                                int id = query.GetInt32(0);
                                string playlistName = query.GetString(1);
                                DateTime dateAdded = query.GetDateTime(2);
                                // create media dictionary:
                                Dictionary<int, AudioMedia> mediaList = new Dictionary<int, AudioMedia>();
                                // query database for media:
                                using (SqliteCommand mediaCommand = connection.CreateCommand()) {
                                    mediaCommand.CommandText = @"SELECT MediaId FROM PlaylistMediaMap WHERE PlaylistId=$id";
                                    mediaCommand.Parameters.AddWithValue("id", id);
                                    using (SqliteDataReader mediaQuery = mediaCommand.ExecuteReader()) {
                                        while (mediaQuery.Read()) {
                                            id = mediaQuery.GetInt32(0);
                                            if (trackedMedia.TryGetValue(id, out AudioMedia media)) {
                                                mediaList[id] = media;
                                            }
                                        }
                                    }
                                }
                                // create playlist object:
                                Playlist playlist = new Playlist(id, playlistName, dateAdded, mediaList);
                                // track playlist:
                                trackedPlaylists[id] = playlist;
                            }
                        }
                    }
                    // commit transaction:
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region CreatePlaylist

        /// <summary>
        /// Creates a new playlist with the name <paramref name="playlistName"/>.
        /// </summary>
        public Playlist CreatePlaylist(in string playlistName) {
            if (playlistName == null) throw new ArgumentNullException(nameof(playlistName));
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    using (SqliteCommand command = connection.CreateCommand()) {
                        // set playlist into database:
                        command.CommandText = @"INSERT INTO Playlists (PlaylistId, PlaylistName, DateAdded) VALUES (NULL, $name, CURRENT_TIMESTAMP)";
                        command.Parameters.AddWithValue("name", playlistName);
                        command.ExecuteNonQuery();
                        // find playlist id:
                        command.CommandText = @"SELECT last_insert_rowid()";
                        int playlistId = (int)(long)command.ExecuteScalar();
                        // find date added:
                        command.CommandText = @"SELECT DateAdded FROM Playlists WHERE PlaylistId=$id";
                        command.Parameters.AddWithValue("id", playlistId);
                        using (SqliteDataReader query = command.ExecuteReader()) {
                            if (!query.Read()) return null;
                            // get date added:
                            DateTime dateAdded = query.GetDateTime(0);
                            // commit transaction:
                            transaction.Commit();
                            // create playlist:
                            Playlist playlist = new Playlist(playlistId, playlistName, dateAdded, new Dictionary<int, AudioMedia>());
                            // track new playlist:
                            trackedPlaylists[playlistId] = playlist;
                            // return new playlist:
                            return playlist;
                        }
                    }
                }
            }
        }

        #endregion

        #region RenamePlaylist

        /// <summary>
        /// Renames a <paramref name="playlist"/> to <paramref name="playlistName"/>.
        /// </summary>
        public void RenamePlaylist(in Playlist playlist, in string playlistName) {
            if (playlistName == null) throw new ArgumentNullException(nameof(playlistName));
            if (playlist.Name == playlistName) return; // already has target name
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = @"UPDATE Playlists SET PlaylistName=$name WHERE PlaylistId=$id";
                        playlist.Name = playlistName;
                        command.Parameters.AddWithValue("id", playlist.Id);
                        command.Parameters.AddWithValue("name", playlist.Name);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region DeletePlaylist

        public void DeletePlaylist(in Playlist playlist) {
            if (playlist == null) throw new ArgumentNullException(nameof(playlist));
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = @"DELETE FROM Playlists WHERE PlaylistId=$id";
                        command.Parameters.AddWithValue("id", playlist.Id);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
            trackedPlaylists.Remove(playlist.Id);
        }

        #endregion

        #region AddAudioMediaToPlaylist

        public void AddAudioMediaToPlaylist(in AudioMedia audioMedia, in Playlist playlist) {
            if (audioMedia == null) throw new ArgumentNullException(nameof(audioMedia));
            if (playlist == null) throw new ArgumentNullException(nameof(playlist));
            if (playlist.Contains(audioMedia)) return; // already added to playlist
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = @"INSERT INTO PlaylistMediaMap (PlaylistId, MediaId) VALUES ($playlistId, $mediaId)";
                        command.Parameters.AddWithValue("playlistId", playlist.Id);
                        command.Parameters.AddWithValue("mediaId", audioMedia.ID);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    playlist.Register(audioMedia);
                }
            }
        }

        #endregion

        #region RemoveAudioMediaFromPlaylist

        public void RemoveAudioMediaFromPlaylist(in AudioMedia audioMedia, in Playlist playlist) {
            if (audioMedia == null) throw new ArgumentNullException(nameof(audioMedia));
            if (playlist == null) throw new ArgumentNullException(nameof(playlist));
            if (!playlist.Contains(audioMedia)) return; // already removed from playlist
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = @"DELETE FROM PlaylistMediaMap WHERE PlaylistId=$playlistId AND MediaId=$mediaId";
                        command.Parameters.AddWithValue("playlistId", playlist.Id);
                        command.Parameters.AddWithValue("mediaId", audioMedia.ID);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    playlist.Unregister(audioMedia);
                }
            }
        }

        #endregion

        #region GetMediaList

        /// <summary>
        /// Constructs a <see cref="List{T}"/> of tracked <see cref="AudioMedia"/> objects.
        /// </summary>
        public List<AudioMedia> GetMediaList() {
            List<AudioMedia> mediaList = new List<AudioMedia>();
            foreach (AudioMedia media in trackedMedia.Values) {
                mediaList.Add(media);
            }
            return mediaList;
        }

        #endregion

        #region GetPlaylistList

        /// <summary>
        /// Constructs a <see cref="List{T}"/> of tracked <see cref="Playlist"/> objects.
        /// </summary>
        public List<Playlist> GetPlaylistList() {
            List<Playlist> playlistList = new List<Playlist>();
            foreach (Playlist playlist in trackedPlaylists.Values) {
                playlistList.Add(playlist);
            }
            return playlistList;
        }

        #endregion

        #region GetMediaLocationsList

        /// <summary>
        /// Constructs a <see cref="List{T}"/> of tracked <see cref="MediaLocation"/> objects.
        /// </summary>
        public List<MediaLocation> GetMediaLocationsList() {
            List<MediaLocation> locationList = new List<MediaLocation>();
            foreach (MediaLocation location in trackedMediaLocations.Values) {
                locationList.Add(location);
            }
            return locationList;
        }

        #endregion

        #region RegisterScanLocation

        public void RegisterScanLocation(StorageFolder location, bool refresh = true) {
            if (location == null) return; // no location added
            string path = location.Path.Replace('\\', '/');
            using (SqliteConnection connection = DataAccess.GetConnection()) {
                connection.Open();
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    // check if directory already exists in database:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = @"SELECT LocationId FROM MediaLocations WHERE LocationPath=$path";
                        command.Parameters.AddWithValue("path", path);
                        using (SqliteDataReader query = command.ExecuteReader()) {
                            if (query.HasRows) return; // already contains target
                        }
                    }
                    // add location:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText = @"INSERT INTO MediaLocations (LocationId, LocationPath, DateAdded) VALUES (NULL, $path, CURRENT_TIMESTAMP)";
                        command.Parameters.AddWithValue("path", path);
                        command.ExecuteNonQuery();
                    }
                    // commit transaction:
                    transaction.Commit();
                }
            }
            if (refresh) {
                ReloadMediaLocations();
                ReloadLibrary();
                ReloadPlaylists();
            }
        }

        #endregion

        #endregion

    }

}
