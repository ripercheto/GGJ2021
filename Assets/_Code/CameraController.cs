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

    private void Awake()
    {
        //player.onHit.AddListener(OnPlayerHit);
        //player.onHitEnemy.AddListener(OnHitEnemy);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        destination = null;
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
