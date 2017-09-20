using UnityEngine;
using System.Collections;

/// <summary>
/// Script which controls individual rings.
/// </summary>
public class RingController : MonoBehaviour {

    private GameObject player;
    private RingGenerator ringGenerator;
    private RowingMachineController rowingMachine;

    public Engine.Interval Section { get; set; }

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        ringGenerator = GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>();
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<RowingMachineController>();
    }
	
	// Update is called once per frame
	void Update () {
        //float val = Mathf.Exp(-0.1f*Mathf.Abs(gameObject.transform.position.x - player.transform.position.x));
        float meanRPM = rowingMachine.MeanRPM;
        float rpmDiff = Mathf.Abs(meanRPM - ringGenerator.TargetRPM);
        float val = Mathf.Exp(-0.06f*rpmDiff);

        gameObject.GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.red, Color.green, val);

        if (gameObject.transform.position.x < player.transform.position.x - 0.5f)
        {
            GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>().NewRing();
            Destroy(gameObject);
        }
	}
}
