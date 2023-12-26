using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Weapon Item", menuName = "Items/Weapon Item")]
public class WeaponItem: EquipmentItem {

    [Header("Weapon Stats")]
    public int _strenghtRequirement;
    public float _staminaCost;
    public float _durability;

    [Space]
    public float _damage;

    [Space]
    public float _attackCooldown;

}

