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

    public bool IsStarted { get; private set; }
    public bool IsWarmingUp { get; private set; }
    public bool GameOver { get; private set; }
    
    public GUISkin skin;
    public int age = 20;
    public float warmupTime = 5;

    private int bestScore = 0;
    private int score = 0;
    private int combo = 0;
    private float originalPosition;

    private const int targetScore = 1000000;

    private GameObject floor;
    private GameObject warp;
    private ProgressBarBehaviour progressBarBehaviour;
    private MusicController musicController;
    private SpeedIndicator speedIndicator;

    void Awake ()
    {
        IsWarmingUp = false;
        IsStarted = false;
        GameOver = false;

        floor = GameObject.FindGameObjectWithTag("floor");
        warp = GameObject.Find("warp");

        speedIndicator = GameObject.Find("SpeedIndicator").GetComponent<SpeedIndicator>();
        progressBarBehaviour = GameObject.Find("ProgressBar").GetComponent<ProgressBarBehaviour>();
        musicController = GameObject.Find("Music").GetComponent<MusicController>();

        originalPosition = speedIndicator.GetCurrentXLocation();
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

    public void AddToCurrentCombo(int value)
    {
        combo += value;
        GameObject.Find("Combo").GetComponent<TextMesh>().text = "COMBO " + combo.ToString();
    }

    public void ResetCombo()
    {
        combo = 0;
        GameObject.Find("Combo").GetComponent<TextMesh>().text = "COMBO " + combo.ToString();
    }

    public void CompareCurrentScoreToBest()
    {
        if(score > bestScore) bestScore = score;
    }

    public void StartGame()
    {
        IsStarted = true;
        IsWarmingUp = true;
        warmupTime += Time.time;
    }
    
    // Only called in editor
    public void Reset()
    {
        GameOver = false;
        IsStarted = false;
        score = 0;
        AddToCurrentScore(0);
    }

    void Update ()
    {
        if(IsWarmingUp)
        {
            if(Time.time > (warmupTime))
            {
                //If player is finished warmup, set warmup power average to be used for controlling player height gain on row.
                IsWarmingUp = false;
                BirdController birdController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
                birdController.WarmupAveragePower = birdController.WarmupPowerSum / birdController.WarmupCount;
                birdController.forceMultiplier = 85 / birdController.WarmupAveragePower;
                musicController.PlaySong();
            }
        }

        if (floor.transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x - floor.transform.localScale.x/2)
        {
            floor.transform.position += new Vector3(floor.transform.localScale.x/2, 0, 0);
        }

        float currentPos = speedIndicator.GetCurrentXLocation();
        if (currentPos > originalPosition + 0.14f || currentPos < originalPosition - 0.14f)
        {
            ResetCombo();
        }

        //Turns off and on warp effect. Activates when combo is 20 or higher.
        if (combo > 20)
        {
            warp.SetActive(true);
        }
        if (combo < 20)
        {
            warp.SetActive(false);
        }


        // End of HIIT routine
        if (musicController.IsEnded)
        {
            GameOver = true;
            GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
            GameObject.FindGameObjectWithTag("pipecreator").SetActive(false);
        }
    }

    void OnGUI ()
    {
        GUI.skin = skin;
        if (!IsStarted)
        {
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Welcome to HIITCopter!\n Start rowing to begin!"));
        }

        if (IsWarmingUp)
        {
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 4), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Row to escape from light warp"));
        }

        if (GameOver)
        {
            //show score screen gui
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Game Over"));
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8 * 2), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Score" + "\t\t\t\t\t\t\t\t\t" + "Best" + "\n" +
                                                                                                                                      score + "\t\t\t\t\t\t\t\t\t\t" + bestScore + "\nPress 'Space' Twice..."));
        }
    }
}
