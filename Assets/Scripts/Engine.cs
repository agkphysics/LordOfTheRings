using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Engine : MonoBehaviour {
	public GameObject camera,light,bird,floor,background,ringCreator, birdCamera, thumb;

	//private RingCollider ringCollider;
	bool hasPipeCollider;

	//GUI Bool Elements
	bool isNotStarted = true;
	bool scoreTicker = false;
	bool isDead = false;
	int bestScore = 0;
	int score = 0;
    public int age = 20;

    public object InputFieldEventSystemManager { get; private set; }


    // Use this for initialization
    void Awake () {
		//Instantiate(camera);
		Instantiate(light);
		Instantiate(floor);
        //Instantiate(background);
        //Instantiate(bird);
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
		//if(!hasPipeCollider){
		//	GameObject go = GameObject.FindWithTag("ringCollider");
		//	if(go!=null){
  //              //ringCollider = GameObject.FindWithTag("ringCollider").GetComponent<RingCollider>();
		//		hasPipeCollider = true;
		//	}
		//}


		//		if(Input.GetKey(KeyCode.E)){
		//			if(Input.GetKey(KeyCode.A)){
		//				if(Input.GetKey(KeyCode.T)){
		//					Debug.Log ("Cheat code entered");
		//					Destroy(birdCamera);
		//				}
		//			}
		//		}
	
	}



	void OnGUI () {

        if (isNotStarted)
        {
            // nameInputField = GUI.TextField(new Rect(10, 10, 200, 20), "Enter Age", 25);
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("First Person\nHIITCopter\nPress Space To Jump"));
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
