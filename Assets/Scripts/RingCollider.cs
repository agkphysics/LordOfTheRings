using UnityEngine;
using System.Collections;

public class RingCollider : MonoBehaviour {
	//private GameObject bird;
	//GameObject thumb;

	//RingGenerator pipeG;

	void Awake(){
		//bird = GameObject.FindWithTag ("Player");
		//thumb = GameObject.FindWithTag ("thumb");
		//transform.position = bird.transform.position + new Vector3(-5,1,0);
	}

    void Start()
    {
        //pipeG = GameObject.FindGameObjectWithTag("ringcreator").GetComponent<RingGenerator>();
    }


    //// Update is called once per frame
    //void FixedUpdate () {
    //	//transform.position = bird.transform.position + new Vector3(-5,1,0);
    //	//thumb.transform.position = bird.transform.position + new Vector3 (2, 0, -2.4f);
    //}

    //public void UpdatePipeGenReference()
    //{
    //    pipeG = GameObject.FindGameObjectWithTag("ringcreator").GetComponent<RingGenerator>();
    //}

    void OnCollisionEnter (Collision coll) {
        //Destroy parent pipe object

        if (coll.gameObject.tag.Equals("ring"))
            print("collided");
        //	pipeG.RingCreator ();

            //if(!coll.transform.tag.Equals("floor"))
            //   Destroy (coll.transform.parent.gameObject.transform.parent.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        print("collided");
        //characterInQuicksand = true;
    }
}
