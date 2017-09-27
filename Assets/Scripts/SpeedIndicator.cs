using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedIndicator : MonoBehaviour
{
    private Vector3 position;

    public float targetRPM { get; set; }

    private float currentMeanRPM;

    public void UpdatePosition(float value)
    {
        position.x += value;
    }

    public void UpdateRPM(float value)
    {
        currentMeanRPM = value;
    }

    // Use this for initialization
    void Start ()
	{
	    position = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

    }
}
