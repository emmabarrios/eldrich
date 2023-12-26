using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickItemSlotsContainer : MonoBehaviour
{
    private bool wasEnabled = false;

    public void ResetSlotsClickedState() {
        foreach (ItemCardUI itemCardUi in this.transform.GetComponentsInChildren<ItemCardUI>()) {
            itemCardUi.ResetSlotCardState();
        }
    }

    private void OnEnable() {
        if (wasEnabled == false) {
            UpdateSlotGroupContent(CombatInventory.instance.GetItemLists());
            UpdateMainWeaponSlotContent(CombatInventory.instance.GetEquipedWeaponIten());
        }
        wasEnabled = true;
    }

    private void UpdateSlotGroupContent(List<List<QuickItem>> itemLists) {

        ItemCardUI[] quickItemUiSlots = this.transform.GetComponentsInChildren<ItemCardUI>();

        for (int row = 0; row < itemLists.Count; row++) {
            for (int col = 0; col < itemLists[row].Count; col++) {

                QuickItem currentItem = itemLists[row][col];

                if (currentItem != null) {
                    if (row == 0 && col < 4) {
                        quickItemUiSlots[col].UpdateItemOnSlot(currentItem);
                    } else if (row == 1 && col < 4) {
                        quickItemUiSlots[col + 4].UpdateItemOnSlot(currentItem);
                    }
                }
            }
        }
    }

    private void UpdateMainWeaponSlotContent(WeaponItem currentWeapon) {
        ItemCardUI[] itemCardUIslots = this.transform.GetComponentsInChildren<ItemCardUI>();
        itemCardUIslots[8].UpdateItemOnSlot(currentWeapon);
        GameObject.Find("UICharacter").GetComponent<UICharacter>().SpawnWeaponOnPivot(currentWeapon);
    }
}
