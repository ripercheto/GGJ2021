using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class PlayerController
{
    public float attackCooldown = 1;
    public float attackMovement = 2;
    public float attackTime = 0.5f;

    private float attackTimer;

    private bool CanAttack => attackTimer <= 0;
    private bool IsAttacking => attackRoutine != null;

    private Coroutine attackRoutine;

    void InitAttack()
    {

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
        attackTimer = attackCooldown;
        attackRoutine = StartCoroutine(_Attack(dir));
    }

    private IEnumerator _Attack(Vector3 dir)
    {
        var startPos = transform.position;
        var targetPos = startPos + dir * attackMovement;
        var t = 0f;
        while (t < attackTime)
        {
            t += Time.deltaTime;
            var a = t / attackTime;
            var newPos = Vector3.Lerp(startPos, targetPos, a);
            body.MovePosition(newPos);
            yield return null;
        }

        attackRoutine = null;
    }
}
