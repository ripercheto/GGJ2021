using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    protected abstract Stats Stats { get; }
    [Header("References")]
    public Rigidbody body;
    public Animator animator;
    public MeshRenderer renderer;

    protected float currentHealth;
    protected float attackTimer;
    protected bool isAttacking;

    protected Quaternion targetRot = Quaternion.identity;

    private Coroutine attackRoutine;
    private Coroutine knockbackRoutine;
    private RigidbodyConstraints defaultConstraints;
    public bool HasHealth => currentHealth > 0;
    public bool isDead;
    protected bool CanAttack => !isAttacking && attackTimer <= 0 && !IsKnockedBack;
    protected bool IsKnockedBack { get; private set; }
    protected abstract Quaternion RecoverRotation { get; }

    protected virtual void Awake()
    {
        currentHealth = Stats.Health;
        defaultConstraints = body.constraints;
    }

    #region Taking damage
    protected virtual void OnDeath()
    {
        isDead = true;
    }
    public virtual void OnHit(Vector3 force, float damage)
    {
        var health = currentHealth;
        currentHealth -= damage;
        var isKillingBlow = health > 0 && currentHealth <= 0;

        if (!HasHealth && !isDead && !isKillingBlow)
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
        while (t < Stats.knockbackDuration)
        {
            t += Time.deltaTime;
            var a = t / Stats.knockbackDuration;
            var exponent = Mathf.Pow(0.001f, a);// * Time.deltaTime;
            var f = exponent * force;
            body.AddForce(f, ForceMode.VelocityChange);
            //body.MovePosition(transform.position + f);
            yield return null;
        }

        yield return new WaitForSeconds(Stats.knockbackRecoveryDelay);
        if (HasHealth)
        {
            yield return _Recover(force);
        }

        EndKnockBack();
    }

    private IEnumerator _Recover(Vector3 dir)
    {
        var startRot = transform.rotation;
        var t = 0f;
        while (t < Stats.knockbackRecoveryTime)
        {
            t += Time.deltaTime;
            var a = t / Stats.knockbackRecoveryTime;
            transform.rotation = Quaternion.Lerp(startRot, RecoverRotation, a);
            yield return null;
        }
        transform.rotation = RecoverRotation;
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

        CancelAttack();
        attackRoutine = StartCoroutine(_Attack(dir));
    }

    protected void CancelAttack(bool cleanup = false)
    {
        if (attackRoutine == null)
        {
            return;
        }
        StopCoroutine(attackRoutine);

        if (!cleanup)
        {
            return;
        }
        EndAttack();
    }

    protected virtual void PrepareAttack(Vector3 dir)
    {
        isAttacking = true;
        body.AddForce(Vector3.zero, ForceMode.VelocityChange);
    }
    protected abstract IEnumerator _Attack(Vector3 dir);
    protected virtual void EndAttack()
    {
        body.AddForce(Vector3.zero, ForceMode.VelocityChange);
        attackTimer = Stats.AttackRate;
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
    private Color lastDefaultColor;

    private void BlinkDamage()
    {
        Blink(Color.red, 3, 0.1f);
    }

    protected void Blink(Color color, int times, float duration)
    {
        if (blinkRoutine != null)
        {
            StopCoroutine(blinkRoutine);
            renderer.material.SetColor("_BaseColor", lastDefaultColor);
        }

        blinkRoutine = StartCoroutine(_Blink(color, times, duration));
    }
    private IEnumerator _Blink(Color color, int times, float colorTime)
    {
        var mat = renderer.material;
        lastDefaultColor = mat.GetColor("_BaseColor");

        for (int i = 0; i < times; i++)
        {
            mat.SetColor("_BaseColor", color);
            yield return new WaitForSeconds(colorTime);
            mat.SetColor("_BaseColor", lastDefaultColor);
            yield return new WaitForSeconds(colorTime);
        }

        blinkRoutine = null;
    }
}
