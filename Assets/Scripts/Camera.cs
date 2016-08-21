using UnityEngine;
using System.Collections;

/// <summary>
/// Camera.
/// 
/// Represents the player's POV camera
/// </summary>
public class Camera : MonoBehaviour {

	GameObject player;
	Vector3 offset;

	void Start()
	{
		//Find player and find the offset between camera and player
		player = GameObject.FindWithTag ("Player");
		offset = transform.position - player.transform.position;
	}
		

	void LateUpdate()
	{
		//Move camera to player's position
		transform.position = player.transform.position + offset;
	}
}
