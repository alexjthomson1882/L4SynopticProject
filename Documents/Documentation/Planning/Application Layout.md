# Main Layout
The application should have 3 main sections that are always displayed:

- **Content Area**
  This is used to view content that the user wants to view.
- **Navigation Panel**
  A panel containing a short list of distinct functions. This will allow the user to quickly navigate the application. This will make the application simple to use.
- **Media Playback Controls**
  A set of powerful media playback controls that are simple to understand and use. They should allow the user to play/pause the current track, skip to the next track, go to the previous track, enable / disable shuffle play, enable / disable repeat mode, change the volume of the playback, mute the playback, view the name and album of the track currently being played.

---

# Colour Scheme & Readability
- A simplistic, modern colour scheme should be used.
- Dark colours should be used so the application is easier to view during all times of day.
- Colours should be clear and distinct from each other.
- Text should be easily read and appropriately sized.

---

# Basic Panel Layout
To design the basic layout of the application I used Photoshop:
![[PhotoshopApplicationLayout.png]]

The bottom panel contains all of the media controls in the centre. I took inspiration from Spotify for their simple controls. The volume slider is positioned on the right since the majority of media players I haved used on PC place it there. This should make it familiar to use for most people. On the left of this bar is where the information about the current media being played should be displayed.

The left panel will contain a list of each of the pages that the user can access. These should be layed out logically and make it easy to navigate and use the application. In other projects I have used something called "font-awesome" to add icons to text options. A similar approach could be used here to add icons to the navigations options, making it easier to navigate.

The large black panel will contain the current content the user is looking at. This will be split into pages that can be accessed via the navigation panel.

---

# Individual Content Pages
The navigation panel needs a set of content pages that can accessed. These are defined below:

## Media Library
This will contain a list of the currently tracked media:
![[MediaLibrary.png]]
The media library contains a table that can be scrolled through. It will show details about each media file with a row for each.

## Tracked Folders
The application needs to show the user which folders it is looking for media inside. This should be done similar to how the list of media is shown to the user:
![[TrackedFoldersLayout.png]]

## Playlist List
The application needs to show the user a list of their playlists. This should be done similar to how the list of media is shown to the user:
![[PlaylistListLayout.png]]

## Playlist Editor
The application needs a playlist editor that allows the user to customize a playlist:
![[PlaylistLayout.png]]
I have used red to annotate which features are displayed in each area.

There should be an options box on the top right that lets the user delete and rename their playlist.

There should be two tables, one containing their entire media library so they can add songs to their playlist, and the other should contain the songs in the users playlist.