using UnityEngine;
using System.Collections;

/// <summary>
/// Script which generates rings.
/// </summary>
public class RingGenerator : MonoBehaviour {

    public GameObject ring;

    public GameObject LastGeneratedRing { get { return lastRing; } }
    public bool IsHighIntensity { get; set; }

    public int difficulty = 1;
    public float highRingSeparation = 22f;
    public int ringsPerInterval = 20;

    private Engine engine;
    private Vector3 currentPos;

    private HeartRateService.HeartStatus hrLvl;
    private bool hasInitialSpawned = false;
    private int ringCount = 0;
    private GameObject lastRing;

    private void Awake()
    {
        engine = GameObject.FindGameObjectWithTag("GameController").GetComponent<Engine>();
    }

    void Start()
    {
        IsHighIntensity = false;
        GameObject playerController = GameObject.FindGameObjectWithTag("Player");
        currentPos = Vector3.zero;
        transform.position = new Vector3(0, playerController.transform.position.y, 0);
    }

    void Update()
    {
        if(!hasInitialSpawned)
        {
            if(engine.IsStarted)
            {
                // Generate initial rings after warmup complete
                for (int i = 0; i < 5; i++)
                {
                    NewRing();
                }
                hasInitialSpawned = true;
            }
        }

        if (!engine.IsWarmingUp)
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
        GameObject generatedRing = Instantiate(ring, transform, false);
        generatedRing.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
        generatedRing.transform.localPosition = currentPos;
        generatedRing.transform.localScale = new Vector3(5, 5, 1.25f);
        generatedRing.GetComponent<RingController>().Section = IsHighIntensity ? Engine.Interval.HIGH_INTENSITY : Engine.Interval.LOW_INTENSITY;
        if (lastRing != null) lastRing.GetComponent<RingController>().NextRing = generatedRing;
        lastRing = generatedRing;

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
