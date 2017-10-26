using UnityEngine;

/// <summary>
/// Controller for detecting rows.
/// </summary>
public class RowingMachineController : MonoBehaviour
{
    public bool WaitingRow { get; set; }
    public uint DistanceTravelled { get; private set; }
    public uint CurrentForce { get; private set; }
    public bool DEBUG { get { return rower.DEBUG; } }

    /// <summary>
    /// The current mean row time delta in seconds, i.e. the mean time between rows.
    /// Averaged over a number of periods.
    /// </summary>
    public float MeanRowTime { get; private set; }

    /// <summary>
    /// The mean rows per minute. Equal to 60/MeanRowTime.
    /// </summary>
    public float MeanRPM { get { return 60 / MeanRowTime; } }

    private Rower rower;
    private Engine engine;
    
    private uint oldPower;
    private uint oldPace;

    private float[] rowTimes = new float[3];
    private float lastRowTime;
    private int currIdx = 0;
    private int rowCount = 0;

    // Use this for initialization
    void Awake ()
    {
        WaitingRow = false;
        rower = GetComponent<Rower>();
        engine = GameObject.FindGameObjectWithTag("GameController").GetComponent<Engine>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (engine.IsStarted)
        {
            //Check if user has performed a row.
            uint distance = rower.RowDistance;
            uint pace = rower.RowPace;
            uint power = rower.RowPower;

            if (distance == 0 || pace == 0 || power == 0)
            {
                return;
            }

            CurrentForce = power;

            //Compare users previous pace and power to their new pace and power to check for difference
            //Currently no way of just getting if the user has rowed so detection done by looking for differences
            if (oldPace != pace || oldPower != power)
            {
                Row(distance, power, pace);
            }
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
        DistanceTravelled = distance;
        WaitingRow = true;
        
        oldPower = power;
        oldPace = pace;
        lastRowTime = Time.time;
        if (!rower.DEBUG)
        {
            Debug.Log("Row detected: distance = " + distance + ", power = " + power + ", pace = " + pace);
        }
        GameObject.FindGameObjectWithTag("RPM").GetComponent<TextMesh>().text = Mathf.RoundToInt(MeanRPM) + "RPM";
    }
}
