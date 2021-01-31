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
    int CurrentSatisfactionSlot = 0;
    
    public Item[] AvailableItems;

    public CustomerBehaviour CustomerPrefab;
    List<CustomerBehaviour> ListOfCustomers = new List<CustomerBehaviour>();

    void Start() {
        LostItem_UI.sprite = LostItem.icon;
        ResetSatisfactionUI();
    }

    void ResetSatisfactionUI() {
        foreach (Image IconSlot in SatisfactionSlot_UI) {
            IconSlot.sprite = Satisfaction_Unknown;
        }
        
    }

    void SetLostItem(Item a_Item) {
        //print("Setting lost item: " + a_Item.name);

        LostItem = a_Item;
        LostItem_UI.sprite = a_Item.icon;
    }
    void CompareFoundItem(Item a_FoundItem) {
        //print("Found item: "+ a_FoundItem.name);
        
        bool LostItemIsFound = LostItem == a_FoundItem;

        //Set the satisfaction image depending on found or lost...  
        Sprite CurrentSatisfaction = LostItemIsFound ? Satisfaction_Happy : Satisfaction_Unhappy;
        SatisfactionSlot_UI[CurrentSatisfactionSlot].sprite = CurrentSatisfaction; //Set satisfaction sprite on HUD

        //Remove NPC from customers
        if (ListOfCustomers.Count > 0) { //Running function without customer
            ListOfCustomers[0].SendHome(CurrentSatisfaction);
            ListOfCustomers.Remove(ListOfCustomers[0]);
        }
    }

    void InitNextCustomer() {
  
        SetLostItem(GetRandomItem()); //Pick a random lost item

        CustomerBehaviour NewCustomer = Instantiate(CustomerPrefab); //Create new customer
        NewCustomer.Set_SpeechBubbleIcon(LostItem.icon); //Set the NPC speech bubble icon
        ListOfCustomers.Add(NewCustomer); //Keep track of customers
    }

    //Temporary reset function
    void InSlotBounds() {
        if (CurrentSatisfactionSlot > 4) {
            CurrentSatisfactionSlot = 0;
            ResetSatisfactionUI();
        }
    }

    Item GetRandomItem() {
        return AvailableItems[Random.Range(0, AvailableItems.Length)];
    }

    //Temporary customer spawner
    //int someTime = 100;
    //int someDynamicTime = 0;
    //void customerSpawner() {
    //    if (someDynamicTime < 0) {
    //        someDynamicTime = someTime;
    //        print("TimeDone");
    //        GameObject newCustomer = Instantiate<GameObject>(CustomerPrefab);
    //    }
    //    else {
    //        someDynamicTime--;
    //    }
    //}

    void Update(){
        //Temporary
        if (Input.GetKeyUp("o")) {
            InitNextCustomer();
        }

        if (Input.GetKeyUp("i")) {
            CompareFoundItem(GetRandomItem()); 

            //Select next satisfaction slot...
            CurrentSatisfactionSlot++;
            InSlotBounds(); //restarts from satisfaction slot 0

            InitNextCustomer();
        }
    }

}
