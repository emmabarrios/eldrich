using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatInventory : MonoBehaviour
{
    // Make it "singleton"
    public static CombatInventory instance;

    [Header("Quick Items")]
    public List<QuickItem> itemList1 = new List<QuickItem>();
    public List<QuickItem> itemList2 = new List<QuickItem>();
    public List<List<QuickItem>> itemLists = new List<List<QuickItem>>();

    [Header("Main Weapoms")]
    public WeaponItem WeaponItemSO;
    public WeaponSlotManager weaponSlotManager = null;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start() {
        itemLists.Add(itemList1);
        itemLists.Add(itemList2);
    }

    public void LoadPlayerWeapon() {
        weaponSlotManager = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponSlotManager>();
        if (weaponSlotManager != null) {
            weaponSlotManager.LoadWeaponOnSlot(WeaponItemSO);
        } 
    }

    public void RemoveItem(int row, int col){
        itemLists[row][col] = null;
    }   
    
    public void AddItem(int row, int col, Item item){

        if (item is QuickItem) {
            if (itemLists[row][col] == (QuickItem)item) {
                RemoveItem(row, col);
            } else {
                itemLists[row][col] = (QuickItem)item;
            }
        } else if(item is WeaponItem) {
            WeaponItemSO = (WeaponItem)item;
        }
    }

    public List<List<QuickItem>> GetItemLists() {
        return itemLists;
    }

    public WeaponItem GetEquipedWeaponIten() {
        return WeaponItemSO;
    }

    public List<string> GetQuickItemsAsStrings() {
        List<string> quickItemStrings = new List<string>();

        foreach (List<QuickItem> itemList in itemLists) {
            if (itemList != null) {
                // Use null-conditional operator to handle null items
                List<string> itemStrings = itemList.Select(item => item?.ToString()).Where(s => !string.IsNullOrEmpty(s)).ToList();
                quickItemStrings.AddRange(itemStrings);
            }
        }

        if (quickItemStrings.Count > 0) {
            return quickItemStrings;
        }

        return null;
    }

    public string GetEquipedWeaponAsString() {
        return WeaponItemSO.ToString();
    }
}
