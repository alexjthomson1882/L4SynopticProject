This document outlines how the application should be used.

---

## Music Library Management

### Adding Music
The application will search for media within your Windows music library. This is a built-in Windows feature that allows you to add a number of folders to a music library. By deafult, `C:/Users/<user>/Music/` is added to this library.

To add an additonal folder, first create the folder, then right click the folder within the Windows file explorer:

![[AddingMusicFolderToLibrary.png]]
Navigate to the `Include in library` option and pick `Music`.

You will need to restart the application for it to register new libraries. Tracked libraries can be viewed in the `Scan Locations` section:

![[ScanLocations.png]]

### Viewing the Media Library
The music library tracked by the application can be viewed in the `Library` section:

![[Library.png]]

### Removing Missing Music from Library
Music will still be tracked, even if removed. To remove it from the library, right click the target music, and select `Remove from Library`:

![[GonnaGiveYouUp.png]]
You can also remove music from the playlist screen using the same method.

---

## Playlist Management

### Creating a Playlist
To create a playlist, simply press the `Create Playlist` button:

![[CreatePlaylist.png]]

### Viewing Created Playlists
To view playlists you have created, simply click the `Playlists` button:

![[ViewPlaylists.png]]

### Renaming a Playlist
You can rename a playlist by selecting the playlist and clicking the playlist name. You can then type a new name. The new name is saved automatically:

![[RenamePlaylist.png]]

### Adding or Removing Music from a Playlist
First, navigate to the playlist. Next right click some music you would like to either add or remove from the current playlist. Then select the option you want. The playlist is displayed on the left, while your entire music library is displayed on the right. You can drag the vertical white bar to expand / shrink the library section (indicated below with red arrows):

![[LibraryExpandBar.png]]

### Searching for Music
You can use either one of two search bars on the playlist page to search either your playlist, or your media library. To display everything, clear the search box.

### Sorting Music
Music can be sorted by each column by clicking the column header. Clicking the header will cycle through sorting modes. The sorting modes include `None`, `Ascending`, and `Descending`:
![[SortingMusic.png]]

### Deleting a Playlist
To delete a playlist, click the `Delete Playlist` button:

![[DeletePlaylistStep1.png]]

Next, select the `Delete` option, or press `Cancel` if you do **not** want to delete the playlist:

![[DeletePlaylistStep2.png]]

---

## Playback Controls
The media controls are displayed at all times at the bottom of the application:

![[MediaControls.png]]
The above screenshot contains numbers, corresponding to different media control features:

| Number | Feature | How to Use |
| - | - | - |
| 1 | Music Information | Displays the music track name followed by the artist (if any). |
| 2 | Play / Pause | Clicking this toggles the play / pause state of the currently selected music. |
| 3 | Last Track | Clicking this navigates to the last track. |
| 4 | Next Track | Clicking this navigates to the next track. |
| 5 | Shuffle | Clicking this toggles the shuffle state. When shuffle is enabled, tracks will be picked at random from the current context. If you select a track in your playlist and enable shuffle, the next tracks will be picked at random. |
| 6 | Repeat | Clicking this toggles the repeat state. When repeat is enabled, the current set of tracks will be repeated when each track has been played. |
| 7 | Current Time | Displays how far into the current track you are. |
| 8 | Music Duration | Displays the total duration of the current track. |
| 9 | Seek Bar | Displays how far through the current track you are. You can also drag the slider and change your position manually. |
| 10 | Mute | Indicates if playback has been muted or not. This can be manually clicked to toggle the muted state. |
| 11 | Volume Bar | Indicates the current volume playback will be played with. The slider can be manually dragged to set a target volume between `0.0` (muted) and `100.0` (maximum volume). |