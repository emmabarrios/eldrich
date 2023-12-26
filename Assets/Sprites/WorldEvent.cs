using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldEvent")]
public class WorldEvent : ScriptableObject {
    public GameObject _enemy;
    public GameObject _markerObject;
    public List<QuickItem> _itemList = new List<QuickItem>();
    public List<WeaponItem> _weaponItemList = new List<WeaponItem>();
    public int _exp;
}
