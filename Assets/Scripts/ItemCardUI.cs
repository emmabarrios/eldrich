using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCardUI : MonoBehaviour
{
    [SerializeField] private int rowNumber = 0;
    [SerializeField] private int colNumber = 0;

    public int RowNumber { get { return rowNumber; } }
    public int ColNumber { get { return colNumber; } }

    private UnityEngine.UI.Button button;
    private UnityEngine.UI.Button removeButton;
    private Item item;
    private GameObject iconGameObject;
    private Sprite emptySlotSprite;



    

    private void Start() {
        button.onClick.AddListener(() => SetCardStateToClicked());
        removeButton.onClick.AddListener(() => SendItemToGeneralInventory());
        iconGameObject = this.GetComponent<RectTransform>().GetChild(1).gameObject;
    }

    [Header("State settings")]
    public CardState cardCurrentState = CardState.NotClicked;

    public void ResetSlotCardState() {
        this.cardCurrentState = CardState.NotClicked;
    }
    
    public void SetCardStateToClicked() {
        this.cardCurrentState = CardState.Clicked;
    }

    public bool IsCardClicked() {
        return this.cardCurrentState == CardState.Clicked;
    }

    public void UpdateItemOnSlot(Item item) {
        this.item = item;
        Image iconImage = iconGameObject.GetComponent<Image>();
        iconImage.sprite = item._menuSprite;
        iconGameObject.SetActive(true);
    }

    public bool HasItem() {
        return item != null;
    }

    public bool HasQuickItem() {
        return item is QuickItem;
    }   

    public bool HasWeaponItem() {
        return item is WeaponItem;
    }

    public Item GetItem() {
        return item;
    }

    public void ResetCard() {
        ResetSlotCardState();
        Image iconImage = iconGameObject.GetComponent<Image>();
        //iconImage.sprite = emptySlotSprite;
        iconImage.sprite = null;
        item = null;
        iconGameObject.SetActive(false);
        
    }

    public void SendItemToGeneralInventory() {
        if (item!=null) {
            GeneralInventory.instance.AddItem(GetItem());
            CombatInventory.instance.RemoveItem(rowNumber,colNumber);
            ResetCard();
        }
    }
}
