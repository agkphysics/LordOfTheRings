using UnityEngine;
using System.Collections;

/// <summary>
/// Script which controls individual rings.
/// </summary>
public class RingController : MonoBehaviour {

    private BirdController playerController;
    private RingGenerator ringGenerator;
    private RowingMachineController rowingMachine;

    public GameObject NextRing { get; set; }
    public Engine.Interval Section { get; set; }

	// Use this for initialization
	void Start () {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<BirdController>();
        ringGenerator = GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>();
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<RowingMachineController>();
    }
	
	// Update is called once per frame
	void Update () {
        //float val = Mathf.Exp(-0.1f*Mathf.Abs(gameObject.transform.position.x - player.transform.position.x));
        float meanRPM = rowingMachine.MeanRPM;
        float rpmDiff = Mathf.Abs(meanRPM - playerController.TargetRPM);
        float val = Mathf.Exp(-0.06f*rpmDiff);

        gameObject.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.red, Color.green, val);

        if (gameObject.transform.position.x < playerController.transform.position.x - 0.5f)
        {
            ringGenerator.NewRing();
            Destroy(gameObject);
        }
	}
}
