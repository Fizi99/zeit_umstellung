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

    private GameManager gameManager;
    private buildManager BuildManager;

    public GameObject DragObject;

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.BuildManager = gameManager.buildManager;
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
            frame.transform.GetComponent<displayTurretCost>().DragObject = DragObject;
            frame.transform.GetComponent<displayTurretCost>().texture = mapping.data.turretIconTexture;




            iconImage.texture = mapping.data.turretIconTexture;

            Button btn = frame.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                turretInfoUIElem.DisplayFromData(mapping.data);
            });

            BuildManager.addToUiLoadoutList(frame);
        }
    }
}

[Serializable]
public class TurretPrefabMapping
{
    public GameObject prefab;
    public TurretData data;
    public Sprite buttonSprite;
}
