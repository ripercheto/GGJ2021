using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public LostItem lostItemPrefab;

    public Transform playerSpawnPos;
    public EnemySpawner[] spawners;
    public Transform[] possibleItemPositions;

    private LostItem lostItemInstance;
    public void ActivateSpawners()
    {
        foreach (var spawner in spawners)
        {
            spawner.Spawn();
        }
    }

    public void SpawnItemAtRandomLocation()
    {
        var random = Random.Range(0, possibleItemPositions.Length);
        var pos = possibleItemPositions[random].position;
        lostItemInstance = Instantiate(lostItemPrefab, pos, Quaternion.identity, transform);

    }

    public void CleanUp()
    {
        foreach (var spawner in spawners)
        {
            spawner.CleanUp();
        }
    }
}
