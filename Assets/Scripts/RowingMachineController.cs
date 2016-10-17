using UnityEngine;
using System.Collections;

public class RowingMachineController : MonoBehaviour {

    GameObject rowingMachine;

    uint oldDistance;
    uint oldPower;
    uint oldPace;

    public bool waitingRow = false;
    public uint waitingDistance;
    public uint distanceTravelled; 
    public uint currentForce;


	// Use this for initialization
	void Start () {
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //Check if user has performed a row.
        uint distance = rowingMachine.GetComponent<Rower>().rowDistance;
        uint pace = rowingMachine.GetComponent<Rower>().rowPace;
        uint power = rowingMachine.GetComponent<Rower>().rowPower;

        if(distance == 0 || pace == 0 || power == 0)
        {
            return;
        }

        currentForce = power;
        //Compare users previous pace and power to their new pace and power to check for difference
        //Currently no way of just getting if the user has rowed so detection done by looking for differences
        if(oldPace != pace || oldPower != power)
        {
            
            Row(distance, power, pace);
        }
	}

    void Row(uint distance, uint power, uint pace)
    {
        distanceTravelled = distance;
        waitingDistance = distance - oldDistance;
        waitingRow = true;

        oldDistance = distance;
        oldPower = power;
        oldPace = pace;
    }
}
