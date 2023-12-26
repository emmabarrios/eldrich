using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickItemsCardGroup : MonoBehaviour
{
    [SerializeField] private QuickItemEquipmentCard cardTemplate;
    private string currentItemSOname = "";
    //private bool wasEnabled = false;

    public enum CardContainerType {
        QuickItem,
        WeaponItem
    }

    [Header("Card Container Type")]
    public CardContainerType cardContainerType = CardContainerType.QuickItem;

    public void UpdateCardGroupContent(GeneralInventory inventory) {

        List<QuickItem> inventoryQuickItems = inventory.GetQuickItems();
        List<WeaponItem> inventoryWeaponItems = inventory.GetWeaponItems();

        QuickItem lastQuickItem = null;
        WeaponItem lastWeaponItem = null;

        int itemCounter = 0;

        // Instantiate a card per quick item type
        if (cardContainerType == CardContainerType.QuickItem) {
            foreach (QuickItem item in inventoryQuickItems) {

                if (lastQuickItem != item) {
                    QuickItemEquipmentCard currentCard = Instantiate(cardTemplate, this.transform);
                    currentCard.InitializeQuickItemEquipmentCard(item);
                    lastQuickItem = item;
                }
            }
        }

        if (cardContainerType == CardContainerType.WeaponItem) {
            foreach (WeaponItem wItem in inventoryWeaponItems) {

                if (lastWeaponItem != wItem) {
                    QuickItemEquipmentCard currentCard = Instantiate(cardTemplate, this.transform);
                    currentCard.InitializeQuickItemEquipmentCard(wItem);
                    lastWeaponItem = wItem;
                }
            }
        }

        // Udate cards counter UI
        foreach (QuickItemEquipmentCard card in this.transform.GetComponentsInChildren<QuickItemEquipmentCard>()) {
            foreach (QuickItem item in inventoryQuickItems) {
                if (item._name == card.GetItemSOname()) {
                    itemCounter++;
                }
            }

            foreach (WeaponItem item in inventoryWeaponItems) {
                if (item._name == card.GetItemSOname()) {
                    itemCounter++;
                }
            }

            card.UpdateQuickItemEquipmentCardCounter(itemCounter);
            itemCounter = 0;
        }
    }

    private void OnDisable() {
        Debug.Log("disabled");
        for (int i = 0; i < this.transform.childCount; i++) {
            Destroy(this.transform.GetChild(i).gameObject);
        }
    }

    private void OnEnable() {
        //StartCoroutine(HighLightEquipedQuickItemCoroutine());

        UpdateCardGroupContent(GeneralInventory.instance);

        // Update content just once
        //if (wasEnabled == false) {
        //    UpdateCardGroupContent(GeneralInventory.instance);
        //}
        //wasEnabled = true;
    }


    private IEnumerator HighLightEquipedQuickItemCoroutine() {

        bool wasElementFound = false;
        Item tempItem = null;

        while (!wasElementFound) {
            foreach (ItemCardUI itemCardUi in GameObject.Find("Quick Item Slots Container").GetComponentsInChildren<ItemCardUI>()) {
                if (itemCardUi.IsCardClicked()) {
                    wasElementFound = true;
                    tempItem = itemCardUi.item;
                }
            }
            yield return null;
        }

        foreach (QuickItemEquipmentCard equipmentCard in this.transform.GetComponentsInChildren<QuickItemEquipmentCard>()) {
            if (equipmentCard.item == tempItem) {
                equipmentCard.SetSelectedIconState(true);
                wasElementFound = true;
            } else {
                equipmentCard.SetSelectedIconState(false);
            }
        }

    }

    
}
