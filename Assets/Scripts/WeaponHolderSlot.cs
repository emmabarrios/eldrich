using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    public GameObject currentWeaponModel;

    public void UnloadWeaponModel() {
        if (currentWeaponModel != null) {
            currentWeaponModel.SetActive(false);
        }
    }

    public void UnloadWeaponAndDestroy() {
        if (currentWeaponModel != null) {
            Destroy(currentWeaponModel);
        }
    }

    public void LoadWeaponModel(WeaponItem weaponItem) {

        UnloadWeaponAndDestroy();

        if (weaponItem == null) {
            UnloadWeaponModel();
            return;
        }

        GameObject model = Instantiate(weaponItem._equipmentPrefab);

        if (model != null) {
            model.transform.parent = this.transform;
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
        }
        currentWeaponModel = model;
        SetLayerAllChildren(model.transform, model.transform.root.GetComponent<Transform>().gameObject.layer);
    }

    void SetLayerAllChildren(Transform root, int layer) {
        var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children) {
            child.gameObject.layer = layer;
        }
    }

}
