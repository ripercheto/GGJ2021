using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    [Header("Stats")]
    public Stats stats;
    public Rigidbody body;
    public Animator animator;
    public MeshRenderer renderer;

    protected float currentHealth;
    protected float attackTimer;
    protected bool isAttacking;

    protected Quaternion targetRot;

    private Coroutine attackRoutine;
    private Coroutine knockbackRoutine;
    private RigidbodyConstraints defaultConstraints;
    protected bool HasHealth => currentHealth > 0;
    protected bool CanAttack => !isAttacking && attackTimer <= 0;
    protected bool IsKnockedBack { get; private set; }
    protected abstract Quaternion RecoverRotation { get; }

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

        BlinkDamage();

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
        if (isAttacking && attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            EndAttack();
        }
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
            yield return new WaitForSeconds(stats.knockbackRecoveryDelay);
            yield return _Recover(force);
        }

        EndKnockBack();
    }

    private IEnumerator _Recover(Vector3 dir)
    {
        var startRot = transform.rotation;
        var t = 0f;
        while (t < stats.knockbackRecoveryTime)
        {
            t += Time.deltaTime;
            var a = t / stats.knockbackRecoveryTime;
            transform.rotation = Quaternion.Lerp(startRot, RecoverRotation, a);
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
        attackTimer = stats.attackRate;
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

    private Coroutine blinkRoutine;

    private void BlinkDamage()
    {
        Blink(Color.red, Color.white, 3, 0.1f);
    }

    protected void Blink(Color c1, Color c2, int times, float duration)
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
        }

        blinkRoutine = StartCoroutine(_Blink(c1, c2, times, duration));
    }
    private IEnumerator _Blink(Color c1, Color c2, int times, float colorTime)
    {
        var mat = renderer.material;
        var defaultColor = mat.GetColor("_BaseColor");

        for (int i = 0; i < times; i++)
        {
            mat.SetColor("_BaseColor", c1);
            yield return new WaitForSeconds(colorTime);
            mat.SetColor("_BaseColor", c2);
            yield return new WaitForSeconds(colorTime);
        }

        mat.SetColor("_BaseColor", defaultColor);
        blinkRoutine = null;
    }
}
