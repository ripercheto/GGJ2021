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

    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationX = 0F;
    float rotationY = 0F;

    Quaternion originalRotation;

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    private void Awake()
    {
        //player.onHit.AddListener(OnPlayerHit);
        //player.onHitEnemy.AddListener(OnHitEnemy);
    }

    private void Update()
    {

        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        Debug.Log(mouseX + "/" + mouseY);
        // Read the mouse input axis
        rotationX += mouseX * sensitivityX;
        rotationY += mouseY * sensitivityY;

        rotationX = ClampAngle(rotationX, minimumX, maximumX);
        rotationY = ClampAngle(rotationY, minimumY, maximumY);

        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

        transform.localRotation = xQuaternion * yQuaternion;

        Debug.Log(transform.localRotation);

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
