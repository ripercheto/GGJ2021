﻿using UnityEngine;

partial class PlayerController
{
    #region Movement
    [Header("Movement")]
    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;
    [SerializeField, Range(0f, 100f)]
    float maxAirAcceleration = 1f;

    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    Vector3 velocity, desiredVelocity;

    Vector3 contactNormal;

    int groundContactCount;

    bool OnGround => groundContactCount > 0;
    bool HasPlayerLostInput => isAttacking || IsKnockedBack;
    float minGroundDotProduct;

    void InitMovement()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);

        //rotation
        targetRot = Quaternion.identity;
    }

    private void HandleMovement()
    {
        if (isAttacking)
        {
            body.velocity = attackDir;
            return;
        }

        if (HasPlayerLostInput)
        {
            return;
        }

        UpdateState();
        AdjustVelocity();
        body.velocity = velocity;
        ClearState();
    }

    void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    void UpdateState()
    {
        velocity = body.velocity;
        if (OnGround)
        {
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = Vector3.up;
        }
    }

    void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX =
            Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ =
            Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);

        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                groundContactCount += 1;
                contactNormal += normal;
            }
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - contactNormal * Vector3.Dot(vector, contactNormal);
    }
    #endregion

    #region Rotation
    [Header("Rotation")]
    public Transform modelPivot;
    [Range(0, 1)]
    public float tiltAmount;
    private Vector3 lastLookDir;

    private Quaternion Rotation
    {
        get => modelPivot.rotation;
        set => modelPivot.rotation = value;
    }

    private void HandleRotationUpdate()
    {
        if (HasPlayerLostInput)
        {
            return;
        }
        HandleModelTargetRot();
        RotateModelTowardsTarget();
    }

    void HandleModelTargetRot()
    {
        if (body.velocity.magnitude > 1)
        {
            lastLookDir = body.velocity;
            lastLookDir.y = 0;
            lastLookDir.Normalize();
            targetRot = Quaternion.LookRotation(lastLookDir + Vector3.down * tiltAmount);
        }
        else
        {
            targetRot = Quaternion.LookRotation(transform.position + lastLookDir);
        }
    }

    private void RotateModelTowardsTarget()
    {
        Rotation = Quaternion.Lerp(Rotation, targetRot, Stats.turnFactor * Time.deltaTime);
    }
    #endregion
}