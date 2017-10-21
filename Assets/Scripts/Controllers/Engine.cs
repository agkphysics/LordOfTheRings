using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using ProgressBar;

/// <summary>
/// Main game controller.
/// </summary>
public class Engine : MonoBehaviour
{
    public enum Interval { LOW_INTENSITY, HIGH_INTENSITY };

    public bool IsStarted { get; private set; }
    public bool IsWarmingUp { get; private set; }
    public bool GameOver { get; private set; }
    
    public GUISkin skin;
    public int age = 20;
    public float warmupTime = 5;
    public int combo = 0;
    public bool noMusicCondition = false;

    private int bestScore = 0;
    private int score = 0;

    private const int targetScore = 1000000;

    private GameObject floor;
    private GameObject warp;
    private ProgressBarBehaviour progressBarBehaviour;
    private MusicController musicController;

    void Awake ()
    {
        IsWarmingUp = false;
        IsStarted = false;
        GameOver = false;

        floor = GameObject.FindGameObjectWithTag("floor");
        warp = GameObject.FindGameObjectWithTag("Warp");
        
        progressBarBehaviour = GameObject.Find("ProgressBar").GetComponent<ProgressBarBehaviour>();
        musicController = GameObject.Find("Music").GetComponent<MusicController>();
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
                //BirdController birdController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
                //birdController.WarmupAveragePower = birdController.WarmupPowerSum / birdController.WarmupCount;
                //birdController.forceMultiplier = 85 / birdController.WarmupAveragePower;
                musicController.PlaySong();
            }
        }

        if (floor.transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x - floor.transform.localScale.x/2)
        {
            floor.transform.position += new Vector3(floor.transform.localScale.x/2, 0, 0);
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
            GameObject.FindGameObjectWithTag("GUIText").GetComponent<Text>().text = "Welcome to HIITCopter!\n Start rowing to begin!";
        }
        else if (IsWarmingUp)
        {
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 4), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Warmup time!"));
            GameObject.FindGameObjectWithTag("GUIText").GetComponent<Text>().text = "Warmup time!";
        }
        else GameObject.FindGameObjectWithTag("GUIText").GetComponent<Text>().text = "";

        if (GameOver)
        {
            //show score screen gui
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Game Over"));
            GUI.Box(new Rect((Screen.width / 3), (Screen.height / 8 * 2), (Screen.width / 3), (Screen.height / 8)), new GUIContent("Score" + "\t\t\t\t\t\t\t\t\t" + "Best" + "\n" +
                                                                                                                                      score + "\t\t\t\t\t\t\t\t\t\t" + bestScore + "\nPress 'Space' Twice..."));
            GameObject.FindGameObjectWithTag("GUIText").GetComponent<Text>().text = "Score\t\t\t\t\t\t\t\t\tBest\n" + score + "\t\t\t\t\t\t\t\t\t\t" + bestScore + "\nPress 'Space' Twice...";
        }
    }
}
