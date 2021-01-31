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

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, 10 * Time.deltaTime);
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
