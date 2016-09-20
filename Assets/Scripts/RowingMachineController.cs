using UnityEngine;
using System.Collections;

public class RowingMachineController : MonoBehaviour {

    GameObject rowingMachine;

    uint oldDistance;
    uint oldPower;
    uint oldPace;

    public bool waitingRow = false;
    public uint waitingDistance;
    public uint currentForce;


	// Use this for initialization
	void Start () {
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine");
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        uint distance = rowingMachine.GetComponent<Rower>().rowDistance;
        uint pace = rowingMachine.GetComponent<Rower>().rowPace;
        uint power = rowingMachine.GetComponent<Rower>().rowPower;

        if(distance == 0 || pace == 0 || power == 0)
        {
            return;
        }

        currentForce = power;

        if(oldPace != pace || oldPower != power)
        {
            Row(distance, power, pace);
        }
	}

    void Row(uint distance, uint power, uint pace)
    {
        waitingDistance = distance - oldDistance;
        waitingRow = true;

        oldDistance = distance;
        oldPower = power;
        oldPace = pace;
    }
}
