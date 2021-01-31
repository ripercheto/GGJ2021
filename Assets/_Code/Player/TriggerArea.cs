using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    public int checkLayer;

    public List<EnemyBase> InTrigger
    {
        get
        {
            for (int i = 0; i < inTrigger.Count; i++)
            {
                if (inTrigger[i] != null)
                {
                    continue;
                }

                inTrigger.RemoveAt(i);
                i--;
            }
            return inTrigger;
        }
    }

    private List<EnemyBase> inTrigger = new List<EnemyBase>();

    private void OnDisable()
    {
        inTrigger.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != checkLayer)
        {
            return;
        }

        var enemy = other.GetComponentInParent<EnemyBase>();
        if (enemy == null)
        {
            return;
        }

        inTrigger.Add(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != checkLayer)
        {
            return;
        }

        var enemy = other.GetComponentInParent<EnemyBase>();
        if (enemy == null)
        {
            return;
        }
        if (!inTrigger.Contains(enemy))
        {
            return;
        }

        inTrigger.Remove(enemy);
    }
}
