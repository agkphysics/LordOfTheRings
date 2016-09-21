using UnityEngine;
using System.Collections;

public class RingGenerator : MonoBehaviour {

    public GameObject lowIntensityRing;
    public GameObject highIntensityRing;

	Vector3 ringOrigin = Vector3.zero;

    Vector3 nextRingPosition = new Vector3(0, 10f, 0);

    Vector3 randPos;

    public int difficulty = 1;

    int ringCount = 0;

    private Engine engine;
    bool hasInitialSpawned = false;

    public enum Intensity
    {
        Low, Optimal, OverExertion
    };

    //This is abut 36 seconds long.
    public int ringsPerInterval = 20;
    public Intensity hrLvl = Intensity.Low;

    //Vector3 randPipeSeparation;

    bool isHighIntensity = false;

    void Awake()
    {
        engine = GameObject.Find("GameObjectSpawner").GetComponent<Engine>();
    }

	void Start()
    {

	}

    void Update()
    {

        if(!hasInitialSpawned)
        {
            Debug.Log("LSIDJFGHLSDGFVBKSUJHVDBFKJHSGDF");
            if(!engine.isWarmingUp && !engine.isNotStarted)
            {
                for (int i = 0; i < 3; i++)
                {
                    NewRing();
                }
                hasInitialSpawned = true;
            }
        }

        uint rowDistance = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<Rower>().rowDistance;

        if (rowDistance < 100)
        {
            // Do Nothing
        } 
        else
        {
            //TODO
            //Implement the HR level check code here

            if (hrLvl == Intensity.OverExertion)
            {
                HandleOverExertion();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                //RingCreator();
                NewRing();
            }
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
        if (hrLvl == Intensity.OverExertion)
        {
            isHighIntensity = false;
            DecreaseDifficulty();
            return;
        } 
        // Case where in high intensity interval but low heart rate. Increase difficulty of next high intensity
        else if (isHighIntensity && hrLvl == Intensity.Low)
        {
            IncreaseDifficulty();
            isHighIntensity = false;
            return;
        }

        isHighIntensity = (isHighIntensity ? false : true);
    }

    // Called when user is overexerted 
    void HandleOverExertion()
    {
        ringCount = 0;
        isHighIntensity = false;
    }

    void DecreaseDifficulty()
    {
        if (difficulty > 0)
        {
            difficulty--;
        }
    }

    void IncreaseDifficulty()
    {
        difficulty++;
    }


	public void RingCreator()
    {
        HighIntensity();
    }

    public void HighIntensity()
    {
        //highIntensityRing.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
        //GameObject centre;
        //centre = Instantiate(highIntensityRing, nextRingPosition, Quaternion.identity) as GameObject;
        ////Instantiate Top and Bottom Pipes

        //centre.transform.parent = this.transform;
        //centre.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        //centre.transform.localScale = new Vector3(5,5,1.25f);


        //Reset height for randomization
      //  nextRingHeight = nextRingHeight;
        //nextRingDist = nextRingDist - randHeight;

        //// Calculation for adjusting difficulty
        //float nextHeightMax = 8f + ((float)difficulty * 2f);
        //float nextHeightMin = 3f + ((float)difficulty * 2f);

        ////Randomize next Positions
        //randHeight = new Vector3(0, Random.Range(nextHeightMin, nextHeightMax), 0);
        //randDist = new Vector3(10f, 0, 0);


        //nextRingPosition = new Vector3(0, GameObject.FindGameObjectWithTag("Player").transform.position.y, 0) + randHeight + randDist;
        //nextRingDist = nextRingDist + randDist + randHeight;
        highIntensityRing.GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;

        // Calculation for adjusting difficulty
        float nextHeightMax = 8f + ((float)difficulty * 2f);
        float nextHeightMin = 3f + ((float)difficulty * 2f);

        //Randomize next Positions
        randPos = new Vector3(10f,Random.Range(nextHeightMin, nextHeightMax), 0);

        nextRingPosition = new Vector3(nextRingPosition.x, nextRingPosition.y, 0) + randPos;

        GameObject centre;
        centre = Instantiate(highIntensityRing, nextRingPosition, Quaternion.identity) as GameObject;

        centre.transform.parent = this.transform;
        centre.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        centre.transform.localScale = new Vector3(5, 5, 1.25f);
    }

    public void LowIntensity()
    {
        lowIntensityRing.GetComponent<MeshRenderer>().sharedMaterial.color = Color.green;

        randPos = new Vector3(10f, Random.Range(0f, 5f), 0);

        if (ringCount > 2)
        {
            nextRingPosition = new Vector3(nextRingPosition.x, GameObject.FindGameObjectWithTag("Player").transform.position.y, 0) + randPos;
        }
        else
        {
            nextRingPosition = new Vector3(nextRingPosition.x, nextRingPosition.y, 0) + randPos;
        }

        GameObject centre;
        centre = Instantiate(lowIntensityRing, nextRingPosition, Quaternion.identity) as GameObject;

        centre.transform.parent = this.transform;
        centre.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

        centre.transform.localScale = new Vector3(5, 5, 1.25f);
    }
}
