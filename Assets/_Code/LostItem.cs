using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostItem : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 10)
        {
            return;
        }

        var p = other.gameObject.GetComponent<PlayerController>();
        if (p == null)
        {
            return;
        }

        if (p.carryingItem != null)
        {
            return;
        }

        p.carryingItem = item;
    }
}
