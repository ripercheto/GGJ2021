using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class PlayerController
{
    private Plane plane;
    private Vector2 playerInput;

    private void InitInput()
    {
        plane = new Plane(Vector3.up, Vector3.zero);
    }

    private void ReadInput()
    {
        HandleVelocity();
        HandleMouseInput();
    }

    private void HandleVelocity()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * stats.movementSpeed;
    }

    private void HandleMouseInput()
    {
        if (!CanAttack)
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        var mousePos = Input.mousePosition / RenderCamera.instance.scaleFactor;
        var ray = Game.Cam.cam.ScreenPointToRay(mousePos);

        if (!plane.Raycast(ray, out var enter))
        {
            return;
        }
        var hitPos = ray.GetPoint(enter);

        var dir = hitPos - transform.position;
        dir.y = 0;
        dir.Normalize();

        Attack(dir);
    }
}
