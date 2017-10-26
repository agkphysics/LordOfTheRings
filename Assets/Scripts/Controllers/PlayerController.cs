using UnityEngine;
using System;

/// <summary>
/// Script which controls the player.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Engine.Interval Section { get; set; }
    public float TargetRPM { get; set; }
    public uint WarmupPowerSum { get; private set; }
    public int WarmupCount { get; private set; }
    public float WarmupAveragePower { get; set; }
    
    public float maxPowerOutput;
    public float forceMultiplier;
    public float dragForce;

    private const float timeBetweenlogging = 1f;

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private float time;

    private Engine engine;
    private Rigidbody rb;
    private RowingMachineController rowingMachine;
    private MusicController musicController;
    private SpeedIndicator speedIndicator;
    private HeartRateService hrService;
    private RingGenerator ringGenerator;

    private static LoggerService logger;

    void Awake()
    {
		engine = GameObject.FindGameObjectWithTag("GameController").GetComponent<Engine>();
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<RowingMachineController>();
        musicController = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicController>();
        speedIndicator = GameObject.FindGameObjectWithTag("SpeedIndicator").GetComponent<SpeedIndicator>();
        hrService = GameObject.FindGameObjectWithTag("HRMonitor").GetComponent<HeartRateService>();
        ringGenerator = GameObject.FindGameObjectWithTag("RingCreator").GetComponent<RingGenerator>();

        rb = GetComponent<Rigidbody>();
        logger = new LoggerService();
    }

	// Use this for initialization
	void Start ()
    {
        Section = Engine.Interval.LOW_INTENSITY;
        TargetRPM = 60;

        startingPosition = transform.position;
		startingRotation = transform.rotation;

        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        time = timeBetweenlogging;
    }

    void Update()
    {
        // Track time for polling logs every second
        time -= Time.deltaTime;

        if (time <= 0 && engine.IsStarted)
        { 
            LogData();
            time = timeBetweenlogging;
        }

        // Recenter oculus when F12 pressed
        if (Input.GetKey(KeyCode.F12))
        {
            UnityEngine.VR.InputTracking.Recenter();
        }

        if (!engine.IsStarted)
        {
            // Reset game variables when space pressed at start of game
			if (Input.GetKeyDown(KeyCode.Space))
            {
                engine.StartGame();
			}
		}
        else
        {
            if (rowingMachine.WaitingRow)
            {
                rowingMachine.WaitingRow = false;

                if (rb.velocity.y < 0)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                }
                rb.AddForce(Vector3.right*rowingMachine.CurrentForce*forceMultiplier/maxPowerOutput, ForceMode.Impulse);

                if (!rowingMachine.DEBUG)
                {
                    Debug.Log("Current proportionate force: " + rowingMachine.CurrentForce*forceMultiplier/maxPowerOutput);
                }
                engine.AddToCurrentScore(50);
            }

            // Don't move backwards
            if (rb.velocity.x <= 0) rb.velocity = new Vector3(0, rb.velocity.y);

            if (Section == Engine.Interval.HIGH_INTENSITY && hrService.currentHeartStatus == HeartRateService.HeartStatus.Resting)
            {
                musicController.IncreasePitch();
            }
            else if (musicController.Intensity == Engine.Interval.LOW_INTENSITY)
            {
                musicController.ResetPitch();
            }

            if (engine.noMusicCondition) TargetRPM = Section == Engine.Interval.HIGH_INTENSITY ? 35 : 25 ;

            rb.AddForce(-dragForce*Math.Abs(rb.velocity.x), 0, 0);
        }
	}

	void PlayerReset()
    {
		rb.velocity = Vector3.zero;
		transform.position = startingPosition;
		rb.rotation = startingRotation;
		rb.freezeRotation = true;
	}

	void OnTriggerEnter(Collider trigger)
    {
        // On collision with ring, create new ring and increment score by 500, remove ring
        ringGenerator.NewRing();
        Section = trigger.gameObject.GetComponent<RingController>().NextRing.GetComponent<RingController>().Section;
        Debug.Log("Entering section " + Section);

        // Rings rewards increased scores depending on combo.
        float currentPos = speedIndicator.CurrentXLocation;
        if (currentPos > 0.17f || currentPos < -0.17f)
        {
            engine.AddToCurrentScore(50);
            engine.ResetCombo();
        }
        else if (engine.combo > 15)
        {
            engine.AddToCurrentScore(400);
            engine.AddToCurrentCombo(1);
        }
        else if(engine.combo > 10)
        {
            engine.AddToCurrentScore(300);
            engine.AddToCurrentCombo(1);
        }
        else if (engine.combo > 5)
        {
            engine.AddToCurrentScore(200);
            engine.AddToCurrentCombo(1);
        }
        else if (engine.combo >= 0)
        {
            engine.AddToCurrentScore(100);
            engine.AddToCurrentCombo(1);
        }

        Destroy(trigger.gameObject);
	}

    void LogData()
    {
        // Logging system for force, distance, heartrate and current mean rpm.
        var force = new Power(Time.time.ToString(), rowingMachine.CurrentForce, Section == Engine.Interval.HIGH_INTENSITY);
        var distance = new Distance(Time.time.ToString(), rowingMachine.DistanceTravelled);
        var heartRate = new HeartRate(Time.time.ToString(), hrService.heartRate);
        var rpm = new RPM(Time.time.ToString(), rowingMachine.MeanRPM, Section == Engine.Interval.HIGH_INTENSITY);

        logger.heartRate.Enqueue(heartRate);
        logger.distance.Enqueue(distance);
        logger.power.Enqueue(force);
        logger.rpm.Enqueue(rpm);
        logger.Log();
    }
}

[Serializable]
public class RPM
{
    public string time;
    public bool intervalType;
    public double rpm;

    public RPM(string time, double rpm, bool interval)
    {
        this.time = time;
        this.rpm = rpm;
        intervalType = interval;
    }

    public override string ToString()
    {
        return time + "," + rpm + "," + intervalType;
    }
}

[Serializable]
public class HeartRate
{
    public String time;
    public double heartrate;

    public HeartRate(String time, double data)
    {
        this.time = time;
        heartrate = data;
    }

    public override string ToString()
    {
        return time + "," + heartrate;
    }
}

[Serializable]
public class Power
{
    public String time;
    public bool intervalType;
    public double power;

    public Power(String time, double data, bool interval)
    {
        this.time = time;
        power = data;
        intervalType = interval;
    }

    public override string ToString()
    {
        return time + "," + power + "," + intervalType;
    }
}
[Serializable]
public class Distance
{
    public String time;
    public double distance;

    public Distance(String time, double data)
    {
        this.time = time;
        distance = data;
    }

    public override string ToString()
    {
        return time + "," + distance;
    }
}
