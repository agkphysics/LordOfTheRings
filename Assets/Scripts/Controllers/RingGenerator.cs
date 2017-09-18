using UnityEngine;
using System.Collections;

public class RingGenerator : MonoBehaviour {

    public GameObject lowIntensityRing;
    public GameObject highIntensityRing;

    public GameObject ring;
    
    Vector3 currentPos = Vector3.zero;
    Vector3 randOffset;

    public int difficulty = 1;

    int ringCount = 0;

    private Engine engine;
    bool hasInitialSpawned = false;

    //This is abut 36 seconds long.
    public int ringsPerInterval = 20;
    public HeartRateService.HeartStatus hrLvl;


    public bool isHighIntensity = false;

    public float TargetRPM { get; private set; }

    void Awake()
    {
        TargetRPM = 50;
    }

	void Start()
    {
        engine = GameObject.Find("GameObjectSpawner").GetComponent<Engine>();
    }

    void Update()
    {
        if(!hasInitialSpawned)
        {
            if(!engine.isNotStarted)
            {
                //generate initial rings after warmup complete
                for (int i = 0; i < 3; i++)
                {
                    NewRing();
                }
                hasInitialSpawned = true;
            }
        }

        if (!engine.isWarmingUp)
        {
            hrLvl = GameObject.FindGameObjectWithTag("HRMonitor").GetComponent<HeartRateService>().currentHeartStatus;
            //check if user is overexerting, if they are perform overexertion handling
            if (hrLvl == HeartRateService.HeartStatus.Overexerting)
            {
                if (isHighIntensity == true)
                {
                    HandleOverExertion();
                }
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
        Vector3 randOffset;
        if(isHighIntensity)
        {
            // Calculation for adjusting difficulty
            float nextDistMax = 18f + (difficulty * 2f);
            float nextDistMin = 24f + (difficulty * 2f);
            randOffset = new Vector3(Random.Range(nextDistMin, nextDistMax), 0, 0);
        }
        else
        {
            randOffset = new Vector3(Random.Range(13f, 16f), 0, 0);
        }
        currentPos += randOffset;
        GameObject centre = Instantiate(ring, currentPos, Quaternion.identity);
        centre.transform.parent = transform;
        centre.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        centre.transform.localScale = new Vector3(5, 5, 1.25f);

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
        if (hrLvl == HeartRateService.HeartStatus.Overexerting)
        {
            DecreaseDifficulty();
            isHighIntensity = false;
        } 
        // Case where in high intensity interval but low heart rate. Increase difficulty of next high intensity
        else if (isHighIntensity && hrLvl == HeartRateService.HeartStatus.Resting)
        {
            IncreaseDifficulty();
            isHighIntensity = false;
        }
        else
        {
            isHighIntensity = !isHighIntensity;
        }
        TargetRPM = isHighIntensity ? 80 : 50;
    }

    // Called when user is overexerted 
    void HandleOverExertion()
    {
        PhaseChange();
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

    public Vector3 HighIntensity()
    {
        // Calculation for adjusting difficulty
        float nextDistMax = 15f + (difficulty*2f);
        float nextDistMin = 20f + (difficulty*2f);

        //Randomize next Positions
        randOffset = new Vector3(Random.Range(nextDistMin, nextDistMax), 0, 0);
        return randOffset;
    }

    public Vector3 LowIntensity()
    {
        randOffset = new Vector3(Random.Range(13f, 16f), 0, 0);
        return randOffset;
    }

    public void FirstRings()
    {
        lowIntensityRing.GetComponent<MeshRenderer>().sharedMaterial.color = Color.green;
        
        currentPos.y = GameObject.FindGameObjectWithTag("Player").transform.position.y;
  
        GameObject centre = Instantiate(lowIntensityRing, currentPos, Quaternion.identity);
        centre.transform.parent = transform;
        centre.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        centre.transform.localScale = new Vector3(5, 5, 1.25f);
    }
}
