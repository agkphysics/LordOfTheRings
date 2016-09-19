using UnityEngine;
using System.Collections;

public class RingGenerator : MonoBehaviour {

	//public GameObject bottomPipe;
	//public GameObject topPipe;

    public GameObject ring;

	//Vector3 topPipeOrigin = Vector3.up + new Vector3(0, 2, 0);
	Vector3 ringOrigin = Vector3.zero;

	Vector3 nextRingHeight = Vector3.up;
    Vector3 nextRingDist = Vector3.zero;

    //Vector3 nextBottomPipePosition = Vector3.zero;

    //vars for distance between pipes horizontally (new pair), vertically (space between pipes), and space between pipes
    //Vector3 randPipeX;
    Vector3 randHeight;
    Vector3 randDist;

    //Vector3 randPipeSeparation;

    bool isHighIntensity = false;

	void Start() {

        for (int i = 0; i < 50; i++)
        {
            if (i % 10 == 0)
            {
                isHighIntensity = true;
                ring.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
            }
            else if (i % 10 == 50)
            {
                isHighIntensity = false;
                ring.GetComponent<MeshRenderer>().sharedMaterial.color = Color.green;
            }

            if (isHighIntensity)
            {
                HighIntensity();
            }
            else
            {
                LowIntensity();
            }
        }
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.P)) {
		
			RingCreator();


		}

	}

	public void RingCreator(){
        HighIntensity();

    }

    public void HighIntensity()
    {
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
        GameObject centre;
        centre = Instantiate(ring, nextRingHeight, Quaternion.identity) as GameObject;
        //Instantiate Top and Bottom Pipes

        centre.transform.parent = this.transform;
        centre.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));


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
