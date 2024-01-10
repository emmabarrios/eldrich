using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
public class Item : ScriptableObject {
    [Header("Base Settings")]
    public string _name;
    public string _description;
    public Sprite _sprite;
    public Sprite _menuSprite;
    public GameObject _modelPrefab;
    public float _discoverability;
    public float _price;

    public override string ToString() {
        return _name;
    }
}

