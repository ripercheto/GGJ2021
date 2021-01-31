﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public LostItem lostItemPrefab;

    public Transform playerSpawnPos;
    public EnemySpawner[] spawners;
    public Transform[] possibleItemPositions;

    public GameObject gate;
    public PlayerTrigger entranceTrigger;

    private LostItem lostItemInstance;
    private bool runStarted;

    private void Awake()
    {
        entranceTrigger.onEnter.AddListener(PlayerEntered);
    }

    private void PlayerEntered()
    {
        if (!runStarted)
        {
            StartRun();
        }
        else if (Game.Player.carryingItem != null)
        {
            EndRun();
        }
    }

    void StartRun()
    {
        gate.SetActive(true);
        runStarted = true;

        ActivateSpawners();
        SpawnItemAtRandomLocation();
    }

    void EndRun()
    {
        CleanUp();

        //came back with item.
        gate.SetActive(false);
        runStarted = false;
    }

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

        var itemCount = Game.Settings.AvailableItems.Length;

        if (itemCount == 0)
        {
            return;
        }
        random = Random.Range(0, itemCount);

        lostItemInstance.item = Game.Settings.AvailableItems[random];
    }

    public void CleanUp()
    {
        foreach (var spawner in spawners)
        {
            spawner.CleanUp();
        }

        if (lostItemInstance != null)
        {
            Destroy(lostItemInstance.gameObject);
        }
    }

    internal void OnPlayerDied()
    {
        EndRun();
    }
}
