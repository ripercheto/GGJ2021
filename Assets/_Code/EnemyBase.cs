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
    public float stoppingRange = 2;
    public float leaveRange = 20;
    public float inStoppingDistanceAcceleration = 50;
    [Header("Attack")]
    public float attackRadius = 45f;
    public float attackAnticipationTime = 0.15f;

    private Vector3 lastHitDir;
    private Vector3 homePosition;

    protected override Quaternion RecoverRotation => Quaternion.LookRotation(lastHitDir.normalized);

    private bool inDetectRange, inAttackRange, isOutOfLeaveRange;

    private bool hasDetectedPlayer;

    protected override void Awake()
    {
        base.Awake();
        agent.speed = stats.movementSpeed;
        agent.stoppingDistance = attackRange;

        Vector3 randPos = Random.insideUnitCircle;
        randPos.y = 0;
        randPos *= 3;
        homePosition = transform.position + randPos;

        agent.SetDestination(homePosition);
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

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

        inDetectRange = distToPlayer <= detectRange;
        inAttackRange = distToPlayer <= attackRange;
        isOutOfLeaveRange = distToPlayer > leaveRange;

        if (isOutOfLeaveRange)
        {
            GoHome();//ur drunk
            hasDetectedPlayer = false;
        }

        if (!inDetectRange && !hasDetectedPlayer)
        {
            return;
        }

        //in sight
        hasDetectedPlayer = true;
        targetRot = Quaternion.LookRotation(dirToPlayer);
        agent.SetDestination(playerPos);

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
        base.OnDeath();
        //enemy dead
        EndAttack(); //cancel attack
        StartKnockBack();
    }

    protected override void DoAttack(Vector3 dir)
    {
        if (!CanAttack)
        {
            //cant attack
            return;
        }

        if (!inAttackRange)
        {
            return;
        }

        var angle = Vector3.Angle(dir, transform.forward);
        if (angle > attackRange)
        {
            //needs to rotate to player
            return;
        }

        base.DoAttack(dir);
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

    private void GoHome()
    {
        agent.SetDestination(homePosition);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (isDead)
        {
            return;
        }

        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, detectRange);
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, leaveRange);

        UnityEditor.Handles.color = Color.red;
        var rot = Quaternion.Euler(0, -attackRadius * 0.5f, 0);
        UnityEditor.Handles.DrawWireArc(transform.position, transform.up, rot * transform.forward, attackRadius, attackRange);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + rot * transform.forward * attackRange);
        UnityEditor.Handles.DrawLine(transform.position, transform.position + Quaternion.Euler(0, attackRadius * 0.5f, 0) * transform.forward * attackRange);
    }

    /*private void OnGUI()
    {
        var str = "CanAttack: " + CanAttack +
        "\nIsAttacking: " + isAttacking +
        "\nIsKnockedBack: " + IsKnockedBack +
        "\ninDetectRange: " + inDetectRange +
        "\ninAttackRange: " + inAttackRange +
        "\ninLeaveRange: " + isOutOfLeaveRange;
        UnityEditor.EditorGUILayout.TextArea(str, UnityEditor.EditorStyles.helpBox);
    }*/
#endif
}
