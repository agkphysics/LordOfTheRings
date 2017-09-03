﻿using UnityEngine;
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
    public bool isWarmingUp = false;
	bool scoreTicker = false;
	bool isDead = false;
	int bestScore = 0;
	int score = 0;
    public int age = 20;
    public float warmupTime = 30;

    // Use this for initialization
    void Awake () {
		Instantiate(light);
		Instantiate(floor);
		Instantiate(ringCreator);
        isWarmingUp = false;
        isNotStarted = true;
    }

	public void AddToCurrentScore(int value)
	{
        String scoreText = GameObject.Find("Score").GetComponent<TextMesh>().text;
        int scoreValue = Int32.Parse(scoreText) + value;
        GameObject.Find("Score").GetComponent<TextMesh>().text = scoreValue.ToString();
    }

    public void CompareCurrentScoreToBest(){
		if(score > bestScore) bestScore = score;
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
        if (go==null) Debug.Log("ringCreator null");
        DestroyImmediate(go);
        Instantiate(ringCreator);
    }

	// Update is called once per frame
	void Update () {
        if(isWarmingUp)
        {
            if(Time.time > (warmupTime))
            {
                //If player is finished warmup, set warmup power average to be used for controlling player height gain on row.
                isWarmingUp = false;
                GameObject.FindGameObjectWithTag("Warp").GetComponent<WarpSpeed>().Disengage();
                BirdController birdController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
                birdController.warmupAverage = birdController.warmupPowerSum / birdController.warmupCount;
            }
        }

    }

    void OnGUI () {
        GUI.skin = skin;
        if (isNotStarted)
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
	}
}
