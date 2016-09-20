using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class HeartRateService : MonoBehaviour {

    public enum HeartStatus { Resting, Optimal, Overexerting};

    public Double heartRate = 0;
    private Double maxHeartRate = 0;
    public HeartStatus currentHeartStatus = HeartStatus.Resting;
    Text txt;
    // Use this for initialization
    void Start () {
        txt = gameObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        string input = System.IO.File.ReadAllText(@"C:\Users\ofekw\AppData\Local\Packages\Microsoft.SDKSamples.BluetoothGattHeartRate.CS_8wekyb3d8bbwe\LocalState\heartrate.txt");
        txt.text = input;
        heartRate = int.Parse(input);

        if (maxHeartRate == 0)
        {
            this.maxHeartRate = calculateMaxHeartRate();
        }

        currentHeartStatus = calculateHeartStatus();
        Debug.Log(currentHeartStatus);


    }

    private HeartStatus calculateHeartStatus()
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
        var age = 20;
        int maxHeartRate = 220 - age;
        return maxHeartRate;
    }
}
