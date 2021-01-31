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

    [SerializeField]
    private UpgradeableStats stats;
    public int level = 1;

    [Header("Damage")]
    public float immuneDuration = 1f;
    public Collider coll;

    public UnityEvent onPlayerHit = new UnityEvent();
    public UnityEvent onEnemyHit = new UnityEvent();

    private PhysicMaterial defaultMat;

    private float immuneTime;
    protected override Stats Stats => stats;
    private bool IsImmune => Time.time < immuneTime;

    protected override void Awake()
    {
        instance = this;

        stats.SetLevel(level);

        base.Awake();

        body.centerOfMass = Vector3.up;
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
        if (IsImmune)
        {
            return;
        }

        if (!HasHealth)
        {
            //already dead
            return;
        }

        if (IsKnockedBack)
        {
            return;
        }

        base.OnHit(from, damage);
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
        immuneTime = Time.time + immuneDuration;
        Blink(Color.blue, 1, immuneDuration);

        transform.rotation = RecoverRotation;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hit, 3f, LayerMask.GetMask("World")))
        {
            transform.position = hit.point;
        }

        base.EndKnockBack();
    }

    protected override void OnDeath()
    {
        Debug.LogError("DEAD, FAILED TO DELIVED ITEM, GO BACK TO SHOP");
    }
}
