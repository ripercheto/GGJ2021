using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

partial class Game
{
    public static PlayerController Player => PlayerController.instance;
}

public partial class PlayerController : Pawn
{
    public static PlayerController instance;

    [Header("Damage")]
    public float invulnerabilityTime = 1f;
    public Collider coll;

    public UnityEvent onPlayerHit = new UnityEvent();
    public UnityEvent onEnemyHit = new UnityEvent();
    private float vulnerableTime;

    private PhysicMaterial defaultMat;

    private bool IsVulnerable => Time.time > vulnerableTime;

    protected override void Awake()
    {
        instance = this;

        base.Awake();

        defaultMat = coll.sharedMaterial;

        InitInput();
        InitMovement();
        InitAttack();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void Update()
    {
        HandleAttackCooldown();
        ReadInput();
        HandleRotationUpdate();
    }

    public override void OnHit(Vector3 from, float damage)
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

        base.OnHit(from, damage);
        vulnerableTime = Time.time + invulnerabilityTime;
        onPlayerHit.Invoke();
    }

    protected override void StartKnockBack()
    {
        base.StartKnockBack();
        coll.material = null;
    }

    protected override void EndKnockBack()
    {
        coll.material = defaultMat;
        base.EndKnockBack();
    }

    protected override void OnDeath()
    {
        Debug.LogError("DEAD, FAILED TO DELIVED ITEM, GO BACK TO SHOP");
    }
}
