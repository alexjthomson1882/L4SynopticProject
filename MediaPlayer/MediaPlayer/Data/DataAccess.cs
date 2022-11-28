using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Data.Sqlite;

using Windows.Storage;

namespace MediaPlayer.Data {
    
    public static class DataAccess {

        #region constant

        public const string SqliteDatabaseFileName = "sqlite.db";

        #endregion

        #region variable

        private static string databasePath = null;

        #endregion

        #region logic

        #region InitialiseDatabase

        public static async void InitialiseDatabase() {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            await localFolder.CreateFileAsync(SqliteDatabaseFileName, CreationCollisionOption.OpenIfExists);
            databasePath = Path.Combine(localFolder.Path, SqliteDatabaseFileName);
            using (SqliteConnection connection = new SqliteConnection($"Filename={databasePath}")) {
                connection.Open();
                using (SqliteTransaction transaction = connection.BeginTransaction()) {
                    // create media table:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText =
                            "CREATE TABLE IF NOT EXISTS Media (" +
                                "MediaId INTEGER PRIMARY KEY," +
                                "MediaName NVARCHAR(256) NULL," +
                                "MediaPath NVARCHAR(256) NULL," +
                                "DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP" +
                            ")";
                        command.ExecuteReader();
                    }
                    // create playlist table:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText =
                            "CREATE TABLE IF NOT EXISTS Playlists (" +
                                "PlaylistId INTEGER PRIMARY KEY," +
                                "PlaylistName NVARCHAR(256) NULL," +
                                "DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP" +
                            ")";
                        command.ExecuteReader();
                    }
                    // create playlist media map table:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText =
                            "CREATE TABLE IF NOT EXISTS PlaylistMediaMap (" +
                                "PlaylistId INTEGER," +
                                "MediaId INTEGER," +
                                "DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP" +
                            ")";
                        command.ExecuteReader();
                    }
                    // create media locations table:
                    using (SqliteCommand command = connection.CreateCommand()) {
                        command.CommandText =
                            "CREATE TABLE IF NOT EXISTS MediaLocations (" +
                                "LocationId INTEGER PRIMARY KEY," +
                                "LocationPath NVARCHAR(256)," +
                                "DateAdded DATETIME DEFAULT CURRENT_TIMESTAMP" +
                            ")";
                        command.ExecuteReader();
                    }
                    transaction.Commit();
                }
            }
        }

        #endregion

        #region GetConnection

        public static SqliteConnection GetConnection() {
            return new SqliteConnection($"Filename={databasePath}");
        }

        #endregion

        #endregion

    }

}
