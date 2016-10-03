using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

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
            this.maxHeartRate = calculateMaxHeartRate();
        }
    }
	
	// Update is called once per frame
	void Update () {

        string config = System.IO.File.ReadAllText(Application.dataPath + "/config.json");
        this.config = JsonUtility.FromJson<Config>(config);

        if (time + 1.0f <= Time.time)
        {
            StartCoroutine(WaitForRequest(new WWW(this.config.api)));
            time = Time.time;
        }

        this.currentHeartStatus = calculateHeartStatus();


        Debug.Log(currentHeartStatus);

    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
        }
        else {
            Debug.Log("WWW Error: " + www.error);
        }
        if (www.text != null && www.text.Length >0 )
        {
            var jsonHeartRate = JsonUtility.FromJson<HeartRate>(www.text);
            this.heartRate = (jsonHeartRate.heartrate);
        } else
        {
            this.heartRate = default_heart_rate;
        }

        if (www.text.Length != 0)
        {
            var jsonHeartRate = JsonUtility.FromJson<HeartRate>(www.text);
            this.heartRate = (jsonHeartRate.heartrate);
        }
    }

    public HeartStatus calculateHeartStatus()
    {
        if((this.heartRate  / this.maxHeartRate) > 0.85)
        {
            return HeartStatus.Overexerting;
        }
        else if ((this.heartRate / this.maxHeartRate) > 0.50)
        {
            return HeartStatus.Optimal;
        }
        else
        {
            return HeartStatus.Resting;
        }
    }

    private Double calculateMaxHeartRate()
    {
        var age = this.config.age;
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
