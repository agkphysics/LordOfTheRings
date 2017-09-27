using UnityEngine;
using System.Collections;

/// <summary>
/// Script which controls individual rings.
/// </summary>
public class RingController : MonoBehaviour {

    private BirdController playerController;
    private RingGenerator ringGenerator;
    private RowingMachineController rowingMachine;
    private SpeedIndicator speedIndicator;
    private MusicController musicController;

    public GameObject NextRing { get; set; }
    public Engine.Interval Section { get; set; }

	// Use this for initialization
	private void Awake ()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<RowingMachineController>();
        speedIndicator = GameObject.Find("SpeedIndicator").GetComponent<SpeedIndicator>();
        musicController = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicController>();
    }
	
    private void Start()
    {
        ringGenerator = GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>();
    }

	// Update is called once per frame
    void Update ()
    {
        float val = Mathf.Exp(-0.1f*Mathf.Abs(gameObject.transform.position.x - playerController.transform.position.x));
        float meanRPM = rowingMachine.MeanRPM;
        float rpmDiff = Mathf.Abs(rowingMachine.MeanRPM - playerController.TargetRPM);
        val *= Mathf.Exp(-0.06f*rpmDiff);

        //float val = Mathf.Exp(-0.2f*Mathf.Abs((ringGenerator.LastGeneratedRing.transform.position.x - playerController.transform.position.x)/playerController.GetComponent<Rigidbody>().velocity.x - musicController.NextBeatDelta));

        speedIndicator.UpdateRPM(meanRPM);
	    speedIndicator.targetRPM = playerController.TargetRPM;

        gameObject.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.red, Color.green, val);

        if (gameObject.transform.position.x < playerController.transform.position.x - 0.5f)
        {
            ringGenerator.NewRing();
            Destroy(gameObject);
        }
	}
}
