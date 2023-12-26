using UnityEngine;

[CreateAssetMenu(fileName = "Equipment Item", menuName = "Items/Equipment Item")]
public class EquipmentItem : Item
{
    [Header("Equipment Settings")]
    public GameObject _equipmentPrefab;
    public float _weight;

}
