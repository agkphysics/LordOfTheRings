using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSpawner : MonoBehaviour {

    public GameObject[] tilePrefabs;

    private Transform playerTransform;
    private float spawnX = 0.0f;
    private float tilelength = 370.0f; //Size of the water tile
    private int amnTilesOnScreen = 2;
    private float safeZone = 375.0f;
    private int lastPrefabIndex = 0;
    private List<GameObject> activeTiles;

    // Use this for initialization
    void Start()
    {
        activeTiles = new List<GameObject>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //Spawns X amount of tiles corresponding to the variable amnTileOnScreen
        for (int i = 0; i < amnTilesOnScreen; i++)
        {
            SpawnTile();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform.position.x - safeZone > (spawnX - amnTilesOnScreen * tilelength))
        {
            SpawnTile();
            DeleteTile();
        }
    }

    //Spawns a tile in the X-axis (right direction)
    private void SpawnTile(int prefabIndex = -1)
    {
        GameObject go;
        go = Instantiate(tilePrefabs[0]) as GameObject;
        go.transform.SetParent(transform);
        go.transform.position = Vector3.right * spawnX;
        spawnX += tilelength;
        activeTiles.Add(go);
    }

    //Deletes a tile
    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }


}
