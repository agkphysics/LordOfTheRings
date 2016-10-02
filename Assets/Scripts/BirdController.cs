using UnityEngine;
using System.Collections;

public class BirdController : MonoBehaviour {

	public GameObject ringCollider;

	public float boost = 20f;
	public float forwardMovement = 2f;
	public int upAngle=45, downAngle=280; //-80 degrees
    public float forceDivider = 8.0f; // lower is faster

    public int workoutPhase = 0; //0 menu screen, 1 warmup, 2 intervals

	public Vector3 startingPosition;
	[Range (-90,90)] public int zRotation;
	public Quaternion startingRotation;

	private bool waitingForPlayerToStart, scoreboard;

	private Engine engine;
    private int fallCount = 0;
    private int scoreShowingCount = 0;
	private float rotationAmount;

    public uint warmupDistance;
	
	void Awake(){
		engine = GameObject.Find("GameObjectSpawner").GetComponent<Engine>();
	}

	// Use this for initialization
	void Start () {
		transform.position = startingPosition;
		startingRotation = transform.rotation;

        GetComponent<Rigidbody>().useGravity = false;
		waitingForPlayerToStart = true;
        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, 0);

	}

	void Update(){

        if (Input.GetKey(KeyCode.F12))
        {
            UnityEngine.VR.InputTracking.Recenter();
        }

        //if(scoreboard){
        //    if(Input.GetKeyDown(KeyCode.Space) && scoreShowingCount<1){
        //        scoreShowingCount++;
        //    }else if(Input.GetKeyDown(KeyCode.Space)){
        //        engine.Reset();
        //        BirdReset();
        //        scoreboard = false;
        //    }
        //}else 
        if (waitingForPlayerToStart){
			if(Input.GetKeyDown(KeyCode.Space)){
				scoreShowingCount = 0;

                engine.StartGame();

				waitingForPlayerToStart = false;

                //GetComponent<Rigidbody>().freezeRotation = false;
				GetComponent<Rigidbody>().useGravity = true;
			}

		}else{
            //Remove this if block, only for testing using space bar
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    gameObject.GetComponent<RowingMachineController>().waitingRow = false;
            //    uint rowBoost = GetComponent<RowingMachineController>().waitingDistance;


            //    if (GetComponent<Rigidbody>().velocity.y < 0)
            //    {
            //        GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, 0);
            //    }
            //    GetComponent<Rigidbody>().AddForce(Vector3.up * 30.0f / 5.0f, ForceMode.Impulse);
            //    if (transform.rotation.eulerAngles.z < upAngle)
            //    {
            //        rotationAmount = upAngle - transform.rotation.eulerAngles.z;
            //        transform.RotateAround(transform.position, Vector3.forward, rotationAmount * .5f);
            //    }
            //    else if (transform.rotation.eulerAngles.z > 180)
            //    {
            //        rotationAmount = 360 - (transform.rotation.eulerAngles.z - upAngle);
            //        transform.RotateAround(transform.position, Vector3.forward, rotationAmount * .5f);
            //    }
            //    engine.AddToCurrentScore(50);
            //    fallCount = 0;
            //}
            //else 
            if (engine.isWarmingUp)
            {
                

                //Show WarmUP progress here


                if (Input.GetKeyDown(KeyCode.Space) || gameObject.GetComponent<RowingMachineController>().waitingRow)
                {
                    gameObject.GetComponent<RowingMachineController>().waitingRow = false;

                    GetComponent<Rigidbody>().AddForce(Vector3.up * GetComponent<RowingMachineController>().currentForce / (forceDivider), ForceMode.Impulse);

                    fallCount = 0;
                }

                
            }
            else if (Input.GetKeyDown(KeyCode.Space) || gameObject.GetComponent<RowingMachineController>().waitingRow)
            {
                gameObject.GetComponent<RowingMachineController>().waitingRow = false;

				if(GetComponent<Rigidbody>().velocity.y<0){
					GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x,0,0);
				}
                GetComponent<Rigidbody>().AddForce(Vector3.up * GetComponent<RowingMachineController>().currentForce / forceDivider, ForceMode.Impulse);

                engine.AddToCurrentScore(50);
				fallCount = 0;
                
            }

		}

        if (engine.isWarmingUp)
        {
            if (GetComponent<Rigidbody>().velocity.y < 0)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                Debug.Log("Else clamp the y velocity");

            }
            else if (GetComponent<Rigidbody>().velocity.y > 3.0f)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0, 3.0f, 0);
            }
            else
            {
                GetComponent<Rigidbody>().velocity = new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);
                Debug.Log("Else reachedf");

            }
        }
        else
        {
            Debug.Log("LJSHBGDFLJSDBF");
            if (GetComponent<Rigidbody>().velocity.y < -3.0f)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, -3.0f, 0);
            }
            if (GetComponent<Rigidbody>().velocity.y > 10.0f)
            {
                GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 10.0f, 0);
            }
        }
	}

	void FixedUpdate(){
		if(!waitingForPlayerToStart){
            uint rowDistance = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<Rower>().rowDistance;

            if (engine.isWarmingUp)
            {


                if (rowDistance > warmupDistance)
                {
                    engine.isWarmingUp = false;
                    GetComponent<Rigidbody>().AddForce(Vector3.right * boost, ForceMode.Force);

                }
            }

            if (rowDistance > warmupDistance)
            {
                transform.position += Vector3.right * Time.fixedDeltaTime * forwardMovement;
               
            }
		}
	}

	void OnCollisionEnter(Collision obj){
	    if (obj.gameObject.tag.Equals("ring"))
	    {

            
	    }
	    else
	    {

	    }
	}

    void StartWorkout()
    {

    }

	void BirdReset(){
		GetComponent<Rigidbody>().velocity=Vector3.zero;
		transform.position = startingPosition;
		GetComponent<Rigidbody>().rotation = startingRotation;
		GetComponent<Rigidbody>().freezeRotation = true;
	}

	void OnTriggerEnter(Collider scorebox){
		Debug.Log ("Score Increased");
        GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>().NewRing();

        engine.AddToCurrentScore(500);
		Destroy (scorebox.gameObject);
	}
}
