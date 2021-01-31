using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RenderCamera : MonoBehaviour
{
    public static RenderCamera instance;

    public Camera ownCamera;
    public Camera viewCam;
    public RenderTexture tex;
    public Material viewMat;


    [Min(4)]
    public float scaleFactor = 4;
    public Transform renderTarget;
    [Min(0.1f)]
    public float camDistance = 10;
    private float lastScaleFactor;

    private float xFactor, yFactor;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        CreateRT();
    }

    public static Vector3 MousePosition
    {
        get
        {
            var mousePos = Input.mousePosition;
            mousePos.x /= RenderCamera.instance.xFactor;
            mousePos.y /= RenderCamera.instance.yFactor;
            return mousePos;
        }
    }

    private void CreateRT()
    {
        if (scaleFactor <= 0)
        {
            return;
        }

        var w = Screen.width;
        var h = Screen.height;
        var bigger = Mathf.Max(w, h);
        float smaller = Mathf.Min(w, h);
        var textureWidth = 1920f / scaleFactor;
        yFactor = smaller / bigger;
        var textureHeight = yFactor * textureWidth;

        var x = Mathf.RoundToInt(textureWidth);
        var y = Mathf.RoundToInt(textureHeight);

        xFactor = Screen.width / (float)x;
        yFactor = Screen.height / (float)y;

        tex = new RenderTexture(x, y, 0)
        {
            filterMode = FilterMode.Point
        };
        viewCam.targetTexture = tex;
        viewMat.SetTexture("_MainTex", tex);

        // distance on the local z axis of the camera (i.e. the look at direction)
        Vector3[] corners = new Vector3[4]; // will hold the result

        ownCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), camDistance, Camera.MonoOrStereoscopicEye.Mono, corners);

        float zSize = (corners[0] - corners[1]).magnitude;
        float xSize = (corners[1] - corners[2]).magnitude;

        renderTarget.localPosition = new Vector3(0, 0, camDistance);
        renderTarget.localScale = new Vector3(xSize, zSize, 1);
        ownCamera.farClipPlane = camDistance;
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        var mPos = (Vector2)Input.mousePosition;
        var resol = new Vector2(Screen.width, Screen.height);
        var nPos = (Vector2)MousePosition;
        var newResol = new Vector2(tex.width, tex.height);

        var str = mPos + " | " + resol + "\n";
        str += nPos + " | " + newResol;
        //Debug.Log(str);

        var ray = Game.Cam.cam.ScreenPointToRay(nPos);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10, Color.green);
        ray = ownCamera.ScreenPointToRay(mPos);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 10, Color.red);

        if (tex == null || viewMat.GetTexture("_MainTex") == null)
        {
            CreateRT();
        }

        if (scaleFactor != lastScaleFactor)
        {
            CreateRT();
            lastScaleFactor = scaleFactor;
        }
    }
#endif
}
