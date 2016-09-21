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

    public uint rowPace;
    public uint rowPower;
    public uint rowDistance;


	// Use this for initialization
	void Start () {
        numRowers = InitRower();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (numRowers > 0)
        {
            RowData rowData = new RowData();
            GetRowData(ref rowData);
            //Debug.Log(string.Format("Power: {0}, Pace: {1}, Distance: {2}", rowData.Power, rowData.Pace, rowData.Horizontal));
            

            rowPace = rowData.Pace;
            rowPower = rowData.Power;
            rowDistance = rowData.Horizontal;
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
