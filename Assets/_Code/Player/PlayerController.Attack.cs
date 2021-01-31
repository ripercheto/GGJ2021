using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class PlayerController
{
    [Header("Attack")]
    public TriggerArea attackTriggerArea;
    public float attackTriggerDirOffset = 1;

    public float attackMovementForce = 20;
    public float attackDuration = 0.1f;

    private List<EnemyBase> hitEnemies = new List<EnemyBase>();

    private Vector3 attackDir;
    private Quaternion attackRotation;
    private Vector3 triggerAreaOffset;

    protected override Quaternion RecoverRotation => Quaternion.identity;

    void InitAttack()
    {
        triggerAreaOffset = attackTriggerArea.transform.localPosition;
    }

    protected override void PrepareAttack(Vector3 dir)
    {
        base.PrepareAttack(dir);
        attackDir = dir * attackMovementForce;

        //prepare attack rotation
        Rotation = attackRotation = lastLookRot = targetRot = Quaternion.LookRotation(dir);

        attackTriggerArea.gameObject.SetActive(true);
        attackTriggerArea.transform.localPosition = triggerAreaOffset + dir * attackTriggerDirOffset;
        attackTriggerArea.transform.rotation = attackRotation;

        body.AddForce(Vector3.zero, ForceMode.VelocityChange);
    }

    protected override IEnumerator _Attack(Vector3 dir)
    {
        PrepareAttack(dir);

        var t = 0f;
        hitEnemies.Clear();
        while (t < attackDuration)
        {
            t += Time.deltaTime;
            //body.AddForce(dir * attackMovementDistance, ForceMode.VelocityChange);

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
        EndAttack();
    }

    protected override void EndAttack()
    {
        attackTriggerArea.gameObject.SetActive(false);
        body.AddForce(Vector3.zero, ForceMode.VelocityChange);

        base.EndAttack();
    }
}
