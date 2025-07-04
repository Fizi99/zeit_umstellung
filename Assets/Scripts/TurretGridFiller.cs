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
        //reset purchased turrets:
        //SaveManager.SavePurchasedTurrets(new List<TurretType>());
        PopulateGridV3();
    }

    void PopulateGridV3()
    {
        Debug.Log("gets populated");
        foreach (var mapping in turretMappings)
        {
            GameObject frame = Instantiate(turretFramePrefab, gridParent);

            RawImage iconImage = frame.transform.Find("TurretImage").GetComponent<RawImage>();
            DragObject.transform.Find("TurretImage").GetComponent<RawImage>().texture = mapping.data.turretIconTexture;
            frame.transform.GetComponent<displayTurretCost>().turret = mapping.prefab;
            frame.transform.GetComponent<displayTurretCost>().ButtonSprite = mapping.buttonSprite;
            DragObject.GetComponent<CarryTurretInfo>().Turret = mapping.prefab;
            frame.transform.GetComponent<displayTurretCost>().DragObject = DragObject;
            frame.transform.GetComponent<displayTurretCost>().texture = mapping.data.turretIconTexture;
            frame.transform.GetComponent<displayTurretCost>().ChangedDragObject = mapping.DragObject;
            frame.transform.GetComponent<displayTurretCost>().turretType = mapping.data.turretType;
            if (!SaveManager.LoadPurchasedTurrets().Contains(mapping.prefab.GetComponent<TurretAI>().name))
            {
                frame.transform.GetComponent<displayTurretCost>().lockImage.SetActive(true);
            }



            iconImage.texture = mapping.data.turretIconTexture;

            Button btn = frame.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                turretInfoUIElem.DisplayFromData(mapping.data);
                // if u have enough currency and dont have the turret, buy it
                if (!SaveManager.LoadPurchasedTurrets().Contains(mapping.data.turretType) &&
                    mapping.prefab.GetComponent<TurretAI>().uhraniumPrice < gameManager.player.savedUhranium)
                {
                    Debug.Log("turret bought (not yet implemented)");
                    //reduce currency
                    gameManager.player.savedUhranium -= mapping.prefab.GetComponent<TurretAI>().uhraniumPrice;
                    //hide lock image
                    frame.transform.GetComponent<displayTurretCost>().lockImage.SetActive(false);
                   //change posession status of turret permanently
                    List<TurretType> purchasedTurrets = SaveManager.LoadPurchasedTurrets();
                    purchasedTurrets.Add(mapping.data.turretType);
                    SaveManager.SavePurchasedTurrets(purchasedTurrets);

                }
            });
            if(6 >  BuildManager.uiLoadoutList.Count) { 
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
