using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerBehaviour : MonoBehaviour
{
    public GameObject SpeechBubble;
    public SpriteRenderer LostItemIcon_Renderer;
    public GameObject ThinkingBubble;
    public SpriteRenderer SatisfactionIcon_Renderer;

    private Animator NpcAnimator;
    private bool requestStarted = false;

    private void Start() {
        NpcAnimator = GetComponent<Animator>();
        SpeechBubble.SetActive(false);
        ThinkingBubble.SetActive(false);
    }

    public void Set_SpeechBubbleIcon(Sprite a_ItemIcon) {
        LostItemIcon_Renderer.sprite = a_ItemIcon;
    }

    public void Set_ThinkingBubbleIcon(Sprite a_ItemIcon) {
        SatisfactionIcon_Renderer.sprite = a_ItemIcon;
    }

    public void SendHome(Sprite a_SatisfactionIcon) {
        SpeechBubble.SetActive(false);
        ThinkingBubble.SetActive(true);
        Set_ThinkingBubbleIcon(a_SatisfactionIcon);
        NpcAnimator.enabled = true;
    }

    //Animator events...
    void event_StartInteraction() {
        SpeechBubble.SetActive(true);
        NpcAnimator.enabled = false;
        //requestStarted = true;
    }

    void event_Exit() {
        Destroy(this.gameObject);
    }
}
