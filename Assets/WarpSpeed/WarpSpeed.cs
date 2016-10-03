using UnityEngine;
using System.Collections;

public class WarpSpeed : MonoBehaviour {
	public float WarpDistortion;
	public float Speed;
	ParticleSystem particles;
	ParticleSystemRenderer rend;
	bool isWarping;

	void Awake()
	{
		particles = GetComponent<ParticleSystem>();
		rend = particles.GetComponent<ParticleSystemRenderer>();
        Engage();
	}

	void Update()
	{
		//if(isWarping && !atWarpSpeed())
		//{
		//	rend.velocityScale = WarpDistortion * (Time.deltaTime * Speed);
		//}

		//if(!isWarping && !atNormalSpeed())
		//{
		//	rend.velocityScale = WarpDistortion * (Time.deltaTime * Speed);
		//}
	}

	public void Engage()
	{
		isWarping = true;
        rend.velocityScale = WarpDistortion * (Time.deltaTime * Speed);
    }

	public void Disengage()
	{
		isWarping = false;
        rend.velocityScale = 0;
    }

	bool atWarpSpeed()
	{
		return rend.velocityScale < WarpDistortion;
	}

	bool atNormalSpeed()
	{
		return rend.velocityScale > 0;
	}
}
