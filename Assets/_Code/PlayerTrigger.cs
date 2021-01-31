using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    public UnityEvent onEnter = new UnityEvent();
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

        onEnter.Invoke();
    }
}
