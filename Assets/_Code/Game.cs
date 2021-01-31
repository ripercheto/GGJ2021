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
    }

    #region dungeon
    public static Dungeon dungeonInstance;

    public static void StartDungeon()
    {
        //spawn
        //generate random item placements
        //move player and start battle

        dungeonInstance = Instantiate(Settings.dungeonPrefab);

        Player.ResetPlayer(dungeonInstance.playerSpawnPos.position);
    }

    public void CleanUpDungeon()
    {
        if (dungeonInstance != null)
        {
            Destroy(dungeonInstance.gameObject);
        }
    }
    #endregion

}
#if UNITY_EDITOR
public static class Reload
{
    [UnityEditor.MenuItem("GameObject/MyCategory/Snap", false, 10)]
    public static void ContextSnapToFloor()
    {
        var activeObj = UnityEditor.Selection.activeGameObject;
        if (activeObj == null)
        {
            return;
        }

        var objPos = activeObj.transform.position;
        if (!Physics.Raycast(objPos + Vector3.up, Vector3.down, out var hitInfo, Mathf.Infinity))
        {
            return;
        }

        activeObj.transform.position = hitInfo.point;
    }

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