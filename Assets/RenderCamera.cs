using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RenderCamera : MonoBehaviour
{
    public Camera ownCamera;
    public Camera viewCam;
    public RenderTexture tex;
    public Material viewMat;
    [Min(4)]
    public float scaleFactor = 4;
    public Transform renderTarget;
    private float lastScaleFactor;

    // Start is called before the first frame update
    void Awake()
    {
        view = tex;
        CreateRT();
    }

    private void CreateRT()
    {
        if (scaleFactor <= 0)
        {
            return;
        }

        var x = Mathf.RoundToInt(Screen.width / scaleFactor);
        var y = Mathf.RoundToInt(Screen.height / scaleFactor);

        tex = new RenderTexture(x, y, 0)
        {
            filterMode = FilterMode.Point
        };
        viewCam.forceIntoRenderTexture = true;
        viewCam.targetTexture = tex;
        viewMat.SetTexture("_MainTex", tex);

        float camDistance = 10; // distance on the local z axis of the camera (i.e. the look at direction)
        Vector3[] corners = new Vector3[4]; // will hold the result

        ownCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camDistance, Camera.MonoOrStereoscopicEye.Mono, corners);

        float zSize = (corners[0] - corners[1]).magnitude;
        float xSize = (corners[1] - corners[2]).magnitude;

        renderTarget.localScale = new Vector3(xSize, zSize, 1);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (scaleFactor != lastScaleFactor)
        {
            CreateRT();
            lastScaleFactor = scaleFactor;
        }
#endif
    }
}
