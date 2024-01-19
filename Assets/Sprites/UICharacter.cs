using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacter : MonoBehaviour
{

    public Transform pivot;
    public void SpawnWeaponOnPivot(WeaponItem weaponItem) {
        if (pivot.childCount == 0) {
            Instantiate(weaponItem._equipmentPrefab, pivot);
        } else {
            Destroy(pivot.GetChild(0).gameObject);
            Instantiate(weaponItem._equipmentPrefab, pivot);
        }
        SetLayerAllChildren(this.transform, this.transform.root.GetComponent<Transform>().gameObject.layer);
    }

    void SetLayerAllChildren(Transform root, int layer) {
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children) {
            child.gameObject.layer = layer;
        }
    }
}
