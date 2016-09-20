using UnityEngine;
using System.Collections;

public class RingController : MonoBehaviour {

    GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
	    if(this.gameObject.transform.position.x < player.transform.position.x - 0.5f)
        {
            GameObject.FindGameObjectWithTag("pipecreator").GetComponent<RingGenerator>().NewRing();
            Destroy(gameObject);
        }
	}
}
