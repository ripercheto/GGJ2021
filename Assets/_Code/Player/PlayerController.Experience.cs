using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntEvent : UnityEvent<int> { }

partial class PlayerController
{
    [Header("XP")]
    public float xpPerLevel = 60;
    public IntEvent onLevelUp = new IntEvent();

    public int XP
    {
        get => xp;
        set
        {
            xp = value;
            var level = Level;
            Level = XpToLevel(xp);
            if (Level > level)
            {

                onLevelUp.Invoke(level);
            }
        }
    }
    private int xp;
    private void HandleExperience()
    {
        Level = 1;
    }
    private int XpToLevel(int xp) => Mathf.FloorToInt(xp / xpPerLevel);
}
