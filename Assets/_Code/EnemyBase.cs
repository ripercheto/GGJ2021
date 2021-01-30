using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : Pawn
{
    [Header("References")]
    public NavMeshAgent agent;
    [Header("Ranges")]
    public float detectRange = 2;

    protected override void Awake()
    {
        base.Awake();
        agent.speed = stats.movementSpeed;
    }

    private void Update()
    {
        if (IsKnockedBack)
        {
            return;
        }

        var playerPos = Game.Player.transform.position;
        var distToPlayer = Vector3.Distance(transform.position, playerPos);

        if (distToPlayer < detectRange)
        {
            agent.SetDestination(playerPos);
        }
    }

    public override void OnHit(Vector3 force, float damage)
    {
        base.OnHit(force, damage);
    }

    protected override void StartKnockBack()
    {
        base.StartKnockBack();
        agent.enabled = false;
    }
    protected override void EndKnockBack()
    {
        agent.enabled = true;
        base.EndKnockBack();
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, detectRange);
    }
#endif
}
