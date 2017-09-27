using UnityEngine;
using System.Collections;

/// <summary>
/// Controller for detecting rows.
/// </summary>
public class RowingMachineController : MonoBehaviour {

    public bool waitingRow = false;
    public uint waitingDistance;
    public uint distanceTravelled; 
    public uint currentForce;

    public bool DEBUG { get { return rower.DEBUG; } }

    Rower rower;

    uint oldDistance;
    uint oldPower;
    uint oldPace;

    float[] rowTimes = new float[6];
    float lastRowTime;
    int currIdx = 0;
    int rowCount = 0;


    /// <summary>
    /// The current mean row time delta in seconds, i.e. the mean time between rows.
    /// Averaged over a number of periods.
    /// </summary>
    public float MeanRowTime { get; private set; }

    /// <summary>
    /// The mean rows per minute. Equal to 60/MeanRowTime.
    /// </summary>
    public float MeanRPM { get { return 60/MeanRowTime; } }

    // Use this for initialization
    void Awake ()
    {
        rower = GetComponent<Rower>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Check if user has performed a row.
        uint distance = rower.rowDistance;
        uint pace = rower.rowPace;
        uint power = rower.rowPower;

        if(distance == 0 || pace == 0 || power == 0)
        {
            return;
        }

        currentForce = power;
        //Compare users previous pace and power to their new pace and power to check for difference
        //Currently no way of just getting if the user has rowed so detection done by looking for differences
        if(oldPace != pace || oldPower != power)
        {
            Row(distance, power, pace);
        }
	}

    /// <summary>
    /// Performs one row action and updates all necessary values.
    /// </summary>
    /// <param name="distance">The distance the rower has travelled.</param>
    /// <param name="power">The power output of the rower.</param>
    /// <param name="pace">The rower's pace, in s/km.</param>
    void Row(uint distance, uint power, uint pace)
    {
        rowCount++;
        float deltaT = Time.time - lastRowTime;
        if (rowCount <= rowTimes.Length)
        {
            MeanRowTime = (MeanRowTime*(rowCount - 1) + deltaT)/rowCount;
        }
        else
        {
            MeanRowTime += (deltaT - rowTimes[currIdx])/rowTimes.Length;
        }
        rowTimes[currIdx] = deltaT;
        currIdx = (currIdx + 1) % rowTimes.Length;
        distanceTravelled = distance;
        waitingDistance = distance - oldDistance;
        waitingRow = true;

        oldDistance = distance;
        oldPower = power;
        oldPace = pace;
        lastRowTime = Time.time;
        if (!rower.DEBUG)
        {
        Debug.Log("Row detected: distance = " + distance + ", power = " + power + ", pace = " + pace);
        }
        GameObject.Find("RPM").GetComponent<TextMesh>().text = Mathf.RoundToInt(MeanRPM) + "RPM";
    }
}
