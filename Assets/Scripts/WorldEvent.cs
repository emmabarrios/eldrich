using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldEvent")]
public class WorldEvent : ScriptableObject {

    public GameObject _enemy;
    public GameObject _pinObject;
    public GameObject _lootPickupFX;
    public int _exp;
    public EventType _eventType;
    public List<Item> _loot;
    
    
}
