using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickItemEquipmentCard : MonoBehaviour
{
    public Item item;
    private UnityEngine.UI.Button button;

    [SerializeField] private int total = 1;
    [SerializeField] private UnityEngine.UI.Image icon;
    [SerializeField] private UnityEngine.UI.Image selectedIcon;
    [SerializeField] private UnityEngine.UI.Text title;
    [SerializeField] private UnityEngine.UI.Text description;
    [SerializeField] private UnityEngine.UI.Text counter;

    private void Start() {
        button = this.GetComponent<UnityEngine.UI.Button>();
        //button.onClick.AddListener(() => GameObject.Find("Quick Item Slots Container").GetComponent<QuickItemSlotsContainer>().ResetSlotsClickedState());
        button.onClick.AddListener(() => EquipQuickItem());
    }

    public void InitializeQuickItemEquipmentCard(Item item) {
        this.item = item;
        icon.sprite = item._menuSprite;
        title.text = item._name;
        description.text = item._description;
    }

    public void UpdateQuickItemEquipmentCardCounter(int val) {
        total += val;
        counter.text = total.ToString();
    }

    public string GetItemSOname() {
        return item._name;
    }

    public void ToggleSelectedIcon() {
        selectedIcon.gameObject.SetActive(!selectedIcon.gameObject.activeSelf);
    }

    public void SetSelectedIconState(bool state) {
        selectedIcon.gameObject.SetActive(state);
    }

    public void EquipQuickItem() {
        foreach (ItemCardUI itemCardUi in GameObject.Find("Quick Item Slots Container").GetComponentsInChildren<ItemCardUI>()) {
            if (itemCardUi.IsCardClicked()) {

                bool skipLoadOut = false;

                if (!itemCardUi.HasItem()) {
                    itemCardUi.UpdateItemOnSlot(item);
                    itemCardUi.ResetSlotCardState();

                    // Remove equipment card item from general inventory
                    GeneralInventory.instance.RemoveItem(item);
                } else {
                    if (item == itemCardUi.GetItem()) {
                        itemCardUi.ResetSlotCardState();
                        skipLoadOut = true;
                    } else {

                        // Add quick item slot current item to general inventory
                        GeneralInventory.instance.AddItem(itemCardUi.GetItem());

                        // Remove equipment card item from general inventory
                        GeneralInventory.instance.RemoveItem(item);

                        // Add equipment card item to quick item slot
                        itemCardUi.UpdateItemOnSlot(item);

                        itemCardUi.ResetSlotCardState();
                    }
                }

                // Spawn selected weapon on UI character
                if (item is WeaponItem weaponItem) {
                    GameObject.Find("UICharacter").GetComponent<UICharacter>().SpawnWeaponOnPivot(weaponItem);

                    // This part will work only on overworld scene
                    GameObject playerPin = GameObject.Find("character_pin");

                    if (playerPin!=null) {
                        playerPin.GetComponent<UICharacter>().SpawnWeaponOnPivot(weaponItem);
                    }
                }

                // Equip selected quick item on corresponding combat inventory slot 
                if (!skipLoadOut) {
                    CombatInventory.instance.AddItem(itemCardUi.RowNumber, itemCardUi.ColNumber, item);
                }


                if (GameObject.Find("Quick Items Panel") != null) {
                    GameObject.Find("Quick Items Panel").SetActive(false);
                } 
                
                if (GameObject.Find("Main Weapons Panel") != null) {
                    GameObject.Find("Main Weapons Panel").SetActive(false);
                }
            }
        }
    }
}
