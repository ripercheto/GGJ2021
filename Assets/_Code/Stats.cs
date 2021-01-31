using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [Header("Base")]
    public float health = 1;
    public float damage = 1;
    public float attackRate = 1;
    public float movementSpeed = 10;
    public float turnFactor = 20;

    [Header("Knockback")]
    public float knockbackForce = 1;
    public float knockbackDuration = 0.5f;
    public float knockbackRecoveryDelay = 1f;
    public float knockbackRecoveryTime = 0.5f;
}