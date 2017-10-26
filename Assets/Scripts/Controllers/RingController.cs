using UnityEngine;

/// <summary>
/// Script which controls individual rings.
/// </summary>
public class RingController : MonoBehaviour
{
    public GameObject NextRing { get; set; }
    public Engine.Interval Section { get; set; }

    private PlayerController playerController;
    private RingGenerator ringGenerator;
    private RowingMachineController rowingMachine;

    private void Awake ()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rowingMachine = GameObject.FindGameObjectWithTag("RowingMachine").GetComponent<RowingMachineController>();
    }
	
    private void Start()
    {
        ringGenerator = GameObject.FindGameObjectWithTag("RingCreator").GetComponent<RingGenerator>();
    }

    void Update ()
    {
        float val = Mathf.Exp(-0.1f*Mathf.Clamp((transform.position.x - playerController.transform.position.x - 10), 0, float.MaxValue));
        val *= Mathf.Exp(-0.06f * Mathf.Abs(rowingMachine.MeanRPM - playerController.TargetRPM));

        GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.red, Color.green, val);

        if (transform.position.x < playerController.transform.position.x - 0.5f)
        {
            ringGenerator.NewRing();
            Destroy(gameObject);
        }
	}
}
