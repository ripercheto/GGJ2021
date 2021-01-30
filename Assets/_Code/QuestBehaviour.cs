using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestBehaviour : MonoBehaviour
{
    //Requested item
    public Image RequestedItem_UI;
    public Item RequestedItem;

    //Satisfaction
    public Image[] SatisfactionSlot_UI;
    public Sprite Satisfaction_Unknown;
    public Sprite Satisfaction_Unhappy;
    public Sprite Satisfaction_Happy;
    int CurrentCustomer = 0;
    enum SatisfactionType {  Unknown, Happy, Sad}

    void Start() {
        RequestedItem_UI.sprite = RequestedItem.icon;
        foreach(Image IconSlot in SatisfactionSlot_UI) {
            IconSlot.sprite = Satisfaction_Unknown;
        }
    }

    void UpdateUI() {
        Sprite CurrentSatisfaction = Satisfaction_Unknown;

        if (true) { //If item == requested item
            CurrentSatisfaction = Satisfaction_Happy;
        }
        else {
            CurrentSatisfaction = Satisfaction_Unhappy;
        }
        
        SatisfactionSlot_UI[CurrentCustomer].sprite = CurrentSatisfaction;

        //Setup next customer and his/her requested item
        CurrentCustomer++;
        RequestedItem_UI.sprite = RequestedItem.icon;


    }
     
    

}
