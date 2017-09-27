using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedIndicator : MonoBehaviour
{
    private Vector3 position;

    public GameObject SpeedIndicatorPointer;

    private const float BoarderMargin = 0.13f;

    private bool isWarmingUp = false;

    public float targetRPM { get; set; }

    private float currentMeanRPM;

    public float GetCurrentXLocation()
    {
        return GameObject.Find("SpeedIndicator").transform.position.x;
    }

    public float GetCurrentLeftBoarderXLocation()
    {
        return GameObject.Find("SpeedIndicator(Slow)").transform.position.x;
    }

    public float GetCurrentRightBoarderXLocation()
    {
        return GameObject.Find("SpeedIndicator(Fast)").transform.position.x;
    }

    public void UpdateRPM(float value)
    {
        currentMeanRPM = value;
    }

    public void WarmUpFinished()
    {
        isWarmingUp = true;
    }

    // Use this for initialization
    void Start ()
	{
	    position = transform.position;
	}
	
	// Update is called once per frame
    void Update()
    {
        if (!isWarmingUp) return;
        if (currentMeanRPM < targetRPM - 20 && GetCurrentXLocation() > GetCurrentLeftBoarderXLocation() - BoarderMargin)
        {
            SpeedIndicatorPointer.transform.Translate(-0.01f * Time.deltaTime, 0, 0);

        }
        else if (currentMeanRPM > targetRPM + 20 && GetCurrentXLocation() < GetCurrentRightBoarderXLocation() + BoarderMargin)
        {
            transform.Translate(0.01f * Time.deltaTime, 0, 0);
        }
    }
}
