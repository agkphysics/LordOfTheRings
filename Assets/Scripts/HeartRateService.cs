using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class HeartRateService : MonoBehaviour {

    public enum HeartStatus { Resting, Optimal, Overexerting};

    public Double heartRate = 0;
    private Double maxHeartRate = 0;
    public HeartStatus currentHeartStatus = HeartStatus.Resting;
    WWW www = new WWW("http://hiitcopter.herokuapp.com/api/heartrate");
    Text txt;
    // Use this for initialization
    void Start () {
        txt = gameObject.GetComponent<Text>();

        if (maxHeartRate == 0)
        {
            this.maxHeartRate = calculateMaxHeartRate();
        }
    }
	
	// Update is called once per frame
	void Update () {
        // string input = System.IO.File.ReadAllText(@"C:\Users\ofekw\AppData\Local\Packages\Microsoft.SDKSamples.BluetoothGattHeartRate.CS_8wekyb3d8bbwe\LocalState\heartrate.txt");
        // txt.text = input;
        // heartRate = int.Parse(input);


        StartCoroutine(WaitForRequest(www));
        Debug.Log(currentHeartStatus);

    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.data);
        }
        else {
            Debug.Log("WWW Error: " + www.error);
        }

        var jsonHeartRate = JsonUtility.FromJson<HeartRate>(www.data);

        this.heartRate = (jsonHeartRate.heartrate);

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

    [Serializable]
    public class HeartRate
    {
        public String time;
        public int heartrate;
    }
}
