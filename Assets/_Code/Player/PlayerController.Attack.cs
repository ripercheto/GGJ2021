using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class PlayerController
{
    [Header("Attack")]
    public TriggerArea attackTriggerArea;

    public float attackMovement = 2;
    public float attackDuration = 0.1f;

    private float attackTimer;
    private Coroutine attackRoutine;
    private List<EnemyBase> hitEnemies;

    private bool CanAttack => attackTimer <= 0;
    private bool IsAttacking => attackRoutine != null;

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
        attackRoutine = StartCoroutine(_Attack(dir));
    }

    void PrepareAttack(Vector3 dir)
    {
        attackTriggerArea.gameObject.SetActive(true);
        var rot = Quaternion.LookRotation(dir);
        attackTriggerArea.transform.rotation = rot;
        attackTriggerArea.transform.localPosition = dir;

        body.AddForce(Vector3.zero, ForceMode.VelocityChange);
    }

    private IEnumerator _Attack(Vector3 dir)
    {
        PrepareAttack(dir);

        var startPos = transform.position;
        var targetPos = startPos + dir * attackMovement;
        var t = 0f;
        hitEnemies = new List<EnemyBase>();
        while (t < attackDuration)
        {
            t += Time.deltaTime;
            var a = t / attackDuration;
            var newPos = Vector3.Lerp(startPos, targetPos, a);
            body.MovePosition(newPos);

            var enemiesInTrigger = attackTriggerArea.InTrigger;
            foreach (var item in enemiesInTrigger)
            {
                if (hitEnemies.Contains(item))
                {
                    continue;
                }

                item.OnHit(transform.position, stats.damage);
                hitEnemies.Add(item);
            }

            yield return null;
        }
        EnadAttack();
    }

    void EnadAttack()
    {
        attackRoutine = null;
        attackTimer = stats.attackRate;
        attackTriggerArea.gameObject.SetActive(false);
    }
}
