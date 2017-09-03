using UnityEngine;
using System.Collections;
using System;

public class BirdController : MonoBehaviour {

	public GameObject ringCollider;

	public float forwardMovement = 2f;
	public int upAngle=45, downAngle=280; //-80 degrees
    public float forceMultiplier = 10f;
    public float dragForce;

    public int workoutPhase = 0; //0 menu screen, 1 warmup, 2 intervals

	public Vector3 startingPosition;
	[Range (-90,90)] public int zRotation;
	public Quaternion startingRotation;

	private bool waitingForPlayerToStart, scoreboard;

	private Engine engine;
    private int scoreShowingCount = 0;
	private float rotationAmount;

    public uint warmupDistance;
    public uint warmupPowerSum = 0;
    public int warmupCount = 0;
    public float warmupAverage = 0;

    public float timeBetweenlogging = 1.0f;
    private float time;

    Rigidbody rb;

    void Awake(){
		engine = GameObject.Find("GameObjectSpawner").GetComponent<Engine>();
        rb = GetComponent<Rigidbody>();
	}

	// Use this for initialization
	void Start () {
		transform.position = startingPosition;
		startingRotation = transform.rotation;

        rb.useGravity = false;
		waitingForPlayerToStart = true;
        rb.velocity = new Vector3(rb.velocity.x, 0, 0);
        time = timeBetweenlogging;

    }

    void Update() {
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

        if (waitingForPlayerToStart)
        {
            //Reset game variables when space pressed at start of game
			if(Input.GetKeyDown(KeyCode.Space)){
				scoreShowingCount = 0;
                engine.StartGame();
				waitingForPlayerToStart = false;

                //rb.freezeRotation = false;
				rb.useGravity = false;
			}
		}
        else
        {
            if (engine.isWarmingUp)
            {
                //Warmup period, time configured witin the GameObjectSpawner
                if (gameObject.GetComponent<RowingMachineController>().waitingRow)
                {
                    warmupPowerSum += GetComponent<RowingMachineController>().currentForce;
                    warmupCount++;

                    gameObject.GetComponent<RowingMachineController>().waitingRow = false;
                    
                    rb.velocity = new Vector3(GetComponent<RowingMachineController>().currentForce*forceMultiplier, 0);
                    Debug.Log("Current Force: " + GetComponent<RowingMachineController>().currentForce);
                    Debug.Log("Warming up period.");
                }
            }
            else if (gameObject.GetComponent<RowingMachineController>().waitingRow)
            {
                gameObject.GetComponent<RowingMachineController>().waitingRow = false;

                if (rb.velocity.y < 0)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0, 0);
                }
                rb.AddForce(Vector3.right*GetComponent<RowingMachineController>().currentForce/warmupAverage*forceMultiplier, ForceMode.Impulse);

                Debug.Log("Current proportionate force: " + GetComponent<RowingMachineController>().currentForce/warmupAverage);

                engine.AddToCurrentScore(50);
            }
            
            //if (Input.GetKey(KeyCode.W))
            //{
            //    rb.AddForce(Vector3.right*10f, ForceMode.Acceleration);
            //}
            //else
            //{
            //    rb.AddForce(Vector3.left*2f, ForceMode.Acceleration);
            //}

            // Don't move backwards
            if (rb.velocity.x <= 0) rb.velocity = new Vector3(0, rb.velocity.y);

            if (engine.isWarmingUp)
            {
                rb.velocity = Vector3.zero;
            }

            rb.AddForce(-dragForce*Math.Abs(rb.velocity.x), 0, 0);
        }
	}

	void FixedUpdate(){
  //      //move player foward at constant rate
		//if(!waitingForPlayerToStart){
  //          uint rowDistance = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<Rower>().rowDistance;

  //          if (rowDistance > warmupDistance)
  //          {
  //              transform.position += Vector3.right*Time.fixedDeltaTime*forwardMovement;
  //          }
		//}
	}

	void BirdReset(){
		rb.velocity = Vector3.zero;
		transform.position = startingPosition;
		rb.rotation = startingRotation;
		rb.freezeRotation = true;
	}

	void OnTriggerEnter(Collider scorebox){
        //On collision with ring, create new ring and increment score by 500, remove ring
        GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>().NewRing();

        engine.AddToCurrentScore(500);
		Destroy(scorebox.gameObject);
	}

    void LogData()
    {
        //Logging system for force, distance and heartrate.
        Power force = new Power(Time.time.ToString(), GetComponent<RowingMachineController>().currentForce, GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>().isHighIntensity);
        Distance distance = new Distance(Time.time.ToString(), GetComponent<RowingMachineController>().distanceTravelled);
        HeartRate heartRate = new HeartRate( Time.time.ToString(), GameObject.FindGameObjectWithTag("HRMonitor").GetComponent<HeartRateService>().heartRate);

        var logger = GetComponent<LoggerService>();
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
        this.heartrate = data;
    }

    public override string ToString()
    {
        return this.time + "," + this.heartrate;
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
        this.power = data;
        this.intervalType = interval;
    }

    public override string ToString()
    {
        return this.time + "," + this.power + "," + this.intervalType;
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
        this.distance = data;
    }

    public override string ToString()
    {
        return this.time + "," + this.distance;
    }
}
