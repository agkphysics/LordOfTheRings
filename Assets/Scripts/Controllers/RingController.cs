using UnityEngine;
using System.Collections;

/// <summary>
/// Script which controls individual rings.
/// </summary>
public class RingController : MonoBehaviour
{

    public GameObject NextRing { get; set; }
    public Engine.Interval Section { get; set; }

    private BirdController playerController;
    private RingGenerator ringGenerator;
    private RowingMachineController rowingMachine;
    private SpeedIndicator speedIndicator;
    private MusicController musicController;

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

    void Update ()
    {
        float val = Mathf.Exp(-0.1f*Mathf.Clamp((transform.position.x - playerController.transform.position.x - 10), 0, float.MaxValue));
        val *= Mathf.Exp(-0.06f*Mathf.Abs(rowingMachine.MeanRPM - playerController.TargetRPM));

        //float val = Mathf.Exp(-0.2f*Mathf.Abs((ringGenerator.LastGeneratedRing.transform.position.x - playerController.transform.position.x)/playerController.GetComponent<Rigidbody>().velocity.x - musicController.NextBeatDelta));

        GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.red, Color.green, val);

        if (transform.position.x < playerController.transform.position.x - 0.5f)
        {
            ringGenerator.NewRing();
            Destroy(gameObject);
        }
	}
}
