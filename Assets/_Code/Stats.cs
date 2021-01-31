using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    public float turnFactor = 20;
    [Header("Base")]

    [SerializeField]
    protected float health = 1;
    [SerializeField]
    protected float damage = 1;
    [SerializeField]
    protected float attackRate = 1;
    [SerializeField]
    protected float movementSpeed = 10;

    [Header("Knockback")]
    public float knockbackForce = 1;
    public float knockbackDuration = 0.5f;
    public float knockbackRecoveryDelay = 1f;
    public float knockbackRecoveryTime = 0.5f;

    public virtual float Health => health;
    public virtual float Damage => damage;
    public virtual float AttackRate => attackRate;
    public virtual float MovementSpeed => movementSpeed;
}

[System.Serializable]
public class UpgradeableStats : Stats
{
    int level;
    public void SetLevel(int level)
    {
        this.level = Mathf.Clamp(level, 1, 100);
    }

    public float healthPerLevel = 1f;
    public float damagePerLevel = 1f;
    public float attackRatePerLevel = 1f;
    public float movementSpeedPerLevel = 1f;

    public override float Health => health + ((level - 1) * healthPerLevel);
    public override float Damage => damage + ((level - 1) * damagePerLevel);
    public override float AttackRate => attackRate + ((level - 1) * attackRatePerLevel);
    public override float MovementSpeed => movementSpeed + ((level - 1) * movementSpeedPerLevel);
}