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
    public Texture defaultTurretTexture;
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

            // We have dronebase and drone as own turret types, so skip the drone
            if (turret == TurretType.DRONE)
                continue;

            GameObject turretFrame = Instantiate(turretFramePrefab, gridParent);

            // Hol das RawImage-UI-Element
            RawImage turretImage = turretFrame.transform.Find("TurretImage").GetComponent<RawImage>();
            turretImage.texture = GetTextureForTurret(turret);

            // Schloss-Overlay aktivieren/deaktivieren (SPÄTER)
            /*
            Transform lockOverlay = turretFrame.transform.Find("LockIcon");
            bool isUnlocked = purchasedTurrets.Contains(turret);
            if (lockOverlay != null)
                lockOverlay.gameObject.SetActive(!isUnlocked);
            */
        }
    }

    Texture GetTextureForTurret(TurretType type)
    {
        var entry = imageMappings.FirstOrDefault(m => m.turretType == type);
        return entry != null ? entry.icon : defaultTurretTexture;
    }
}

[Serializable]
public class TurretIconMapping
{
    public TurretType turretType;
    public Texture icon; // RawImage braucht Texture
}