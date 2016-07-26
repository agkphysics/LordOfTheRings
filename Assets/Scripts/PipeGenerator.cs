using UnityEngine;
using System.Collections;

public class PipeGenerator : MonoBehaviour {

	public GameObject bottomPipe;
	public GameObject topPipe;

	Vector3 topPipeOrigin = Vector3.up + new Vector3(0, 2, 0);
	Vector3 bottomPipeOrigin = Vector3.zero;

	Vector3 nextTopPipePosition = Vector3.up;
	Vector3 nextBottomPipePosition = Vector3.zero;

	//vars for distance between pipes horizontally (new pair), vertically (space between pipes), and space between pipes
	Vector3 randPipeX;
	Vector3 randPipeY;
	Vector3 randPipeSeparation;

	void Start() {

		for (int i = 0; i < 5; i++)
						PipeCreator ();

	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.P)) {
		
			PipeCreator();


		}

	}

	public void PipeCreator(){

		//GameObject pipes = new GameObject();
		//pipes.tag = "pipe";
		//pipes.transform.parent = this.transform;
		GameObject top, bottom;

		//Instantiate Top and Bottom Pipes
		bottom = Instantiate(bottomPipe, nextBottomPipePosition, Quaternion.identity) as GameObject;
		top = Instantiate(topPipe, nextTopPipePosition, Quaternion.identity) as GameObject;
		bottom.transform.parent = this.transform;
		top.transform.parent = this.transform;

		
		//Reset height for randomization
		nextTopPipePosition = nextTopPipePosition - randPipeY - randPipeSeparation;
		nextBottomPipePosition = nextBottomPipePosition - randPipeY;
		
		//Randomize next Positions
		randPipeX = new Vector3(Random.Range(4f, 5f), 0, 0);
		randPipeY = new Vector3(0, Random.Range(-2f, 2f), 0);

		randPipeSeparation = new Vector3(0, Random.Range(0f, 1f), 0);
		
		
		
		nextTopPipePosition = nextTopPipePosition + randPipeX + randPipeY + randPipeSeparation;
		nextBottomPipePosition = nextBottomPipePosition + randPipeX + randPipeY;
	}
}
