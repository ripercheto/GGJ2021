using System.Collections;
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
    public int maxRandomAdditions = 3;
    private void Start()
    {
        Spawn();
    }
    void Spawn()
    {
        count += Random.Range(1, maxRandomAdditions + 1);
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
            randomPoint += transform.position;

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

            var enemy = Instantiate(Game.Settings.enemyPrefab, randomPoint, Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up), transform);
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
