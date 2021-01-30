using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour
{
    [Header("Stats")]
    public Stats stats;
    public float attackHitForce = 3;
    public float knockbackDuration = 1;
    public Rigidbody body;

    protected float currentHealth;

    private Coroutine knockbackRoutine;
    protected bool HasHealth => currentHealth > 0;
    protected bool IsKnockedBack { get; private set; }

    protected virtual void Awake()
    {
        currentHealth = stats.health;
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
    }
    private IEnumerator _Knockback(Vector3 force)
    {
        StartKnockBack();
        var startPos = transform.position;
        var targetPos = startPos + force;
        var t = 0f;
        while (t < knockbackDuration)
        {
            t += Time.deltaTime;
            var a = t / knockbackDuration;

            var f = Mathf.Pow(a, -1) * force * Time.deltaTime;
            body.MovePosition(transform.position + f);
            yield return null;
        }
        EndKnockBack();
    }
    protected virtual void EndKnockBack()
    {
        IsKnockedBack = false;
    }
}
