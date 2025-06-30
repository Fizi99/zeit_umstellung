using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class TurretGridFiller : MonoBehaviour
{
    [Header("UI References")]
    public GameObject turretFramePrefab;
    public Transform gridParent;

    [Header("Script & Mapping")]
    public TurretInfoUI turretInfoUIElem;
    public List<TurretPrefabMapping> turretMappings;

    void Start()
    {
        PopulateGridV3();
    }

    void PopulateGridV3()
    {
        foreach (var mapping in turretMappings)
        {
            GameObject frame = Instantiate(turretFramePrefab, gridParent);

            RawImage iconImage = frame.transform.Find("TurretImage").GetComponent<RawImage>();
            iconImage.texture = mapping.data.turretIconTexture;

            Button btn = frame.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                turretInfoUIElem.DisplayFromData(mapping.data);
            });
        }
    }
}

[Serializable]
public class TurretPrefabMapping
{
    public GameObject prefab;
    public TurretData data;
}
