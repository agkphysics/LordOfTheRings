using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using ProgressBar;

/// <summary>
/// Main game controller.
/// </summary>
public class Engine : MonoBehaviour {

    public enum Interval { LOW_INTENSITY, HIGH_INTENSITY };

    public GameObject worldlight, floorPrefab, ringCreator;

    private GameObject floor;

    public GUISkin skin;

    private ProgressBarBehaviour progressBarBehaviour;

    private ProgressBarBehaviour testBar;

    //GUI Bool Elements
    public bool isStarted = false;
    public bool isWarmingUp = false;
    public bool gameOver = false;
    
	bool isDead = false;
	int bestScore = 0;
	int score = 0;
    public int age = 20;
    public float warmupTime = 5;
    const int targetScore = 10000;

    // Use this for initialization
    void Awake ()
    {
		Instantiate(worldlight);
		floor = Instantiate(floorPrefab);
		Instantiate(ringCreator);
        isWarmingUp = false;
        isStarted = false;
        progressBarBehaviour = GameObject.Find("ProgressBar").GetComponent<ProgressBarBehaviour>();
    }

    public void AddToCurrentProgress()
    {
        progressBarBehaviour.IncrementValue(5);
        Debug.Log("Current percentage" + progressBarBehaviour.Value);
    }

    public void AddToCurrentScore(int value)
	{
        score += value;
        GameObject.Find("Score").GetComponent<TextMesh>().text = score.ToString();
    }

    public void CompareCurrentScoreToBest()
    {
		if(score > bestScore) bestScore = score;
	}

	public void StartGame()
    {
		isStarted = true;
        isWarmingUp = true;
        warmupTime += Time.time;
        GameObject.Find("Music").GetComponent<MusicController>().PlaySong();
    }
	
	public void Die()
    {
		isDead = true;
	}
	
    // Only called in editor
	public void Reset()
    {
        isDead = false;
        isStarted = false;
        score = 0;
        AddToCurrentScore(0);
    }

	// Update is called once per frame
	void Update ()
    {
        if(isWarmingUp)
        {
            if(Time.time > (warmupTime))
            {
                //If player is finished warmup, set warmup power average to be used for controlling player height gain on row.
                isWarmingUp = false;
                BirdController birdController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
                birdController.warmupAverage = birdController.warmupPowerSum / birdController.warmupCount;
            }
        }
        if (floor.transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x - floor.transform.localScale.x/2)
        {
            floor.transform.position += new Vector3(floor.transform.localScale.x/2, 0, 0);
        }

        //Trigger gameOver GUI when score reaches 10,000 points
        //Hides the game UI and rings as well
        if (score > targetScore)
        {
            gameOver = true;
            GameObject.Find("GUI Camera 1").SetActive(false);
            GameObject.FindGameObjectWithTag("pipecreator").SetActive(false);
        }
    }

    void OnGUI ()
    {
        GUI.skin = skin;
        if (!isStarted)
        {
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Welcome to HIITCopter!\n Start rowing to begin!"));
        }

        if (isWarmingUp)
        {
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 4), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Row to escape from light warp"));
        }

        if (isDead) {
			//show score screen gui
			GUI.Box (new Rect ((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent ("Game Over"));
			GUI.Box (new Rect ((Screen.width / 3), (Screen.height / 8 * 2), (Screen.width / 3), (Screen.height / 8)), new GUIContent ("Score" + "\t\t\t\t\t\t\t\t\t"+ "Best" + "\n" + 
			                                                                                                                          score + "\t\t\t\t\t\t\t\t\t\t" + bestScore+"\nPress 'Space' Twice..."));
		}
        if (gameOver)
        {
            //show score screen gui
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Game Over"));
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8 * 2), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Score" + "\t\t\t\t\t\t\t\t\t" + "Best" + "\n" +
                                                                                                                                      score + "\t\t\t\t\t\t\t\t\t\t" + bestScore + "\nPress 'Space' Twice..."));
        }
	}
}
