using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class PlayerController
{
    [Header("Attack")]
    public TriggerArea attackTriggerArea;
    public float attackTriggerDirOffset = 1;

    public float attackMovementForce = 20;
    public float attackDurationFailSafeTime = 0.1f;
    public float attackTravelDistance = 0.1f;

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
        Rotation = attackRotation = targetRot = Quaternion.LookRotation(dir);

        attackTriggerArea.gameObject.SetActive(true);
        attackTriggerArea.transform.localPosition = triggerAreaOffset + dir * attackTriggerDirOffset;
        attackTriggerArea.transform.rotation = attackRotation;
    }

    protected override IEnumerator _Attack(Vector3 dir)
    {
        PrepareAttack(dir);

        var startPos = transform.position;

        var t = 0f;
        hitEnemies.Clear();
        var dist = 0f;
        while (dist < attackTravelDistance && t < attackDurationFailSafeTime)
        {
            var travelVector = transform.position - startPos;
            dist = travelVector.magnitude;
            t += Time.deltaTime;
            //body.AddForce(dir * attackMovementDistance, ForceMode.VelocityChange);

            var enemiesInTrigger = attackTriggerArea.InTrigger;
            foreach (var item in enemiesInTrigger)
            {
                if (!item.HasHealth)
                {
                    continue;
                }
                if (hitEnemies.Contains(item))
                {
                    continue;
                }

                var dirToEnemy = (item.transform.position - transform.position).normalized;
                var force = (dirToEnemy + dir).normalized * Stats.knockbackForce;
                item.OnHit(force, Stats.Damage);
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
        base.EndAttack();
    }
}
