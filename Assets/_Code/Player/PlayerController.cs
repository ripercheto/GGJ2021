using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    void Awake()
    {
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
}
