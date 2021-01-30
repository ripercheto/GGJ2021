using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    [Header("Stats")]
    public Stats stats;
    public Rigidbody body;

    protected float currentHealth;

    private Coroutine knockbackRoutine;
    private RigidbodyConstraints defaultConstraints;
    protected bool HasHealth => currentHealth > 0;
    protected bool IsKnockedBack { get; private set; }

    protected virtual void Awake()
    {
        currentHealth = stats.health;
        defaultConstraints = body.constraints;
    }

    public virtual void OnHit(Vector3 force, float damage)
    {
        currentHealth -= damage;

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
        }
        knockbackRoutine = StartCoroutine(_Knockback(force));
    }

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

        yield return _Recover(force);

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
}
