using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour {

    public GameObject[] tilePrefabs;

    private Transform playerTransform;
    private float spawnX = -38.0f;
    private float tilelength = 38.0f; //Size of one tile preset
    private int amnTilesOnScreen = 8; 
    private float safeZone = 42.0f;
    private int lastPrefabIndex = 0;
    private List<GameObject> activeTiles;

	// Use this for initialization
	void Start () {
        activeTiles = new List<GameObject>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        //Spawns X amount of tiles corresponding to the variable amnTileOnScreen
        for(int i=0; i< amnTilesOnScreen; i++)
        {
            SpawnTile();
        }
	}
	
	// Update is called once per frame
	void Update () {
		if(playerTransform.position.x - safeZone > (spawnX - amnTilesOnScreen * tilelength))
        {
            SpawnTile();
            DeleteTile();
        }
	}

    //Spawns a tile in the X-axis (right direction)
    private void SpawnTile(int prefabIndex = -1)
    {
        GameObject spawn;
        spawn = Instantiate(tilePrefabs[RandomPrefabIndex()]) as GameObject;
        spawn.transform.SetParent(transform);
        spawn.transform.position = Vector3.right * spawnX;
        spawnX += tilelength;
        activeTiles.Add(spawn);
    }

    //Deletes a tile
    private void DeleteTile()
    {
        Destroy(activeTiles[0]);
        activeTiles.RemoveAt(0);
    }

    //Randomizes the tiles being spawned so the same tile does not spawn back to back
    private int RandomPrefabIndex()
    {
        if(tilePrefabs.Length <= 1)
        {
            return 0;
        }

        int randomIndex = lastPrefabIndex;
        while (randomIndex == lastPrefabIndex)
        {
            randomIndex = Random.Range(0, tilePrefabs.Length);
        }
        lastPrefabIndex = randomIndex;
        return randomIndex;
    }
}
