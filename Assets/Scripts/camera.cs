using UnityEngine;
using System.Collections;

public class camera : MonoBehaviour {

	public GameObject player;
	Vector3 offset;

	void Start()
	{
		offset = transform.position - player.transform.position;
	}

	void LateUpdate()
	{
		offset = transform.position - player.transform.position;
		transform.position = player.transform.position + offset;
		Debug.Log ("NewPos:" + transform.position);
	}
}
