using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedIndicator : MonoBehaviour
{
    public float TargetRPM { get; set; }

    public GameObject SpeedIndicatorPointer;

    private const float BorderMargin = 0.13f;
    private float currentMeanRPM;
    private Vector3 origPosition;
    private RowingMachineController rowingMachine;
    private BirdController playerController;
    private Engine engine;
    private GameObject speedIndicator, speedIndicatorSlow, speedIndicatorFast;

    public float GetCurrentXLocation()
    {
        return speedIndicator.transform.position.x;
    }

    private float GetCurrentLeftBorderXLocation()
    {
        return speedIndicatorSlow.transform.position.x;
    }

    private float GetCurrentRightBorderXLocation()
    {
        return speedIndicatorFast.transform.position.x;
    }

    public void UpdateRPM(float value)
    {
        currentMeanRPM = value;
    }

    private void Awake()
    {
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<RowingMachineController>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
        engine = GameObject.FindGameObjectWithTag("GameController").GetComponent<Engine>();
        speedIndicator = GameObject.Find("SpeedIndicator");
        speedIndicatorSlow = GameObject.Find("SpeedIndicator(Slow)");
        speedIndicatorFast = GameObject.Find("SpeedIndicator(Fast)");
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
            //if (currentMeanRPM < TargetRPM - 20 && GetCurrentXLocation() > GetCurrentLeftBorderXLocation() - BorderMargin)
            //{
            //    transform.Translate(-0.01f * Time.deltaTime, 0, 0);
            //}
            //else if (currentMeanRPM > TargetRPM + 20 && GetCurrentXLocation() < GetCurrentRightBorderXLocation() + BorderMargin)
            //{
            //    transform.Translate(0.01f * Time.deltaTime, 0, 0);
            //}

            float targetX = origPosition.x + 0.7f*(float)Math.Tanh((currentMeanRPM - TargetRPM)/20f);
            transform.Translate(new Vector3(Time.deltaTime*0.4f*(targetX - transform.localPosition.x), 0, 0), Space.Self);
        }
    }
}
