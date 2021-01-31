using UnityEngine;
using UnityEditor;

public class MyWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/My Window")]
    static void CreateWindow()
    {
        // Get existing open window or if none, make a new one:
        MyWindow window = (MyWindow)EditorWindow.GetWindow(typeof(MyWindow));
        window.Show();
    }

    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        var e = Event.current;

        if (e == null)
        {
            return;
        }

        if (e.type != EventType.KeyDown)
        {
            return;
        }

        if (e.keyCode != KeyCode.Space)
        {
            return;
        }

        var activeObj = Selection.activeGameObject;
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

        /*
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();*/
    }
}