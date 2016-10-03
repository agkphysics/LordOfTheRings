using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Engine : MonoBehaviour {
	public GameObject camera,light,bird,floor,background,ringCreator, birdCamera, thumb;

    public GUISkin skin;
	//private RingCollider ringCollider;
	bool hasPipeCollider;

	//GUI Bool Elements
	public bool isNotStarted = true;
    public bool isWarmingUp = true;
	bool scoreTicker = false;
	bool isDead = false;
	int bestScore = 0;
	int score = 0;
    public int age = 20;
    public float warmupTime = 30;

    public object InputFieldEventSystemManager { get; private set; }


    // Use this for initialization
    void Awake () {
		//Instantiate(camera);
		Instantiate(light);
		Instantiate(floor);
        //Instantiate(background);
        //  Instantiate(bird);
		Instantiate(ringCreator);


    }

	public void AddToCurrentScore(int value)
	{
        Text scoreText = GameObject.Find("Score").GetComponent<Text>() as Text;
	    //Text scoreText = GetComponent("Score") as Text;
		score+= value;
	    if (scoreText != null)
	    {
	        scoreText.text = score.ToString();
	    }
	}

    private void GetComponent<T>(string v)
    {
        throw new NotImplementedException();
    }

    public void CompareCurrentScoreToBest(){
		if(score>bestScore) bestScore = score;
	}

	public void StartGame(){
		isNotStarted = false;
        isWarmingUp = true;
        warmupTime += Time.time;
        scoreTicker = true;
    }
	
	public void Die(){
		isDead = true;
		scoreTicker = false;
	}
	
	public void Reset(){
		isDead = false;
		isNotStarted = true;
        AddToCurrentScore(score * -1);
		GameObject go = GameObject.FindWithTag("ringCreator");
		if(go==null) Debug.Log ("ringCreator null");
		DestroyImmediate (go);
		Instantiate(ringCreator);
		//ringCollider.UpdatePipeGenReference();
	}

	// Update is called once per frame
	void Update () {
        if(isWarmingUp)
        {

            if(Time.time > (warmupTime))
            {
                isWarmingUp = false;
                BirdController birdController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
                birdController.warmupAverage = birdController.warmupPowerSum / birdController.warmupCount;
                Debug.LogError("Average power is: " + birdController.warmupAverage);
                //GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>().AddForce(Vector3.right * 20f, ForceMode.Force);
            }
        }

    }



    void OnGUI () {
        GUI.skin = skin;
        if (isNotStarted)
        {
            // nameInputField = GUI.TextField(new Rect(10, 10, 200, 20), "Enter Age", 25);
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Welcome to HIITCopter!\n Start rowing to begin!"));
        }

        if (isWarmingUp)
        {
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Row To Start The CHOOOOOOOPER"));
        }

        if (isWarmingUp)
        {
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Row To Start The CHOOOOOOOPER"));
        }
         
               

        //if (scoreTicker)
        //	GUI.Box (new Rect (Screen.width/2-25, 20, 50, 50), new GUIContent (""+score+""));		

        if (isDead) {
			//show score screen gui
			GUI.Box (new Rect ((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent ("Game Over"));
			GUI.Box (new Rect ((Screen.width / 3), (Screen.height / 8 * 2), (Screen.width / 3), (Screen.height / 8)), new GUIContent ("Score" + "\t\t\t\t\t\t\t\t\t"+ "Best" + "\n" + 
			                                                                                                                          score + "\t\t\t\t\t\t\t\t\t\t" + bestScore+"\nPress 'Space' Twice..."));
		}
	}
}
