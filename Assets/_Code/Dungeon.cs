using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public Transform playerSpawnPos;
    public EnemySpawner[] spawners;

    public void ActivateSpawners()
    {
        foreach (var spawner in spawners)
        {
            spawner.Spawn();
        }
    }

    public void CleanUp()
    {
        foreach (var spawner in spawners)
        {
            spawner.CleanUp();
        }
    }
}
