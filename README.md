Oculus-Flappy

Work Contributions

Jared
  * Ring spawning and generation
  * Dynamic difficulty via heart rate during high intensity periods
  * Length and diffuclty of intensity intervals and switching
  * Warm up period to set difficulty dynamically
  * Rowing power to movement balancing

Harry
  * 
  
Ofek
  * Base architecture (including controllers, physics, procedural generation etc)
  * Placeholder assets
  * Heart rate service (recieves heartate data from web service, can be polled at any time to identify current heartate zone)
  * Bluetooth heart rate software, (including web service to communicate with the game engine). Repo for the bluetooth software and   webservice: https://github.com/Ofekw/hiitcopter-heartrate-service
  * Logging system (all metrics such as heart rate, power and distance traveled is logged as a unique csv)
  * Config system
  
Alex
  * 
  
Patrick
  * 



# How To setup and use heartrate service:
* Clone this repo: https://github.com/Ofekw/hiitcopter-heartrate-service
* Install Node.js
* Open up the solution and run it in Visual Studio (you may need to install dependencies first such as the unviersal store SDK, Windows machine must be in developer mode)
* Once the bluetooth software is running, pair any generic bluetooth heart rate sensor to the computer and select it within the software
* Once a heart rate is shown visually,
* Open up a console in the root directory of the repo and type: `npm install`
* After node module dependencies are installed: `npm start`
* If the game is running on a different machine, open up a new console and find the current device's ip (`ipconfig`)
* Go to the assets folder of the game, open the config.js file; if the server is running on the same computer simply type in `localhost:8080`, otherwise place the afermentioned ip of the device running the bluetooth server with the `8080` port, ie: `1.1.1.10:8080`

