﻿using UnityEngine;
using System.Collections;

public class armor : MonoBehaviour
{
	//An array of Objects that stores the results of the Resources.LoadAll() method
	private Object[] objects;
	//Each returned object is converted to a Texture and stored in this array
	private Texture[] textures;
	//With this Material object, a reference to the game object Material can be stored
	private Material goMaterial;
	//An integer to advance frames
	private int frameCounter = 0;	
	private float a=1; //alpha control
	public static float armor_=0;

    GameObject rowingMachine;
    Rower rowerScript;

    void Awake()
	{
		//Get a reference to the Material of the game object this script is attached to
		this.goMaterial = this.GetComponent<Renderer>().material;
		this.GetComponent<Renderer>().material.color = new Color(this.GetComponent<Renderer>().material.color.r,this.GetComponent<Renderer>().material.color.b,this.GetComponent<Renderer>().material.color.g,.65f*a);
        
    }

	void Start ()
	{
        rowingMachine= GameObject.FindGameObjectWithTag("RowingMachine");
        rowerScript= rowingMachine.GetComponent<Rower>();
        armor_ = rowerScript.RowPower;
        Debug.Log(armor_);
        print(this.GetComponent<Renderer>().material.color);
		//Load all textures found on the Sequence folder, that is placed inside the resources folder
		this.objects = Resources.LoadAll("Holographic/output/main/armor", typeof(Texture));

		//Initialize the array of textures with the same size as the objects array
		this.textures = new Texture[objects.Length];

		//Cast each Object to Texture and store the result inside the Textures array
		for(int i=0; i < objects.Length;i++)
		{
			this.textures[i] = (Texture)this.objects[i];
		}
	}
	
	
	void OnGUI () {
		}

	void Update ()
	{

        armor_ = rowerScript.RowPower;
        Debug.Log(armor_);		
		frameCounter=(int)Mathf.Round((300f - armor_) / 15.781f);
		goMaterial.mainTexture = textures[frameCounter];

	}

  

}