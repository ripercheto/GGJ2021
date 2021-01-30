using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    [Header("Stats")]
    public Stats stats;
    public Rigidbody body;
    public Animator animator;

    protected float currentHealth;
    protected float attackTimer;
    protected bool isAttacking;

    private Coroutine attackRoutine;
    private Coroutine knockbackRoutine;
    private RigidbodyConstraints defaultConstraints;
    protected bool HasHealth => currentHealth > 0;
    protected bool CanAttack => attackTimer <= 0;
    protected bool IsKnockedBack { get; private set; }

    protected virtual void Awake()
    {
        currentHealth = stats.health;
        defaultConstraints = body.constraints;
    }

    #region Taking damage
    public virtual void OnHit(Vector3 force, float damage)
    {
        currentHealth -= damage;

        if (!HasHealth)
        {
            //dead
            OnDeath();
            return;
        }

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }
        knockbackRoutine = StartCoroutine(_Knockback(force));
    }
    protected abstract void OnDeath();
    #endregion
    #region Knockback
    protected virtual void StartKnockBack()
    {
        IsKnockedBack = true;
        body.constraints = RigidbodyConstraints.None;
    }
    private IEnumerator _Knockback(Vector3 force)
    {
        StartKnockBack();

        var t = 0f;
        while (t < stats.knockbackDuration)
        {
            t += Time.deltaTime;
            var a = t / stats.knockbackDuration;
            var exponent = Mathf.Pow(0.001f, a);// * Time.deltaTime;
            var f = exponent * force;
            body.AddForce(f, ForceMode.VelocityChange);
            //body.MovePosition(transform.position + f);
            yield return null;
        }

        if (HasHealth)
        {
            yield return _Recover(force);
        }

        EndKnockBack();
    }

    private IEnumerator _Recover(Vector3 dir)
    {
        var startRot = transform.rotation;
        var endRot = Quaternion.LookRotation(dir.normalized);
        var t = 0f;
        while (t < stats.knockbackRecoveryTime)
        {
            t += Time.deltaTime;
            var a = t / stats.knockbackRecoveryTime;
            transform.rotation = Quaternion.Lerp(startRot, endRot, a);
            yield return null;
        }
    }
    protected virtual void EndKnockBack()
    {
        IsKnockedBack = false;
        body.constraints = defaultConstraints;
        body.AddForce(Vector3.zero, ForceMode.VelocityChange);
    }
    #endregion
    #region Dealing Damage
    protected virtual void DoAttack(Vector3 dir)
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

    protected virtual void PrepareAttack(Vector3 dir)
    {
        isAttacking = true;
    }
    protected abstract IEnumerator _Attack(Vector3 dir);
    protected virtual void EndAttack()
    {
        attackRoutine = null;
        isAttacking = false;
    }

    protected void HandleAttackCooldown()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }
    #endregion
}
