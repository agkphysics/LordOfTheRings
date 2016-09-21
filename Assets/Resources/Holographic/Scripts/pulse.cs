using UnityEngine;
using System.Collections;

public class pulse : MonoBehaviour
{
	//An array of Objects that stores the results of the Resources.LoadAll() method
	private Object[] objects;
	//Each returned object is converted to a Texture and stored in this array
	private Texture[] textures;
	//With this Material object, a reference to the game object Material can be stored
	private Material goMaterial;
	//An integer to advance frames
	private int frameCounter = 0;	
	private float delay = 0.04f;
	private float pulse_= 0f;
	private int t=85;
	private float a=1; //alpha control
    private HeartRateService hrscript;

    void Awake()
	{
		//Get a reference to the Material of the game object this script is attached to
		this.goMaterial = this.GetComponent<Renderer>().material;
		this.GetComponent<Renderer>().material.color = new Color(this.GetComponent<Renderer>().material.color.r,this.GetComponent<Renderer>().material.color.b,this.GetComponent<Renderer>().material.color.g,.65f*a);
	}

	void Start ()
	{
        hrscript = this.GetComponent<HeartRateService>();
        pulse_ = (float) hrscript.heartRate;
        //Load all textures found on the Sequence folder, that is placed inside the resources folder
        this.objects = Resources.LoadAll("Holographic/output/pulse", typeof(Texture));

		//Initialize the array of textures with the same size as the objects array
		this.textures = new Texture[objects.Length];

		//Cast each Object to Texture and store the result inside the Textures array
		for(int i=0; i < objects.Length;i++)
		{
			this.textures[i] = (Texture)this.objects[i];
		}
	}

	void Update ()
	{t++;

        pulse_ = (float) hrscript.heartRate;
		this.transform.Find("p_text").GetComponent<TextMesh>().text=pulse_.ToString();
        delay = 1/pulse_;

        Color defaultColor = this.GetComponent<Renderer>().material.color;
        switch (hrscript.calculateHeartStatus())
        {
            case HeartRateService.HeartStatus.Resting:
                this.GetComponent<Renderer>().material.color = defaultColor;
                break;
            case HeartRateService.HeartStatus.Optimal:
                this.GetComponent<Renderer>().material.color =  new Color(0.1f, 1.2f, 0.1f, 1f);
                break;
            case HeartRateService.HeartStatus.Overexerting:
                this.GetComponent<Renderer>().material.color = new Color(3f, 0.2f, 0.2f, 1f);
                break;
        }


        StartCoroutine("PlayLoop",delay);
		
		goMaterial.mainTexture = textures[frameCounter];

	}

    //The following methods return a IEnumerator so they can be yielded:
	//A method to play the animation in a loop
    IEnumerator PlayLoop(float delay)
    {
        //Wait for the time defined at the delay parameter
        yield return new WaitForSeconds(delay);  

		//Advance one frame
		frameCounter = (++frameCounter)%textures.Length;

        //Stop this coroutine
        StopCoroutine("PlayLoop");
    }  

	//A method to play the animation just once
    IEnumerator Play(float delay)
    {
        //Wait for the time defined at the delay parameter
        yield return new WaitForSeconds(delay);  

		//If the frame counter isn't at the last frame
		if(frameCounter < textures.Length-1)
		{
			//Advance one frame
			++frameCounter;
		}

        //Stop this coroutine
        StopCoroutine("PlayLoop");
    } 

}