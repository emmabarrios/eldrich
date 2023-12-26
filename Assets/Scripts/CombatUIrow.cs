using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIrow : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();
    public PouchItem pouchItemPrefab;
    public int currentRow = 0;

    //public Sprite nullSprite;

    public void LoadSlotItems(List<QuickItem> itemList, int row) {

        currentRow = row;

        for (int i = 0; i < slots.Count; i++) {
            
            if (itemList[i] != null) {
                PouchItem currentPouchItem = Instantiate(pouchItemPrefab);
                currentPouchItem.itemSO = itemList[i];
                currentPouchItem.SetItemIndex(currentRow, i);

                currentPouchItem.InitializeQuickItemGraphics();
                currentPouchItem.transform.SetParent(slots[i].transform, false);

                GameObject slot_child = slots[i];

                slot_child.transform.GetChild(0).GetComponent<Image>().sprite = currentPouchItem.itemSO._slot_empty_sprite;
            } else {
                GameObject slot_child = slots[i];
                Image imageComponent = slot_child.transform.GetChild(0).GetComponent<Image>();
                Color currentColor = imageComponent.color;
                currentColor.a = 0;
                imageComponent.color = currentColor;
            }
        }
    }
}
