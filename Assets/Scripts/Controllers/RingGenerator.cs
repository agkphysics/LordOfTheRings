using UnityEngine;
using System.Collections;

/// <summary>
/// Script which generates rings.
/// </summary>
public class RingGenerator : MonoBehaviour {

    public GameObject ring;

    public int difficulty = 1;
    public bool IsHighIntensity { get; set; }
    public float highRingSeparation = 22f;

    private Engine engine;
    Vector3 currentPos = Vector3.zero;

    private bool hasInitialSpawned = false;
    private int ringCount = 0;
    private GameObject lastRing;
    public GameObject LastGeneratedRing { get { return lastRing; } }

    //This is abut 36 seconds long.
    public int ringsPerInterval = 20;
    public HeartRateService.HeartStatus hrLvl;

	void Start()
    {
        IsHighIntensity = false;
        engine = GameObject.Find("GameObjectSpawner").GetComponent<Engine>();
    }

    void Update()
    {
        if(!hasInitialSpawned)
        {
            if(engine.isStarted)
            {
                // Generate initial rings after warmup complete
                for (int i = 0; i < 5; i++)
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
                if (IsHighIntensity == true)
                {
                    HandleOverExertion();
                }
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                NewRing();
            }
        }
    }

    public void NewRing()
    {
        Vector3 randOffset;
        if(IsHighIntensity)
        {
            // Calculation for adjusting difficulty
            float nextDistMax = highRingSeparation * 0.8f + (difficulty * 2f);
            float nextDistMin = highRingSeparation * 1.2f + (difficulty * 2f);
            randOffset = new Vector3(Random.Range(nextDistMin, nextDistMax), 0, 0);
        }
        else
        {
            randOffset = new Vector3(Random.Range(13f, 16f), 0, 0);
        }
        currentPos += randOffset;
        GameObject generatedRing = Instantiate(ring, currentPos, Quaternion.identity);
        generatedRing.GetComponent<RingController>().Section = IsHighIntensity ? Engine.Interval.HIGH_INTENSITY : Engine.Interval.LOW_INTENSITY;
        if (lastRing != null) lastRing.GetComponent<RingController>().NextRing = generatedRing;
        lastRing = generatedRing;
        generatedRing.transform.parent = transform;
        generatedRing.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        generatedRing.transform.localScale = new Vector3(5, 5, 1.25f);

        ringCount++;
        //if(ringCount >= ringsPerInterval)
        //{
        //    PhaseChange();
        //}
    }

    public void PhaseChange()
    {
        ringCount = 0;
        //If heart rate is dangerously high, go to a low intensity interval.
        if (hrLvl == HeartRateService.HeartStatus.Overexerting)
        {
            DecreaseDifficulty();
            IsHighIntensity = false;
        } 
        // Case where in high intensity interval but low heart rate. Increase difficulty of next high intensity
        else if (IsHighIntensity && hrLvl == HeartRateService.HeartStatus.Resting)
        {
            IncreaseDifficulty();
            IsHighIntensity = false;
        }
        else
        {
            IsHighIntensity = !IsHighIntensity;
        }
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
}
