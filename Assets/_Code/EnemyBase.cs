using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    [Header("Stats")]
    public Stats stats;
    [Header("References")]
    public NavMeshAgent agent;
    public Rigidbody body;
    [Header("Ranges")]
    public float detectRange = 2;

    private void Awake()
    {
        agent.speed = stats.movementSpeed;
    }

    private void Update()
    {
        var playerPos = Game.Player.transform.position;
        var distToPlayer = Vector3.Distance(transform.position, playerPos);

        if (distToPlayer < detectRange)
        {
            agent.SetDestination(playerPos);
        }
    }

    public void OnHit(Vector3 from, float damage)
    {
        Debug.Log(name);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, detectRange);  
    }
#endif
}
