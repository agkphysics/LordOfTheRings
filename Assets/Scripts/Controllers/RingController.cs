using UnityEngine;
using System.Collections;

public class RingController : MonoBehaviour {

    GameObject player;
    RingGenerator ringGenerator;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        ringGenerator = GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>();
    }
	
	// Update is called once per frame
	void Update () {
        //float val = Mathf.Exp(-0.1f*Mathf.Abs(gameObject.transform.position.x - player.transform.position.x));
        float meanRPM = player.GetComponent<RowingMachineController>().MeanRPM;
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
