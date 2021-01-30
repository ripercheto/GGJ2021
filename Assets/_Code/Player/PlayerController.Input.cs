using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class PlayerController
{
    public Camera viewCam;
    private Plane plane;

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
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
    }

    private void HandleMouseInput()
    {
        var mousePos = Input.mousePosition;
        var ray = viewCam.ScreenPointToRay(mousePos);

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10, Color.red);
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }


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
