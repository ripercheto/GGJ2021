﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

partial class GameSettings
{
    public EnemyBase enemyPrefab;
}

public class EnemySpawner : MonoBehaviour
{
    public float range = 4;
    public int count = 5;

    private NavMeshPath path;

    private void Start()
    {
        Spawn();
        new NavMeshPath();
    }
    void Spawn()
    {

        var agent = gameObject.AddComponent<NavMeshAgent>();
        var path = agent.path;

        var maxCount = 0;
        for (int i = 0; i < count; i++)
        {
            if (maxCount > 100 * count)
            {
                Debug.LogError("Something has gone terriblt wrong wit hthe navmesh");
                return;
            }

            var randomPoint = Random.insideUnitSphere;
            randomPoint.y = 0;
            randomPoint *= range;

            if (!agent.CalculatePath(randomPoint, path))
            {
                i--;
                maxCount++;
                continue;
            }
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                i--;
                maxCount++;
                continue;
            }

            var enemy = Instantiate(Game.Settings.enemyPrefab, randomPoint, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
        }

        Destroy(agent);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.magenta;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, range);
    }
#endif
}
