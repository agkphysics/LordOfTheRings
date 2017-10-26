using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Script which calculate heart status.
/// </summary>
public class HeartRateService : MonoBehaviour {

    public enum HeartStatus { Resting, Optimal, Overexerting};
    public const Double default_heart_rate = 70.0;
    public Double heartRate = 0;
    private Double maxHeartRate = 0;
    public HeartStatus currentHeartStatus = HeartStatus.Resting;

    public Config config;

    private float time;

    // Use this for initialization
    void Start () {
        time = Time.time;
        if (maxHeartRate == 0)
        {
            maxHeartRate = CalculateMaxHeartRate();
        }
    }
	
	// Update is called once per frame
	void Update () {
        string config = System.IO.File.ReadAllText(Application.dataPath + "/config.json");
        this.config = JsonUtility.FromJson<Config>(config);

        if (time + 1.0f <= Time.time)
        {
            //poll heart rate from node server using ip in config file
            StartCoroutine(WaitForRequest(new WWW(this.config.api)));
            time = Time.time;
        }

        currentHeartStatus = CalculateHeartStatus();
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // Check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
        }

        if (www.text != null && www.text.Length > 0)
        {
            var jsonHeartRate = JsonUtility.FromJson<HeartRate>(www.text);
            heartRate = (jsonHeartRate.heartrate);
        }
        else
        {
            heartRate = default_heart_rate;
        }

        if (www.text.Length != 0)
        {
            var jsonHeartRate = JsonUtility.FromJson<HeartRate>(www.text);
            heartRate = (jsonHeartRate.heartrate);
        }
    }

    public HeartStatus CalculateHeartStatus()
    {
        // Calculate what heart rate zone the user is in based on their max heart rate.
        // > 90% = overexerting, 90% > hr > 70% = optimal
        if((heartRate / maxHeartRate) > 0.90)
        {
            return HeartStatus.Overexerting;
        }
        else if ((heartRate / maxHeartRate) > 0.70)
        {
            return HeartStatus.Optimal;
        }
        else
        {
            return HeartStatus.Resting;
        }
    }

    private Double CalculateMaxHeartRate()
    {
        var age = config.age;
        double maxHeartRate = (208 - (0.7*age));
        return maxHeartRate;
    }

    [Serializable]
    public class HeartRate
    {
        public String time;
        public int heartrate;
    }

    [Serializable]
    public class Config
    {
        public int age;
        public string api;
    }
}
