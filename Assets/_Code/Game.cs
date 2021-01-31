using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public partial class Game : MonoBehaviour
{
    public static Game Instance;
    public static GameSettings Settings => Instance.settings;

    [SerializeField]
    private GameSettings settings;

    public float dungeonTextureScale = 0.1f;

    private void Awake()
    {
        Instance = this;

        Shader.SetGlobalFloat("Vector1_71B9E5F1", dungeonTextureScale);
    }
    private void Update()
    {
#if UNITY_EDITOR
        Shader.SetGlobalFloat("Vector1_71B9E5F1", dungeonTextureScale);
#endif
    }

}
#if UNITY_EDITOR
public static class Reload
{
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        ClearConsole();
        if (!Application.isPlaying)
        {
            return;
        }
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    static void ClearConsole()
    {
        var assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
#endif