# Level 4 Software Development Synoptic Project

## Project Overview & Objectives
You work for Rebmem Engineering that produces solutions for external clients. They wish to produce their own media player. They have the capability to produce the electronic hardware but need an interface to allow for the sale of the device as a portable music player.

Your manager would like you to take ownership of the initial concept.
You will need to:
- Design, configure, test and install a Music player that meets business requirements.
- Roll out within the agreed time frame of a maximum of 5 days.
- Build an interface that allows for continuous improvement.

## Project Layout

- /Documents
  Contains documentation regarding planning, testing, user / application documentation, and some other miscellaneous documents.
- /MediaPlayer
  Contains the main media player project files and source code.
- /MediaPlayer/MediaPlayer
  Contains the media player project.
- /MediaPlayer/MediaPlayer.Tests
  Contains the unit test project for the MediaPlayer.

## Project Evaluation
In the timeframe given, I've managed to create a functional UWP application that delivers all of the specified requirements.

I did not feel it was enough time to write the application how I would've wanted. Due to time constraints, some code does not have thorough commenting, while other parts do. If I had more time, I would review all of the code I had written and completely document it. This would make future development much easier.

I also would've written more unit tests for the project. When it came to testing everything I had to do it manually while developing by creating a test plan. This test plan can be found in `/Documents/Documentation/Testing/Tests.pdf`. Writing unit tests would make the entire process much easier and in the future would make development faster since if something went wrong while developing the application, it should get caught by the unit tests if they're kept up-to-date.