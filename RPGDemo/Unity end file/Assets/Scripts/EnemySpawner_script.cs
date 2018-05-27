using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner_script : MonoBehaviour {

    public Enemy enemyPrefabToSpawn;
    public bool spawnAtGameStart;
    public int maxEnemiesSpawnable;
    public Transform spawnerGFX;

	// Use this for initialization
	void Start () {

		if(spawnAtGameStart)
        {
            spawnFourEnemies();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    bool spawnEnemyAtPoint(Vector3 spawnPoint, Quaternion spawnRotation)
    {
        bool successfulSpawn = false;
        Enemy tempcpy = Instantiate(enemyPrefabToSpawn, spawnPoint, spawnRotation);
        if(tempcpy)
        {
            successfulSpawn = true;
        }
        return successfulSpawn;
    }

    //spawns four Enemy objects in a '+' pattern centered on the enemyspawner gfx location
    bool spawnFourEnemies()
    {
        bool spawnSuccess = false;
        bool allSpawnsSuccessful = true;
        Vector3 north = spawnerGFX.position;
        Vector3 south = spawnerGFX.position;
        Vector3 east = spawnerGFX.position;
        Vector3 west = spawnerGFX.position;

        north.x += 1;
        south.x -= 1;
        east.z += 1;
        west.z -= 1;

        spawnSuccess = spawnEnemyAtPoint(north, spawnerGFX.rotation);
        if(spawnSuccess == false)
        {
            allSpawnsSuccessful = false;
        }
        spawnSuccess = spawnEnemyAtPoint(south, spawnerGFX.rotation);
        if (spawnSuccess == false)
        {
            allSpawnsSuccessful = false;
        }
        spawnSuccess = spawnEnemyAtPoint(east, spawnerGFX.rotation);
        if (spawnSuccess == false)
        {
            allSpawnsSuccessful = false;
        }
        spawnSuccess = spawnEnemyAtPoint(west, spawnerGFX.rotation);
        if (spawnSuccess == false)
        {
            allSpawnsSuccessful = false;
        }
        return allSpawnsSuccessful;
    }
}
