using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Item", menuName = "Items/Quick Item")]
public class QuickItem : Item
{
    public enum QuickItemType { Usable, Thowable };

    [Header("Quick Item Settings")]
    public QuickItemType type;
    public Sprite _slot_sprite;
    public Sprite _slot_empty_sprite;
    public GameObject _usablePrefab;
    public GameObject _projectilePrefab;
    public AudioClip _drawSound;
    public float _cooldown;

}
