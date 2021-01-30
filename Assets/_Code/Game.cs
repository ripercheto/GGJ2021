using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Game : MonoBehaviour
{
    public static Game Instance;
    public static GameSettings Settings => Instance.settings;

    [SerializeField]
    private GameSettings settings;

    private void Awake()
    {
        Instance = this;
    }
}
#if UNITY_EDITOR
public static class Reload
{
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
#endif