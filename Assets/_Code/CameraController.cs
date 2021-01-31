using System;
using UnityEngine;

partial class GameSettings
{
    public CameraSettings camera;
}

[Serializable]
public class CameraSettings
{
    [Range(0, 1)]
    public float playerHitStress = 0.5f;
    [Range(0, 1)]
    public float enemyHitStress = 0.2f;

    public Vector2 maxMovement = new Vector2(3, 3);
}

partial class Game
{
    public CameraController cam;

    public static CameraController Cam => Instance.cam;
}

public class CameraController : MonoBehaviour
{
    CameraSettings Settings => Game.Settings.camera;

    public TraumaShake shake;
    public Camera cam;
    public Rigidbody target;
    public float distanceFromPlayer = 10;

    private Vector3 offset;

    private void Awake()
    {
        var dir = -transform.forward;

        offset = dir * distanceFromPlayer;

        Game.Player.onPlayerHit.AddListener(OnPlayerHit);
        Game.Player.onEnemyHit.AddListener(OnHitEnemy);
    }

    Vector3 leanTarget;

    private void LateUpdate()
    {
        var p = Game.Player;
        var pMovement = p.input;

        if (pMovement.sqrMagnitude > 0)
        {
            var x = Mathf.Clamp(pMovement.x, -Settings.maxMovement.x, Settings.maxMovement.x);
            var y = Mathf.Clamp(pMovement.y, -Settings.maxMovement.y, Settings.maxMovement.y);
            leanTarget = new Vector3(x, 0, y);
        }
        else
        {
            leanTarget = Vector3.zero;
        }

        transform.position = Vector3.Lerp(transform.position, target.position + offset, 10 * Time.deltaTime);

        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, leanTarget, 10 * Time.deltaTime);
    }

    void OnPlayerHit()
    {
        shake.InduceStress(Settings.playerHitStress);
    }

    void OnHitEnemy()
    {
        shake.InduceStress(Settings.enemyHitStress);
    }
}
