using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class PlayerController
{
    [Header("Attack")]
    public TriggerArea attackTriggerArea;
    public float attackTriggerDirOffset = 1;

    public float attackMovementDistance = 3;
    public float attackDuration = 0.1f;

    private float attackTimer;
    private Coroutine attackRoutine;
    private List<EnemyBase> hitEnemies = new List<EnemyBase>();

    private Vector3 attackDir;
    private Quaternion attackRotation;
    private Vector3 triggerAreaOffset;

    private bool isAttacking;
    private bool CanAttack => attackTimer <= 0;

    void InitAttack()
    {
        triggerAreaOffset = attackTriggerArea.transform.localPosition;
    }

    void HandleAttackCooldown()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void Attack(Vector3 dir)
    {
        if (!CanAttack)
        {
            return;
        }

        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }
        attackRoutine = StartCoroutine(_Attack(dir));
    }

    void PrepareAttack(Vector3 dir)
    {
        isAttacking = true;
        attackDir = dir;

        //prepare attack rotation
        lastLookDir = attackDir;
        modelPivot.localRotation = attackRotation = lookRot = targetRot = Quaternion.LookRotation(attackDir);

        attackTriggerArea.gameObject.SetActive(true);
        attackTriggerArea.transform.localPosition = triggerAreaOffset + attackDir * attackTriggerDirOffset;
        attackTriggerArea.transform.rotation = attackRotation;

        body.AddForce(Vector3.zero, ForceMode.VelocityChange);
    }

    private IEnumerator _Attack(Vector3 dir)
    {
        PrepareAttack(dir);

        var t = 0f;
        hitEnemies.Clear();
        while (t < attackDuration)
        {
            t += Time.deltaTime;
            body.AddForce(dir * attackMovementDistance, ForceMode.VelocityChange);

            var enemiesInTrigger = attackTriggerArea.InTrigger;
            foreach (var item in enemiesInTrigger)
            {
                if (hitEnemies.Contains(item))
                {
                    continue;
                }

                var dirToEnemy = (item.transform.position - transform.position).normalized;
                var force = (dirToEnemy + dir).normalized * stats.knockbackForce;
                item.OnHit(force, stats.damage);
                onEnemyHit.Invoke();
                hitEnemies.Add(item);
            }

            yield return null;
        }
        EnadAttack();
    }

    void EnadAttack()
    {
        attackTriggerArea.gameObject.SetActive(false);
        body.AddForce(Vector3.zero, ForceMode.VelocityChange);

        attackTimer = stats.attackRate;

        attackRoutine = null;
        isAttacking = false;
    }
}
