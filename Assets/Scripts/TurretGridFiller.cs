using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;

public class TurretGridFiller : MonoBehaviour
{
    [Header("UI References")]
    public GameObject turretFramePrefab;
    public Transform gridParent;

    [Header("Icon Mapping")]
    public Sprite defaultTurretImage;
    public List<TurretIconMapping> imageMappings;

    void Start()
    {
        PopulateGrid();
    }

    void PopulateGrid()
    {
        var purchasedTurrets = SaveManager.LoadPurchasedTurrets();
        foreach (TurretType turret in Enum.GetValues(typeof(TurretType)))
        {
            GameObject turretFrame = Instantiate(turretFramePrefab, gridParent);
            Image turretImage = turretFrame.transform.Find("TurretImage").GetComponent<Image>();
            turretImage.sprite = GetSpriteForTurret(turret);

            // Schloss-Overlay aktivieren/deaktivieren (SPÄTER)
            /*Transform lockOverlay = turretFrame.transform.Find("LockIcon");
            bool isUnlocked = purchasedTurrets.Contains(turret);
            if (lockOverlay != null)
                lockOverlay.gameObject.SetActive(!isUnlocked);*/
        }
    }

    Sprite GetSpriteForTurret(TurretType type)
    {
        var entry = imageMappings.FirstOrDefault(m => m.turretType == type);
        return entry != null ? entry.icon : defaultTurretImage;
    }
}

[Serializable]
public class TurretIconMapping
{
    public TurretType turretType;
    public Sprite icon;
}
