using UnityEngine;
using System.Collections;

public class RingGenerator : MonoBehaviour {

    public GameObject ring;

	Vector3 ringOrigin = Vector3.zero;

	Vector3 nextRingHeight = Vector3.up;
    Vector3 nextRingDist = Vector3.zero;

    Vector3 randHeight;
    Vector3 randDist;

    int ringCount = 0;

    //This is abut 36 seconds long.
    public int ringsPerInterval = 20;
    int hrLvl = 1;

    //Vector3 randPipeSeparation;

    bool isHighIntensity = false;

	void Start() {
        for (int i = 0; i < 3; i++)
        {
            NewRing();
        }
	}

    void Update()
    {
        //TODO
        //Implement the HR level check code here

        if (hrLvl == 4)
        {
            PhaseChange(false);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //RingCreator();
            NewRing();
        }
    }

    public void NewRing()
    {
        if(isHighIntensity)
        {
            HighIntensity();
        }
        else
        {
            LowIntensity();
        }

        ringCount++;
        if(ringCount >= ringsPerInterval)
        {
            PhaseChange();
        }
    }

    void PhaseChange()
    {
        ringCount = 0;
        //If heart rate is dangerously high, go to a low intensity interval.
        if(hrLvl == 4)
        {
            isHighIntensity = false;
            return;
        }

        isHighIntensity = (isHighIntensity ? false : true);
    }

    void PhaseChange(bool toHigh)
    {
        ringCount = 0;
        isHighIntensity = toHigh;
    }



	public void RingCreator(){
        HighIntensity();

    }

    public void HighIntensity()
    {
        ring.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
        GameObject centre;
        centre = Instantiate(ring, nextRingHeight, Quaternion.identity) as GameObject;
        //Instantiate Top and Bottom Pipes

        centre.transform.parent = this.transform;
        centre.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        centre.transform.localScale = new Vector3(5,5,1.25f);


        //Reset height for randomization
      //  nextRingHeight = nextRingHeight;
        nextRingDist = nextRingDist - randHeight;

        //Randomize next Positions
        randHeight = new Vector3(0, Random.Range(0, 10f), 0);
        randDist = new Vector3(Random.Range(4f, 5f), 0, 0);


        nextRingHeight = nextRingHeight + randHeight + randDist;
        nextRingDist = nextRingDist + randDist + randHeight;


    }

    public void LowIntensity()
    {
        ring.GetComponent<MeshRenderer>().sharedMaterial.color = Color.green;

        GameObject centre;
        centre = Instantiate(ring, nextRingHeight, Quaternion.identity) as GameObject;
        //Instantiate Top and Bottom Pipes

        centre.transform.parent = this.transform;
        centre.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        centre.transform.localScale = new Vector3(5, 5, 1.25f);

        //Reset height for randomization
        nextRingHeight = nextRingHeight - randHeight;
        nextRingDist = nextRingDist - randHeight;

        //Randomize next Positions
        randHeight = new Vector3(0, Random.Range(0, 5f), 0);
        randDist = new Vector3(Random.Range(4f, 5f), 0, 0);

        nextRingHeight = nextRingHeight + randHeight + randDist;
        nextRingDist = nextRingDist + randDist + randHeight;
    }
}
