using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public Stats stats;
    public float invulnerabilityTime = 1f;

    public UnityEvent onPlayerHit = new UnityEvent();
    private float currentHealth;
    private float vulnerableTime;

    private bool HasHealth => currentHealth > 0;
    private bool IsVulnerable => Time.time > vulnerableTime;

    void Awake()
    {
        currentHealth = stats.health;

        InitMovement();
        InitInput();
    }

    void FixedUpdate()
    {
        if (IsAttacking)
        {
            return;
        }
        HandleMovement();
    }

    void Update()
    {
        HandleAttackCooldown();
        ReadInput();
    }

    public void TakeDamage(EnemyBase attacker)
    {
        if (!IsVulnerable)
        {
            //cant take damage
            return;
        }

        if (!HasHealth)
        {
            //already dead
            return;
        }

        vulnerableTime = Time.time + invulnerabilityTime;
        currentHealth -= attacker.stats.damage;
        onPlayerHit.Invoke();
    }
}
