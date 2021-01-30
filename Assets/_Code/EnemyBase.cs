using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : Pawn
{
    [Header("References")]
    public NavMeshAgent agent;
    [Header("Ranges")]
    public float detectRange = 12;
    public float attackRange = 2;

    protected override void Awake()
    {
        base.Awake();
        agent.speed = stats.movementSpeed;
        agent.stoppingDistance = attackRange;
    }

    private void Update()
    {
        if (IsKnockedBack)
        {
            return;
        }

        var playerPos = Game.Player.transform.position;
        var dirToPlayer = playerPos - transform.position;
        var distToPlayer = dirToPlayer.magnitude;

        if (distToPlayer < detectRange)
        {
            transform.rotation = Quaternion.LookRotation(dirToPlayer);
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
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, attackRange);
    }
#endif
}
