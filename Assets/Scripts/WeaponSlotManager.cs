using UnityEngine;

public class WeaponSlotManager : MonoBehaviour
{
    [SerializeField] private WeaponHolderSlot weaponHolderSlot;

    Weapon weapon;

    private void Awake() {
        WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
        foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots) {
            weaponHolderSlot = weaponSlot;
        }
    }

    public void LoadWeaponOnSlot(WeaponItem weaponItem) {
        weaponHolderSlot.LoadWeaponModel(weaponItem);
    }
}
