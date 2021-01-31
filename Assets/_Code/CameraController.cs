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

    public float sideMovement = 5f;
    public float mainMoveLerpFactor = 10f;
    public float localMoveLerpFactor = 10f;
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
    public float distanceFromPlayer = 10;

    private Vector3 offset;

    private void Awake()
    {
        var dir = -transform.forward;

        offset = dir * distanceFromPlayer;
    }

    private void Start()
    {
        Game.Player.onPlayerHit.AddListener(OnPlayerHit);
        Game.Player.onEnemyHit.AddListener(OnHitEnemy);
    }
    Vector3 panTarget;
    private Vector3 camWorldPos;

    private void LateUpdate()
    {
        var p = Game.Player;
        //regular movement
        transform.position = Vector3.Lerp(transform.position, p.transform.position + offset, Settings.mainMoveLerpFactor * Time.deltaTime);

        var pMovement = p.input * Settings.sideMovement;
        if (pMovement.magnitude > 0f)
        {
            panTarget = new Vector3(pMovement.x, 0, pMovement.y);
        }
        else
        {
            panTarget = Vector3.zero;
        }

        camWorldPos = transform.position + panTarget;
        cam.transform.position = Vector3.Lerp(cam.transform.position, camWorldPos, Settings.localMoveLerpFactor * Time.deltaTime);
    }

    void OnPlayerHit()
    {
        shake.InduceStress(Settings.playerHitStress);
    }

    void OnHitEnemy()
    {
        shake.InduceStress(Settings.enemyHitStress);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(cam.transform.position, .5f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(camWorldPos, .5f);
        Gizmos.DrawLine(cam.transform.position, camWorldPos);
    }
}
