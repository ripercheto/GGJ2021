using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour
{
    Camera customCam;

    private void Start() {
        customCam = Camera.main;
    }
    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate() {
        transform.rotation = customCam.transform.rotation;
    }
}
