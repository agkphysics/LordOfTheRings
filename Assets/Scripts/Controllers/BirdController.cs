﻿using UnityEngine;
using System.Collections;
using System;
using ProgressBar;

/// <summary>
/// Script which controls the player, called 'bird' due to being a Flappy Bird VR derivative.
/// </summary>
public class BirdController : MonoBehaviour {

    public Engine.Interval Section { get; set; }
    public float TargetRPM { get; set; }
    public uint warmupPowerSum { get; private set; }
    public int warmupCount { get; private set; }
    public float warmupAverage { get; set; }

    public float forwardMovement;
    public float forceMultiplier;
    public float dragForce;
    public float timeBetweenlogging;

    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private float time;

    private Engine engine;
    private Rigidbody rb;
    private RowingMachineController rowingMachine;

    private static LoggerService logger;

    void Awake()
    {
		engine = GameObject.FindGameObjectWithTag("GameController").GetComponent<Engine>();
        rb = GetComponent<Rigidbody>();
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<RowingMachineController>();
        logger = new LoggerService();
    }

	// Use this for initialization
	void Start ()
    {
        Section = Engine.Interval.LOW_INTENSITY;
        TargetRPM = 60;
        warmupPowerSum = 0;
        warmupAverage = 0;
        warmupCount = 0;
        startingPosition = transform.position;
        //transform.position = startingPosition;
		startingRotation = transform.rotation;

        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        time = timeBetweenlogging;
    }

    void Update()
    {
        //track time for polling logs every second
        time -= Time.deltaTime;

        if (time <= 0)
        { 
            LogData();
            time = timeBetweenlogging;
        }

        //Recenter oculus when F12 pressed
        if (Input.GetKey(KeyCode.F12))
        {
            UnityEngine.VR.InputTracking.Recenter();
        }

        if (!engine.IsStarted)
        {
            //Reset game variables when space pressed at start of game
			if (Input.GetKeyDown(KeyCode.Space))
            {
                engine.StartGame();

                //rb.freezeRotation = false;
				rb.useGravity = false;
			}
		}
        else
        {
            
            if (engine.IsWarmingUp)
            {
                //Warmup period, time configured witin the GameController
                if (rowingMachine.WaitingRow)
                {
                    warmupPowerSum += rowingMachine.CurrentForce;
                    warmupCount++;

                    rowingMachine.WaitingRow = false;
                    
                    rb.velocity = new Vector3(rowingMachine.CurrentForce*forceMultiplier, 0);
                    if (!rowingMachine.DEBUG)
                    {
                    Debug.Log("Current Force: " + rowingMachine.CurrentForce);
                    }
                    Debug.Log("Warming up period.");
                }
            }
            else if (rowingMachine.WaitingRow)
            {
                rowingMachine.WaitingRow = false;

                if (rb.velocity.y < 0)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                }
                rb.AddForce(Vector3.right*rowingMachine.CurrentForce/warmupAverage*forceMultiplier, ForceMode.Impulse);

                if (!rowingMachine.DEBUG)
                {
                Debug.Log("Current proportionate force: " + rowingMachine.CurrentForce/warmupAverage);
                }
                engine.AddToCurrentScore(50);
            }

            // Don't move backwards
            if (rb.velocity.x <= 0) rb.velocity = new Vector3(0, rb.velocity.y);

            if (engine.IsWarmingUp)
            {
                rb.velocity = Vector3.zero;
            }

            rb.AddForce(-dragForce*Math.Abs(rb.velocity.x), 0, 0);
        }
	}

	void FixedUpdate()
    {

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
        //On collision with ring, create new ring and increment score by 500, remove ring
        GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>().NewRing();
        Section = trigger.gameObject.GetComponent<RingController>().NextRing.GetComponent<RingController>().Section;
        Debug.Log("Entering section " + Section);

        engine.AddToCurrentScore(500);
        engine.AddToCurrentCombo(1);
		Destroy(trigger.gameObject);
	}

    void LogData()
    {
        //Logging system for force, distance and heartrate.
        Power force = new Power(Time.time.ToString(), rowingMachine.CurrentForce, GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>().IsHighIntensity);
        Distance distance = new Distance(Time.time.ToString(), rowingMachine.DistanceTravelled);
        HeartRate heartRate = new HeartRate( Time.time.ToString(), GameObject.FindGameObjectWithTag("HRMonitor").GetComponent<HeartRateService>().heartRate);

        
        logger.heartRate.Enqueue(heartRate);
        logger.distance.Enqueue(distance);
        logger.power.Enqueue(force);

        logger.Log();
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
