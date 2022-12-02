This document contains some documentation about the application itself. This may be useful for future development of the application.

## Frameworks Used
This music player was written as a "Universal Windows Platform" (UWP) application. This allows the application to be deployed to any Windows 10 or 11 device and integrate smoothly with Windows. It could also be deployed to XBox, and Windows Phone.

## How Does the Applcation Work?
The application maintains an Sqlite database which is used to track media within the users Music library. This Music library is a built-in windows feature which allows users to add folders to a Music library that applications can then access.

The application first finds the locations of these music libraries and compiles them into a list. It then works through that list, adding each library to the Sqlite database. These folders are then reffered to as as "Scan Locations".

The application will then recursively look through each scan location for any supported media files. These are identified by their file extension. The application supports the following file extensions:

```
.mp3     (tested)
.wav     (tested)
.ogg     (untested)
.flac    (untested)
.avi     (untested)
.wmv     (untested)
.m4a     (untested)
```

Once a list of media has been identified, it is tracked in the Sqlite database.

The application then loads any existing playlists from the Sqlite database and finally completes the startup process.

Any changes made during runtime are tracked in the Sqlite database, which persists between application launches and system reboots since it is stored locally on the system.

## Requirements
The initial project requirements are as follows:

- Allow music playback.
- Allow music playback when the device becomes idle. The device will automatically switch to idle mode to save power if no user interaction is recorded over a 30 second period.
- Have a random shuffle function.
- Have a search option for audio files within a media database.
- List display options by song track or album.
- Allow for a Creation of a song play list.
- Have user control over playback.

Each of these requirements was met.