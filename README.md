# Lord Of the Rings
## An HIIT exergame

This game is based off [HIIT-Copter](https://github.com/Ofekw/HIIT-Copter) described below.

---

## HIIT (High Intensity Interval Training) Copter

Research has shown that intense cardiovascular exercise has been proven to be an effective means to maintaining good health and improving fitness levels. This cardio is called High Intensity Interval Training. The concept of interval training is the transition from low-moderate intensity intervals to very high intensity intervals. However, this form of exercise is very demanding and can be tedious, which can lead to discouragement and a lack of motivation by the end user.

This repo has a proof of concept, novel exergame which successfully and implicitly induces a HIIT workout; masking the tiresome nature of an explosive cardiovascular workout whilst attempting to mitigate overexertion. This is achieved through an immersive yet simple game environment that subtly and intuitively enforces periods of high intensity work. This is accomplished without explicitly instructing the player to perform high intensity work.  The game employs novel algorithms that creates a curated procedurally generated world, which intuitively forces the user into high intensity and low intensity exercise.

Our user study demonstrates a statistically significant increase in exercise engagement and motivation when using the exergame when compared to a workout following pre-recorded instruction. By employing an exercise machine that directly translates real world actions into the game world and the inclusion of a virtual reality headset; players were left immersed and engaged. This led to levels of engagement that seemed to offset the deterrence and loss of enthusiasm associated with intense physical exertion. 


Game demo can be seen here:

[![HIIT Copter promo](http://img.youtube.com/vi/QqtGDxjESN0/0.jpg)](http://www.youtube.com/watch?v=QqtGDxjESN0)

## Work Contributions

Jared
  * Ring spawning and generation
  * Dynamic difficulty via heart rate during high intensity periods
  * Length and diffuclty of intensity intervals and switching
  * Warm up period to set difficulty dynamically
  * Rowing power to movement balancing

Harry
  * Camera calibration
  * Ring spawning and generation
  * Initial dynamic difficulty adaptation via heart rate during high intensity periods 
  * User Interface and heads up display
  * Visuals
  * Heart rate service interface with game
  * User study statistical analysis and evaluation
  
Ofek
  * Base architecture (including controllers, physics, procedural generation etc)
  * Placeholder assets
  * Heart rate service (recieves heartate data from web service, can be polled at any time to identify current heartate zone)
  * Bluetooth heart rate software, (including web service to communicate with the game engine). Repo for the bluetooth software and   webservice: https://github.com/Ofekw/hiitcopter-heartrate-service
  * Logging system (all metrics such as heart rate, power and distance traveled is logged as a unique csv)
  * Config system
  
Alex
  * System to convert rowing machine data to in game rows
  * Ring spawning and generation
  * Interval intensity handling, and interval switching
  * Asset creation (Rings)
  * Warmup Period
  * Dynamic difficulty based on warmup
  * Dynamic difficulty based on heart rate
  
Patrick
  * Oculus support
  * Initial UI
  * Scoring
  * Visuals (Rings, Warp Speed and Skybox)

## Development Requirements
  * Oculus DK2 or Commercial tested/supported
  * Unity 5.4.1

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

