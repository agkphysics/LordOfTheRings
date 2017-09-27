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
    public GUISkin skin;

    private ProgressBarBehaviour progressBarBehaviour;

    private SpeedIndicator speedIndicator;

    private ProgressBarBehaviour testBar;

	//GUI Bool Elements
	public bool isStarted = false;
    public bool isWarmingUp = false;
    public bool gameOver = false;
    
	int bestScore = 0;
	int score = 0;
    public int age = 20;
    public float warmupTime = 5;
    const int targetScore = 10000;

    private GameObject floor;

    GameObject warp;

    // Use this for initialization
    void Awake ()
    {
		Instantiate(worldlight);
		floor = Instantiate(floorPrefab);
		Instantiate(ringCreator);
        isWarmingUp = false;
        isStarted = false;
        progressBarBehaviour = GameObject.Find("ProgressBar").GetComponent<ProgressBarBehaviour>();
        speedIndicator = GameObject.Find("SpeedIndicator").GetComponent<SpeedIndicator>();
    }

    public void AddToCurrentProgress(float value)
    {
        progressBarBehaviour.IncrementValue(value);
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
	
    // Only called in editor
	public void Reset()
    {
        gameOver = false;
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
                speedIndicator.WarmUpFinished();
                BirdController birdController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
                birdController.warmupAverage = birdController.warmupPowerSum / birdController.warmupCount;
            }
        }
        if (floor.transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x - floor.transform.localScale.x/2)
        {
            floor.transform.position += new Vector3(floor.transform.localScale.x/2, 0, 0);
        }

        //Turns off and on warp effect. Temporarily using score to trigger.
        if (score > 10000)
        {
            warp.SetActive(true);
        }
        if (score < 10000)
        {
            warp.SetActive(false);
        }


        //Trigger gameOver GUI when score reaches 10,000 points
        //Hides the game UI and rings as well
        if (score > targetScore)
        {
            gameOver = true;
            GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
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

        if (gameOver)
        {
            //show score screen gui
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Game Over"));
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8 * 2), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Score" + "\t\t\t\t\t\t\t\t\t" + "Best" + "\n" +
                                                                                                                                      score + "\t\t\t\t\t\t\t\t\t\t" + bestScore + "\nPress 'Space' Twice..."));
        }
	}
}
