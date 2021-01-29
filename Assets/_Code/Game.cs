using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
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
