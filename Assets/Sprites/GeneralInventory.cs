using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneralInventory : MonoBehaviour
{
    public static GeneralInventory instance;

    [Header("Stored Items")]
    public List<QuickItem> storedQuickItemList = new List<QuickItem>();
    public List<WeaponItem> storedWeaponItemList = new List<WeaponItem>();

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        SortItemList(storedQuickItemList);
        SortItemList(storedWeaponItemList);
    }

    public void StoreItems(List<QuickItem> items, List<WeaponItem> weapons = null) {
        foreach (QuickItem item in items) {
            storedQuickItemList.Add(item);
        }

        if (weapons!=null) {
            foreach (WeaponItem item in weapons) {
                storedWeaponItemList.Add(item);
            }
        }

        SortItemList(storedQuickItemList);


        //BubbleSortByName(storedQuickItemList);
        // Convert List<QuickItem> to List<Item>
        //List<Item> quickItemList = storedQuickItemList.ConvertAll(item => (Item)item);
        //BubbleSortByName(quickItemList);
        //storedQuickItemList = quickItemList.ConvertAll(item => (QuickItem)item);
    }

    public List<QuickItem> GetQuickItems() {
        return storedQuickItemList;
    }
    public List<WeaponItem> GetWeaponItems() {
        return storedWeaponItemList;
    }


    public List<string> GetQuickItemsAsStrings() {
        List<string> quickItemStrings = storedQuickItemList.Select(q => q.ToString()).ToList();
        return quickItemStrings;
    }

    public List<string> GetWeaponItemsAsStrings() {
        List<string> weaponItemStrings = storedWeaponItemList.Select(w => w.ToString()).ToList();
        return weaponItemStrings;
    }


    public void RemoveItem(Item item) {
        if (item is QuickItem) {
            if (storedQuickItemList.Contains((QuickItem)item)) {
                storedQuickItemList.Remove((QuickItem)item);

                SortItemList(storedQuickItemList);
            }
        } else {
            if (storedWeaponItemList.Contains((WeaponItem)item)) {
                storedWeaponItemList.Remove((WeaponItem)item);

                SortItemList(storedWeaponItemList);
            }
        }
    }

    public void RemoveItems(List<List<QuickItem>> dualListOfItems) {
        foreach (List<QuickItem> listOfItems in dualListOfItems) {
            foreach (QuickItem quickItem in listOfItems) {
                if (storedQuickItemList.Contains(quickItem)) {
                    storedQuickItemList.Remove(quickItem);
                }
            }
        }
    }

    public void AddItems(List<List<QuickItem>> dualListOfItems) {
        foreach (List<QuickItem> listOfItems in dualListOfItems) {
            foreach (QuickItem quickItem in listOfItems) {
                if (quickItem != null) {
                    storedQuickItemList.Add(quickItem);
                }
            }
        }

        SortItemList(storedQuickItemList);

        //BubbleSortByName(storedQuickItemList);
        // Convert List<QuickItem> to List<Item>
        //List<Item> quickItemList = storedQuickItemList.ConvertAll(item => (Item)item);
        //BubbleSortByName(quickItemList);
        //storedQuickItemList = quickItemList.ConvertAll(item => (QuickItem)item);
    }

    public void AddItem(Item item) {

        if (item is QuickItem) {
            storedQuickItemList.Add((QuickItem)item);
            SortItemList(storedQuickItemList);
        } else {
            storedWeaponItemList.Add((WeaponItem)item);
            SortItemList(storedWeaponItemList);
        }

        //storedQuickItemList.Add(quickItem);
    }

    private void BubbleSortByName(List<Item> items) {
        int n = items.Count;
        bool swapped;

        do {
            swapped = false;

            for (int i = 1; i < n; i++) {
                if (string.Compare(items[i - 1]._name, items[i]._name) > 0) {
                    // Swap items
                    Item temp = items[i - 1];
                    items[i - 1] = items[i];
                    items[i] = temp;

                    swapped = true;
                }
            }

            // After each pass, the largest element will be at the end,
            // so we can reduce the range of elements to consider
            n--;
        } while (swapped);
    }

    private void SortItemList<T>(List<T> itemList) where T : Item {
        int n = itemList.Count;
        bool swapped;

        do {
            swapped = false;

            for (int i = 1; i < n; i++) {
                if (string.Compare(itemList[i - 1]._name, itemList[i]._name) > 0) {
                    // Swap items
                    T temp = itemList[i - 1];
                    itemList[i - 1] = itemList[i];
                    itemList[i] = temp;

                    swapped = true;
                }
            }

            // After each pass, the largest element will be at the end,
            // so we can reduce the range of elements to consider
            n--;
        } while (swapped);
    }

    public void SortItems() {
        SortItemList(storedQuickItemList);
        SortItemList(storedWeaponItemList);
    }

    public void GenerateItemsFromDatabase() { 
    }
}
