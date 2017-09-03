using UnityEngine;
using System.Runtime.InteropServices;

public class Rower : MonoBehaviour {

    public struct RowData
    {
        public uint Pace;
        public uint Power;
        public uint Horizontal;
    }

    [DllImport("C2API", EntryPoint = "InitRower")]
    public static extern int InitRower();      //Returns: -1: Init failed, -2: No connected rower detected, >0: number of initialised rowers

    [DllImport("C2API", EntryPoint = "GetRowData")]
    public static extern void GetRowData(ref RowData rowData);

    [DllImport("C2API", EntryPoint = "CloseRower")]
    public static extern int CloseRower();

    public int numRowers = 0;

    public uint rowPace = 0;
    public uint rowPower = 0;
    public uint rowDistance = 0;
    public bool DEBUG;



	// Use this for initialization
	void Start () {
        numRowers = InitRower();
        switch (numRowers)
        {
            case -1:
                Debug.LogWarning("Rower init failed. Switching to manual mode.");
                DEBUG = true;
                break;
            case -2:
                Debug.LogWarning("No connected rower. Switching to manual mode.");
                DEBUG = true;
                break;
            default:
                Debug.Log("Number of rowers: " + numRowers);
                break;
        }
	}
	
	// Update is called once per frame
    void Update()
    {
        if (DEBUG)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                rowPace = (uint)Random.Range(30, 40);
                rowPower = (uint)Random.Range(50, 70);
                rowDistance += (uint)Random.Range(2, 4);
            }
            return;
        }
        else
        {
            //if player has rowed inbetween updates, log row data
            if (numRowers > 0)
            {
                RowData rowData = new RowData();
                GetRowData(ref rowData);

                rowPace = rowData.Pace;
                rowPower = rowData.Power;
                rowDistance = rowData.Horizontal;
            }
        }
    }

    void OnApplicationQuit()
    {
        if (numRowers > 0)
        {
            CloseRower();
        }
    }
}
