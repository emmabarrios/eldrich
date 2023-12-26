using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatUIcarousel : MonoBehaviour
{
    public GameObject rowPrefab;
    public PouchIndicator indicatorContainer;
    // This public list is just to visualize in inspector as debug
    public List<CombatUIrow> rows = new List<CombatUIrow>();

    public void InitializeUIcarousel(List<List<QuickItem>> listOfItems) {
        int rowCount = listOfItems.Count;
        GameObject tempRow = null;
        CombatUIrow row = null;

        // If quick item list are not empty
        if (rowCount > 0) {
            for (int i = 0; i < rowCount; i++) {
                tempRow = Instantiate(rowPrefab, this.transform);
                row = tempRow.GetComponent<CombatUIrow>();
                row.LoadSlotItems(listOfItems[i], i);

                // Add row to visualize in inspector
                rows.Add(row);
            }
            indicatorContainer.InitializeRowIndicators(rows.Count);
            GetComponent<Carousel>().SetPouchCountTotal(rows.Count);
        }
    }
}
