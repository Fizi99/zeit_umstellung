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

    [Header("Script & Mapping")]
    public TurretInfoUI turretInfoUIElem;
    public List<TurretPrefabMapping> turretMappings;

    private GameManager gameManager;
    private buildManager BuildManager;

    public GameObject DragObject;

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.BuildManager = gameManager.buildManager;
        //reset purchased turrets: UHRANIUM PERMANENT SPEICHERN
        SaveManager.SavePurchasedTurrets(new List<TurretType>());
        PopulateGridV3();
    }

    void PopulateGridV3()
    {
        foreach (var mapping in turretMappings)
        {
            GameObject frame = Instantiate(turretFramePrefab, gridParent);

            RawImage iconImage = frame.transform.Find("TurretImage").GetComponent<RawImage>();
            var displayScript = frame.transform.GetComponent<displayTurretCost>();

            iconImage.texture = mapping.data.turretIconTexture;

            // Properly assign and cache original texture
            displayScript.turretImage = iconImage;
            displayScript.AssignOriginalTexture(mapping.data.turretIconTexture);

            displayScript.turret = mapping.prefab;

            displayScript.ButtonSprite = mapping.buttonSprite;
            displayScript.DragObject = DragObject;
            displayScript.ChangedDragObject = mapping.DragObject;
            displayScript.turretType = mapping.data.turretType;
            displayScript.texture = mapping.data.turretIconTexture;

            // Check possession using correct TurretType
            bool isPosessed = SaveManager.LoadPurchasedTurrets().Contains(mapping.data.turretType);
            displayScript.showPosession(isPosessed);

            Button btn = frame.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                turretInfoUIElem.DisplayFromData(mapping.data);

                if (!SaveManager.LoadPurchasedTurrets().Contains(mapping.data.turretType) &&
                    mapping.prefab.GetComponent<TurretAI>().uhraniumPrice < gameManager.player.savedUhranium)
                {
                    gameManager.player.savedUhranium -= mapping.prefab.GetComponent<TurretAI>().uhraniumPrice;

                    List<TurretType> purchasedTurrets = SaveManager.LoadPurchasedTurrets();
                    purchasedTurrets.Add(mapping.data.turretType);
                    SaveManager.SavePurchasedTurrets(purchasedTurrets);
                    this.gameManager.uiManager.UpdateUhraniumLoadoutDisplay();
                    displayScript.showPosession(true);
                }
            });

            if (6 > BuildManager.uiLoadoutList.Count)
            {
                BuildManager.addToUiLoadoutList(frame);
            }
        }
    }
}

    [Serializable]
public class TurretPrefabMapping
{
    public GameObject prefab;
    public GameObject DragObject;
    public TurretData data;
    public Sprite buttonSprite;
}
