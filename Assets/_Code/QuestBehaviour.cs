using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class QuestBehaviour : MonoBehaviour
{
    //Requested item
    public Item LostItem;
    public Image LostItem_UI;


    //Satisfaction
    public Image[] SatisfactionSlot_UI;
    public Sprite Satisfaction_Unknown;
    public Sprite Satisfaction_Unhappy;
    public Sprite Satisfaction_Happy;
    int CurrentCustomer = 0;
    
    public Item[] AvailableItems;

    void Start() {
        LostItem_UI.sprite = LostItem.icon;
        ResetSatisfactionUI();
    }

    void ResetSatisfactionUI() {
        foreach (Image IconSlot in SatisfactionSlot_UI) {
            IconSlot.sprite = Satisfaction_Unknown;
        }
        
    }

    void SetRandomLostItem() {
        LostItem = AvailableItems[Random.Range(0, AvailableItems.Length)];
        LostItem_UI.sprite = LostItem.icon;
        print("Setting lost item: " + LostItem.name);
    }
    void HandinFoundItem(Item FoundItem) {
        print("Found item: "+FoundItem.name);

        //Hand in the current customers requested item
        Sprite CurrentSatisfaction = (LostItem == FoundItem) ? Satisfaction_Happy : Satisfaction_Unhappy;
        SatisfactionSlot_UI[CurrentCustomer].sprite = CurrentSatisfaction;

        //Setup next customer and his/her requested item
        CurrentCustomer++;
        SetRandomLostItem();

    }

    

    //Temporary reset function
    void CheckAndReset() {
        if (CurrentCustomer > 4) {
            CurrentCustomer = 0;
            ResetSatisfactionUI();
            SetRandomLostItem();
        }
    }

    //int someTime = 100;
    //int someDynamicTime = 0;

    void Update(){
        //Temporary
        if (Input.GetKeyUp("o")) {
            CheckAndReset();
            HandinFoundItem(AvailableItems[Random.Range(0, AvailableItems.Length)]);   
        }
        /*
        if(someDynamicTime < 0) {
            someDynamicTime = someTime;
            print("TimeDone");
        }
        else {
            someDynamicTime--;
        }*/
        
        
    }

}
