using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostItem : MonoBehaviour
{
    public Item Item
    {
        get => Item;
        set
        {
            item = value;
            if (item != null)
            {
                lostItemSprite.sprite = item.icon;
            }
        }
    }
    private Item item;
    public SpriteRenderer lostItemSprite;
    private void Awake()
    {
        var randPos = Random.insideUnitSphere;
        randPos.y = 0;
        transform.rotation = Quaternion.LookRotation(randPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 10)
        {
            return;
        }

        var p = other.gameObject.GetComponentInParent<PlayerController>();
        if (p == null)
        {
            return;
        }

        if (p.carryingItem != null)
        {
            return;
        }

        p.carryingItem = item;

        Destroy(gameObject);
    }
}
