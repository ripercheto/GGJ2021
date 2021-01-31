using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    public GameObject SpeechBubble;
    public SpriteRenderer LostItemIcon_Renderer;
    private Animator NpcAnimator;
    private void Start() {
        NpcAnimator = GetComponent<Animator>();
    }

    public void Set_SpeechBubbleIcon(Sprite a_ItemIcon) {
        LostItemIcon_Renderer.sprite = a_ItemIcon;
    }

    public void SendHome() {
        SpeechBubble.SetActive(false);
        NpcAnimator.enabled = true;
    }

    //Animator events...
    void event_StartInteraction() {
        SpeechBubble.SetActive(true);
        NpcAnimator.enabled = false;
    }

    void event_Exit() {
        Destroy(this);
    }
}
