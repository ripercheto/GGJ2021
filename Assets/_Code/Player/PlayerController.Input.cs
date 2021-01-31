using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class PlayerController
{
    private Plane plane;
    [HideInInspector]
    public Vector2 input;

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
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        input = Vector2.ClampMagnitude(input, 1f);
        animator.SetBool("Walking", input.magnitude > 0f);
        desiredVelocity = new Vector3(input.x, 0f, input.y) * Stats.MovementSpeed;
    }

    private void HandleMouseInput()
    {
        var mousePos = RenderCamera.MousePosition;
        var ray = Game.Cam.cam.ScreenPointToRay(mousePos);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10, Color.green);
        if (!CanAttack)
        {
            return;
        }

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

        DoAttack(dir);
    }
}
