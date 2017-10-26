# Lord Of the Rings - an HIIT Exergame

High-intensity interval training (HIIT) has been proven to be superior to moderate-intensity continuous training in improving Cardiorespiratory fitness, which is a strong determinant of morbidity and mortality. However some researchers has addressed that HIIT requires "an extremely high level of subject motivation" and question whether the general population could safely or practically tolerate the extreme nature of the exercise regimen. Lord of the Rings was developed to embrace the benefits of HIIT. It aims to induce HIIT training in a safe and enjoyable way, with the power of music and the gameplay.

Game demo can be seen here: https://youtu.be/ONr2rf9xyMM

## Running the Game



## Development Requirements
  * Oculus DK2 or Commercial tested/supported
  * Unity 2017.1.1f1

## How To setup and use heartrate service:
* Clone this repo: https://github.com/Ofekw/hiitcopter-heartrate-service
* Install Node.js
* Open up the solution and run it in Visual Studio (you may need to install dependencies first such as the unviersal store SDK, Windows machine must be in developer mode)
* Once the bluetooth software is running, pair any generic bluetooth heart rate sensor to the computer and select it within the software
* Once a heart rate is shown visually,
* Open up a console in the root directory of the repo and type: `npm install`
* After node module dependencies are installed: `npm start`
* If the game is running on a different machine, open up a new console and find the current device's ip (`ipconfig`)
* Go to the assets folder of the game, open the config.js file; if the server is running on the same computer simply type in `localhost:8080`, otherwise place the afermentioned ip of the device running the bluetooth server with the `8080` port, ie: `1.1.1.10:8080`

## Oculus Support

If the oculus is not operating correctly when launching the project, perform the following steps:
 * Edit > Project Settings > Player > Other Settings. Check VR SDK is set to Oculus, Check Graphics API for Windows is set to Direct3D11
 * Check safety warning on oculus is accepted
 * With commercial oculus, player must be wearing oculus before starting game

## Work Contributions

Aaron
  * 

Matthew
  * 

Ocean
  * 
  
Jeff
  * 

## Acknowledgements
HIITCopter - https://github.com/Ofekw/HIIT-Copter
Tour de Tune 2016s2 - https://bitbucket.org/lsha074/tourdetune2