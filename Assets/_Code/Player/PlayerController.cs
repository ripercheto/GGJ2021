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

    public Item carryingItem;

    public Transform healthbar, cooldownbar;

    [SerializeField]
    private UpgradeableStats stats;
    public int Level
    {
        get => level;
        set
        {
            level = value;
            stats.SetLevel(level);
        }
    }
    private int level = 1;

    [Header("Damage")]
    public float immuneDuration = 1f;
    public Collider coll;
    public Transform healthBar, cooldownBar;

    public UnityEvent onPlayerHit = new UnityEvent();
    public UnityEvent onEnemyHit = new UnityEvent();

    private PhysicMaterial defaultMat;

    private float immuneTime;
    protected override Stats Stats => stats;
    private bool IsImmune => Time.time < immuneTime;
    protected override Color DefaultColor => Color.white;

    protected override void Awake()
    {
        instance = this;

        HandleExperience();

        base.Awake();
        UpdateHealthBar();

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
        UpdateCooldownhBar();

        HandleAttackCooldown();
        ReadInput();
        HandleRotationUpdate();
    }

    public void ResetPlayer(Vector3 targetPos)
    {
        currentHealth = Stats.Health;
        UpdateHealthBar();
        CancelAttack(true);
        CancelKnockback(true);
        transform.position = targetPos;
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
        UpdateHealthBar();
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
        Blink(Color.cyan, 8, immuneDuration / 8f);

        transform.rotation = RecoverRotation;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out var hit, 3f, LayerMask.GetMask("World")))
        {
            transform.position = hit.point;
        }

        base.EndKnockBack();
    }


    protected override void OnDeath()
    {
        base.OnDeath();
        //try again
        carryingItem = null;
        ResetPlayer(Game.Dungeon.playerSpawnPos.position);
        Game.Dungeon.OnPlayerDied();
    }

    private void UpdateHealthBar()
    {
        healthBar.transform.localScale = new Vector3(currentHealth / stats.Health, 1, 1);
    }

    private void UpdateCooldownhBar()
    {
        var a = attackTimer / stats.AttackRate;

        cooldownbar.gameObject.SetActive(a > 0.1f);
        cooldownbar.transform.localScale = new Vector3(attackTimer / stats.AttackRate, 1, 1);
    }
}
