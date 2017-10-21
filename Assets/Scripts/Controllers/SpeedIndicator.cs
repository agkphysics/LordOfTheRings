using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedIndicator : MonoBehaviour
{
    public GameObject SpeedIndicatorPointer;

    public float TargetRPM { get; set; }
    public float CurrentXLocation { get { return speedIndicator.transform.position.x; } }
    
    private float currentMeanRPM;
    private Vector3 origPosition;
    private RowingMachineController rowingMachine;
    private BirdController playerController;
    private Engine engine;
    private GameObject speedIndicator;

    private void Awake()
    {
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<RowingMachineController>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
        engine = GameObject.FindGameObjectWithTag("GameController").GetComponent<Engine>();
        speedIndicator = GameObject.Find("SpeedIndicator");
    }

    // Use this for initialization
    private void Start ()
	{
	    origPosition = transform.localPosition;
	}
	
	// Update is called once per frame
    private void Update()
    {
        currentMeanRPM = rowingMachine.MeanRPM;
        TargetRPM = playerController.TargetRPM;
        
        // Speed indicator is inappropriate for warmup time.
        if (engine.IsStarted && !engine.IsWarmingUp)
        {
            float targetX = origPosition.x + 0.7f*(float)Math.Tanh((currentMeanRPM - TargetRPM)/20f);
            transform.Translate(new Vector3(Time.deltaTime*0.4f*(targetX - transform.localPosition.x), 0, 0), Space.Self);
        }
    }
}
