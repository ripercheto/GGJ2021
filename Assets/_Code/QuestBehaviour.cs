using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class QuestBehaviour : MonoBehaviour
{
    //Requested item
    bool FirstEncounter = true;
    Item LostItem;

    [Header("Basic settings")]
    public Image LostItem_UI; //Target to draw lost items on
    public CustomerBehaviour CustomerPrefab;
    List<CustomerBehaviour> ListOfCustomers = new List<CustomerBehaviour>();
    public Transform CustomerSpawnPoint;

    [Header("Satisfaction settings")]
    public Image[] Slots_UI; //Target to draw satisfaction rates
    public Sprite Unknown_Icon;
    public Sprite Unhappy_Icon;
    public Sprite Happy_Icon;
    int CurrentSatisfactionSlot = 0;

    void Start() {
        //LostItem_UI.sprite = LostItem.icon;
        ResetSatisfactionUI();
    }
    void ResetSatisfactionUI() {
        foreach (Image IconSlot in Slots_UI) {
            IconSlot.sprite = Unknown_Icon;
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
        Sprite CurrentSatisfaction = LostItemIsFound ? Happy_Icon : Unhappy_Icon;
        Slots_UI[CurrentSatisfactionSlot].sprite = CurrentSatisfaction; //Set satisfaction sprite on HUD

        //Remove NPC from customers
        if (ListOfCustomers.Count > 0) { //Running function without customer
            ListOfCustomers[0].SendHome(CurrentSatisfaction);
            ListOfCustomers.Remove(ListOfCustomers[0]);
        }
    }

    void InitNextCustomer() {
  
        SetLostItem(GetRandomItem()); //Pick a random lost item

        CustomerBehaviour NewCustomer = Instantiate(CustomerPrefab, CustomerSpawnPoint); //Create new customer
        NewCustomer.Set_SpeechBubbleIcon(LostItem.icon); //Set the NPC speech bubble icon
        ListOfCustomers.Add(NewCustomer); //Keep track of customers
    }

    Item GetRandomItem() {
        Item[] t_AvailibleItems = Game.Settings.AvailableItems;
        int t_RandomItemIndex = Random.Range(0, t_AvailibleItems.Length);

        return t_AvailibleItems[t_RandomItemIndex];
    }

    bool GameOver(int a_SlotNumber) {
        if (a_SlotNumber > Slots_UI.Length) {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other) {

        print("OnTriggerEnter");
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        if (FirstEncounter) {
            InitNextCustomer();
            FirstEncounter = false;
            return;
        }
        
        //Deliver the found object
        CompareFoundItem(GetRandomItem());

        //Select next satisfaction slot...
        CurrentSatisfactionSlot++;
        if (GameOver(CurrentSatisfactionSlot) == true) {
            print("GAME OVER!!!");
        }
        else {
            InitNextCustomer();
        }
    }

    //Temporary reset function
    //void InSlotBounds() {
    //    if (CurrentSatisfactionSlot > 4) {
    //        CurrentSatisfactionSlot = 0;
    //        ResetSatisfactionUI();
    //    }
    //}

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
    /*
    void Update(){
        //Temporary
        if (Input.GetKeyUp("o")) {
            InitNextCustomer();//OnStart hand in round
            print("O pressed");
        }
    
        if (Input.GetKeyUp("i")) {
            //Every other round 
            CompareFoundItem(GetRandomItem()); 
    
            //Select next satisfaction slot...
            CurrentSatisfactionSlot++;
            InSlotBounds(); //restarts from satisfaction slot 0
    
            InitNextCustomer();

            print("O pressed");
        }
    }*/

}
