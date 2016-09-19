using UnityEngine;
using System.Collections;

public class BirdController : MonoBehaviour {

	public GameObject ringCollider;

	public float boost = 20f;
	public float forwardMovement = 2f;
	public int upAngle=45, downAngle=280; //-80 degrees

	public Vector3 startingPosition = Vector3.up*5.0f;
	[Range (-90,90)] public int zRotation;
	public Quaternion startingRotation;

	private bool waitingForPlayerToStart, scoreboard;

	private Engine engine;
	private int fallCount=0, scoreShowingCount = 0;
	private float rotationAmount;
	
	void Awake(){
		engine = GameObject.Find ("GameObjectSpawner").GetComponent<Engine>();
	}

	// Use this for initialization
	void Start () {
		transform.position = startingPosition;
		transform.RotateAround(transform.position,Vector3.forward,upAngle);
		startingRotation = transform.rotation;
		Debug.Log ("starting rotation = "+startingRotation);
		GetComponent<Rigidbody>().useGravity = false;
		waitingForPlayerToStart = true;
		//Instantiate(ringCollider);
	}

	void Update(){
		if(scoreboard){
			if(Input.GetKeyDown(KeyCode.Space) && scoreShowingCount<1){
				scoreShowingCount++;
			}else if(Input.GetKeyDown(KeyCode.Space)){
				engine.Reset();
				BirdReset();
				scoreboard = false;
			}
		}else if(waitingForPlayerToStart){
			if(Input.GetKeyDown(KeyCode.Space)){
				scoreShowingCount = 0;
				Debug.Log ("Starting Game");
				engine.StartGame();
				waitingForPlayerToStart = false;
				GetComponent<Rigidbody>().freezeRotation = false;
				GetComponent<Rigidbody>().useGravity = true;
				GetComponent<Rigidbody>().AddForce(Vector3.right*boost,ForceMode.Force);
			}

		}else{
            if (Input.GetKeyDown(KeyCode.Space) || gameObject.GetComponent<RowingMachineController>().waitingRow)
            {
                gameObject.GetComponent<RowingMachineController>().waitingRow = false;
                uint rowBoost = GetComponent<RowingMachineController>().waitingDistance;


				if(GetComponent<Rigidbody>().velocity.y<0){
					GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x,0,0);
				}
                GetComponent<Rigidbody>().AddForce(Vector3.up * GetComponent<RowingMachineController>().currentForce / 5.0f, ForceMode.Impulse);
                Debug.Log("Force equals : " + Vector3.up * GetComponent<RowingMachineController>().currentForce / 5.0f);
				if(transform.rotation.eulerAngles.z<upAngle){
					rotationAmount = upAngle - transform.rotation.eulerAngles.z;
					transform.RotateAround(transform.position,Vector3.forward,rotationAmount *.5f);
				}
				else if(transform.rotation.eulerAngles.z>180){
					rotationAmount = 360 - (transform.rotation.eulerAngles.z - upAngle);
					transform.RotateAround(transform.position,Vector3.forward,rotationAmount *.5f);
				}
                engine.AddToCurrentScore(50);
				fallCount = 0;
			}
		}

        if (GetComponent<Rigidbody>().velocity.y < -3.0f)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, -3.0f, 0);
        }
        if (GetComponent<Rigidbody>().velocity.y > 10.0f)
        {
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 10.0f, 0);
        }
	}

	void FixedUpdate(){
		if(!waitingForPlayerToStart){
			transform.position += Vector3.right*Time.fixedDeltaTime*forwardMovement;
			if(GetComponent<Rigidbody>().velocity.y<0){ //falling
				if(transform.rotation.eulerAngles.z > downAngle || transform.rotation.eulerAngles.z<180 ){
					if(fallCount<10){
						//Debug.Log ("small fall");
						transform.RotateAround(transform.position,Vector3.forward,-.5f);
					}else{
						//drop rotation until it's facing is almost down (-80 degrees?)
						//Debug.Log ("larger fall");
						transform.RotateAround(transform.position,Vector3.forward,-2);
					}
				}
				fallCount++;
			}else{
				//increase rotation on Z until bird is facing in proper up direction
				if(transform.rotation.eulerAngles.z<upAngle ){
					transform.RotateAround(transform.position,Vector3.forward,2);
				}
			}
		}
	}

	void OnCollisionEnter(Collision obj){
	    if (obj.gameObject.tag.Equals("ring"))
	    {
	        engine.AddToCurrentScore(500);
	    }
	    else
	    {
	        Debug.Log("Game Over");
	        GetComponent<Rigidbody>().useGravity = false;
	        waitingForPlayerToStart = true;
	        GetComponent<Rigidbody>().velocity = Vector3.zero;
	        GetComponent<Rigidbody>().freezeRotation = true;
	        Debug.Log("current rotation = " + transform.rotation);
	        engine.Die();
	        engine.CompareCurrentScoreToBest();
	        scoreboard = true;
	    }
	}

	void BirdReset(){
		GetComponent<Rigidbody>().velocity=Vector3.zero;
		transform.position = startingPosition;
		GetComponent<Rigidbody>().rotation = startingRotation;
		GetComponent<Rigidbody>().freezeRotation = true;
	}

	void OnTriggerEnter(Collider scorebox){
		Debug.Log ("Score Increased");
		engine.AddToCurrentScore(500);
		Destroy (scorebox.gameObject);
	}
}
