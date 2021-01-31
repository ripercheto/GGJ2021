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
    public float attackRange = 3;
    public float stoppingDistance = 2;
    public float inStoppingDistanceAcceleration = 50;
    [Header("Attack")]
    public float attackRadius = 45f;
    public float attackAnticipationTime = 0.15f;

    private float accelerationDefault;

    private Vector3 lastHitDir;

    protected override Quaternion RecoverRotation => Quaternion.LookRotation(lastHitDir.normalized);

    protected override void Awake()
    {
        base.Awake();
        agent.speed = stats.movementSpeed;
        //agent.stoppingDistance = stoppingDistance;
        accelerationDefault = agent.acceleration;
        Vector3 randPos = Random.insideUnitCircle;
        randPos.y = 0;
        randPos *= 3;
        agent.SetDestination(transform.position + randPos);
    }

    private void Update()
    {
        HandleAttackCooldown();

        if (IsKnockedBack || isAttacking)
        {
            return;
        }

        LookTowardsTarget();

        var playerPos = Game.Player.transform.position;
        var dirToPlayer = playerPos - transform.position;
        dirToPlayer.y = 0;
        var distToPlayer = dirToPlayer.magnitude;

        agent.acceleration = distToPlayer < stoppingDistance ? inStoppingDistanceAcceleration : accelerationDefault;

        if (distToPlayer > detectRange)
        {
            return;
        }

        //in sight
        targetRot = Quaternion.LookRotation(dirToPlayer);
        agent.SetDestination(playerPos);

        if (!CanAttack)
        {
            //cant attack
            return;
        }

        if (distToPlayer > attackRange)
        {
            return;
        }

        var angle = Vector3.Angle(dirToPlayer, transform.forward);
        if (angle > attackRange)
        {
            //needs to rotate to player
            return;
        }

        //in attack range
        DoAttack(dirToPlayer);
    }

    private void LookTowardsTarget()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, stats.turnFactor * Time.deltaTime);
    }

    private bool IsPlayerInRange(Vector3 dir)
    {
        var distToPlayer = dir.magnitude;
        if (distToPlayer >= attackRange)
        {
            return false;
        }

        var angle = Vector3.Angle(dir, transform.forward);
        if (angle > attackRange)
        {
            //needs to rotate to player
            return false;
        }
        return true;
    }

    #region KnockBack
    protected override void StartKnockBack()
    {
        base.StartKnockBack();
        agent.enabled = false;
    }
    protected override void EndKnockBack()
    {
        agent.enabled = true;
        base.EndKnockBack();
        if (!HasHealth)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    public override void OnHit(Vector3 force, float damage)
    {
        base.OnHit(force, damage);
        lastHitDir = force.normalized;
        animator.ResetTrigger("Attack");
        animator.Play("Idle", 0);
    }

    protected override void OnDeath()
    {
        //enemy dead
        StartKnockBack();
    }

    protected override void PrepareAttack(Vector3 dir)
    {
        base.PrepareAttack(dir);
        animator.SetTrigger("Attack");
        agent.SetDestination(transform.position);
    }

    protected override IEnumerator _Attack(Vector3 dir)
    {
        PrepareAttack(dir);
        yield return new WaitForSeconds(attackAnticipationTime);
        if (IsPlayerInRange(dir))
        {
            //check range again
            Game.Player.OnHit(dir * stats.knockbackForce, stats.damage);
        }
        EndAttack();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, detectRange);
        UnityEditor.Handles.color = Color.red;
        //UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, attackRange);

        UnityEditor.Handles.DrawWireArc(transform.position, transform.up, Quaternion.Euler(0, -attackRadius * 0.5f, 0) * transform.forward, attackRadius, attackRange);
    }
#endif
}
