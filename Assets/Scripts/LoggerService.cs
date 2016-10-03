using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public class LoggerService : MonoBehaviour {
    public Queue<HeartRate> heartRate = new Queue<HeartRate>();
    public Queue<Distance> distance = new Queue<Distance>();
    public Queue<Power> power = new Queue<Power>();

    public float timeBetweenlogging = 1.0f;
    private float time;

    private static string LOGGER_PATH = Environment.SpecialFolder.MyDocuments + "LOGGER_" + DateTime.Now.ToString();
    private const string HEART_RATE = "_heart_rate";
    private const string POWER = "_power";
    private const string DISTANCE = "_distance";

    private Component birdController;

    // Use this for initialization
    void Start () {
        birdController = GetComponent<BirdController>();
    }
	
	// Update is called once per frame
	void Update () {
        timeBetweenlogging -= Time.deltaTime;

        if (timeBetweenlogging <= 0)
        {
            // convert each queue to string,

            string heartRateJson = "";
            while(heartRate.Count > 0)
            {
                HeartRate data = heartRate.Dequeue();
                heartRateJson += JsonUtility.ToJson(data) +"\n";
            }
            
            string powerJson = "";
            while (power.Count > 0)
            {
                Power data = power.Dequeue();
                powerJson += JsonUtility.ToJson(data) + "\n";
            }

            string distanceJson = "";
            while (distance.Count > 0)
            {
                Distance data = distance.Dequeue();
                distanceJson += JsonUtility.ToJson(data) + "\n";
            }

            // write each metric to files
            File.AppendAllText(LOGGER_PATH + HEART_RATE, heartRateJson);
            File.AppendAllText(LOGGER_PATH + POWER, powerJson);
            File.AppendAllText(LOGGER_PATH + DISTANCE, distanceJson);

            //reset timer
            time = timeBetweenlogging;
        }
    }
}
