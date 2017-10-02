﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;

public class LoggerService {
    public Queue<HeartRate> heartRate = new Queue<HeartRate>();
    public Queue<Distance> distance = new Queue<Distance>();
    public Queue<Power> power = new Queue<Power>();

    private static string LOGGER_PATH;
    private const string HEART_RATE = "_heart_rate.csv";
    private const string POWER = "_power.csv";
    private const string DISTANCE = "_distance.csv";

    public LoggerService()
    {
        LOGGER_PATH = Application.persistentDataPath + "/LOGGER_" + DateTime.Now.Ticks.ToString();
        File.AppendAllText(LOGGER_PATH + HEART_RATE, "Time,Heart Rate\n");
        File.AppendAllText(LOGGER_PATH + POWER, "Time,Power,High Intensity\n");
        File.AppendAllText(LOGGER_PATH + DISTANCE, "Time,Distance\n");
    }
	
	public void Log()
    {
        // convert each queue to string,
        string heartRateJson = "";
        while(heartRate.Count > 0)
        {
            HeartRate data = heartRate.Dequeue();
            heartRateJson += data.ToString() + "\n";
        }
            
        string powerJson = "";
        while (power.Count > 0)
        {
            Power data = power.Dequeue();
            powerJson += data.ToString() + "\n";
        }

        string distanceJson = "";
        while (distance.Count > 0)
        {
            Distance data = distance.Dequeue();
            distanceJson += data.ToString() + "\n";
        }

        // write each metric to files
        File.AppendAllText(LOGGER_PATH + HEART_RATE, heartRateJson);
        File.AppendAllText(LOGGER_PATH + POWER, powerJson);
        File.AppendAllText(LOGGER_PATH + DISTANCE, distanceJson);
    }
}
