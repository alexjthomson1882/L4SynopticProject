| Test Number | Test Description | Test Tools | Expected Outcome | Actual Outcome | Passed / Failed |
| ----------- | ---------------- | ---------- | ---------------- | -------------- | ----------- |
| 01 | Application starts. | - | Application opens to the `Library` page. | As expected. | Passed |
| 02 | Navigation pane functions. | - | The navigation panel is able to navigate between every page listed. | As expected. | Passed |
| 03 | Scan locations are correct. Add and remove folders from Windows user's Music library and verify scan locations are tracked correctly. | Sqlite DB Browser | `Scan Locations` reflects the Sqlite database scan locations and the folders added to the Windows user's Music library. | As expected. | Passed |
| 04 | Create new playlist. | Sqlite DB Browser | Application creates a new playlist when clicking the `Create Playlist` button and navigates to the new playlist page. This change should be reflected in the Sqlite database. | As expected. | Passed |
| 05 | View library. | Sqlite DB Browser | Application lists each bit of media tracked in by the Sqlite database. | As expected. | Passed |
| 06 | Remove media from library. | Sqlite DB Browser | Application removes media from the library and reflects the change in the Sqlite database. | Change reflected in the Sqlite database and media removed from tracked media but UI did not update to reflect the change. | Failed |
| 07 | Remove media from library. | Sqlite DB Browser | Application removes media from the library and reflects the change in the Sqlite database. | As expected | Passed |
| 08 | Remove playlist. | Sqlite DB Browser | Playlist is removed from application and Sqlite database. | As expected. | Passed |
| 09 | Rename playlist. | Sqlite DB Browser | Playlist is renamed and updated in Sqlite database. | As expected. | Passed |
| 10 | Add media to playlist. | Sqlite DB Browser | Media is added correctly from library to a target playlist. | As expected. | Passed |
| 11 | Remove media from playlist. | Sqlite DB Browser | Media is removed from the playlist but remains in the library. | UI did not update to reflect change. Otherwise as expected. | Failed |
| 12 | Remove media from playlist. | Sqlite DB Browser | Media is removed from the playlist but remains in the library. | As expected | Passed |
| 13 | Able to play media. | - | The application begins playback for the selected media. | As expected. | Passed |
| 14 | Able to navigate to the next/last media tracks in the current context. | - | The application is able to navigate to the next/last media in the current context, respecting the shuffle and repeat options that are selected. | Works as expected until the end of the context is reached, an IndexOutOfRange exception is thrown. | Failed |
| 15 | Able to navigate to the next/last media tracks in the current context. | - | The application is able to navigate to the next/last media in the current context, respecting the shuffle and repeat options that are selected. | As expected. | Passed |
| 16 | Able to sort media in table. | - | The application is able to sort media by cycling through sorting modes per column. | As expected. | Passed |
| 17 | Able to search for media in current playlist context. | - | The application is able to search for media within the current playlist context (including the playlist media library UI). | As expected. | Passed |
| 18 | Clean shutdown. | Visual Studio | The application closes cleanly without raising any exceptions. | As expected. | Passed |
| 19 | Plays media while idle. | - | The host PC is allowed to enter an idle state but the music player continues to play music. | As expected; however, music briefly stopped while entering idle state. This is caused by Windows, not the application. | Passed |
| 20 | Volume and mute controls function. | - | The volume slider is able to update the volume of the media. The mute button allows the mute state to be toggled and is also automatically toggled when the volume slider is manually set to `0.0`. | As expected. | Passed |
