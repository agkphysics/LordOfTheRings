# Lord Of the Rings - an HIIT Exergame

## Paper
We have published our study in ACSW 2019 [1], the paper is available here:
https://dl.acm.org/doi/abs/10.1145/3290688.3290740

[1] A. Keesing, M. Ooi, O. Wu, X. Ye, L. Shaw, and B. C. Wünsche, 'HIIT With Hits: Using Music and Gameplay to Induce HIIT in Exergames', in Proceedings of the Australasian Computer Science Week Multiconference - ACSW 2019, Sydney, NSW, Australia, 2019, pp. 1–10. doi: 10.1145/3290688.3290740.

If you wish to cite this work, you can copy the citation below:
```
@inproceedings{keesingHIITHitsUsing2019,
author = {Keesing, Aaron and Ooi, Matthew and Wu, Ocean and Ye, Xinghao and Shaw, Lindsay and W\"{u}nsche, Burkhard C.},
title = {HIIT With Hits: Using Music and Gameplay to Induce HIIT in Exergames},
year = {2019},
isbn = {9781450366038},
publisher = {Association for Computing Machinery},
url = {https://doi.org/10.1145/3290688.3290740},
doi = {10.1145/3290688.3290740},
booktitle = {Proceedings of the Australasian Computer Science Week Multiconference},
articleno = {36},
numpages = {10},
}
```


## Summary
High-intensity interval training (HIIT) has been proven to be superior to
moderate-intensity continuous training in improving Cardiorespiratory fitness,
which is a strong determinant of morbidity and mortality. However some
researchers has addressed that HIIT requires "an extremely high level of subject
motivation" and question whether the general population could safely or
practically tolerate the extreme nature of the exercise regimen. Lord of the
Rings was developed to embrace the benefits of HIIT. It aims to implicitly
induce HIIT training in a safe and enjoyable way, with the power of music and
the gameplay.

Game demo can be seen here: https://youtu.be/ONr2rf9xyMM

## Main Features
 * Players are required to collect rings by rowing forward. Ring generation is
   based on music intensity: High intensity rings correspond to high intensity
   music and similarly for low intensity rings.
 * HIIT routine synchronization: Rings’ colour is based on the level of
   synchronisation between rowing rate and music tempo. More green means RPM
   (rows per minute) is close to music tempo, more red means less sync.
   Synchronisation is also shown in the synchronization bar on the top.
 * High intensity sections have a larger distance between rings, encouraging the
   player to row more forcefully.
 * If a player's heart rate is not high enough during high intensity sections,
   music speeds up.
 * Players can choose their own songs by placing them under the Audio directory.
   Songs are automatically analysed when starting the game.
 * Target rowing rate is based on the tempo of the song.

![ScreenShot](screenshot.JPG)

## Running the Game
Once the game is launched, wait until the "Loading song..." GUI box disappears -
this will takes a few seconds. Then press the space bar to start the game.

Note: The player should use their maximum power during warm up period.

### Enable no music condition
Tick the "No music condition" checkbox under GameObjectSpawner to run the game
without music. The game will induce a default HIIT routine.

### Not using hardware
If you are running/debugging the game without a rowing machine, use the
following keyboard controls:
 * W or up arrow key: Row foward
 * N: Change song
 * Left and right square brack keys: Change the pitch of the music
 * F12: Recenter oculus

## How To setup and use heartrate service:
* Clone this repo: https://github.com/Ofekw/hiitcopter-heartrate-service
* Install Node.js
* Open up the solution and run it in Visual Studio (you may need to install
  dependencies first such as the unviersal store SDK, Windows machine must be in
  developer mode)
* Once the bluetooth software is running, pair any generic bluetooth heart rate
  sensor to the computer and select it within the software
* Once a heart rate is shown visually,
* Open up a console in the root directory of the repo and type: `npm install`
* After node module dependencies are installed: `npm start`
* If the game is running on a different machine, open up a new console and find
  the current device's ip (`ipconfig`)
* Go to the assets folder of the game, open the config.js file; if the server is
  running on the same computer simply type in `localhost:8080`, otherwise place
  the afermentioned ip of the device running the bluetooth server with the
  `8080` port, ie: `1.1.1.10:8080`

## Oculus Support

If the oculus is not operating correctly when launching the project, perform the
following steps:
 * Edit > Project Settings > Player > Other Settings. Check VR SDK is set to
   Oculus, Check Graphics API for Windows is set to Direct3D11
 * Check safety warning on oculus is accepted
 * With commercial oculus, player must be wearing oculus before starting game

## Development Requirements
  * Oculus DK2 or Commercial tested/supported
  * Unity 2017.1.1f1

## Work Contributions

Aaron
* Music analysis integration, song intensity thresholding, beat and tempo
  detection, pitch changing
* Ring generation and colour chagning
* Sync meter animation and position function
* Code cleanup and refactoring
* User study design
* Standalone rowing data logger
* Bluetooth heart rate logger

Matthew
  * Game environment and tilesets
  * Tileset generation
  * Combo UI and score interaction
  * Warp effect generation

Ocean
  * Research on song genres and HIIT routines for rowing machine
  * HIIT music composition
  * Game over logic, code cleanup and refactoring, documentation
  * Pilot Study and User Study
  
Jeff
  * HUD redesign and implementations

## Acknowledgements
 * HIITCopter - https://github.com/Ofekw/HIIT-Copter
 * Tour de Tune 2016s2 - https://bitbucket.org/lsha074/tourdetune2
