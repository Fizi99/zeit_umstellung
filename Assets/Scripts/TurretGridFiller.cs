using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;


public class TurretGridFiller : MonoBehaviour
{
    [Header("UI References")]
    public GameObject turretFramePrefab;
    public Transform gridParent;

    [Header("Script & Mapping")]
    public TurretInfoUI turretInfoUIElem;
    public List<TurretPrefabMapping> turretMappings;

    private GameManager gameManager;
    private AudioManager audioManager;
    private buildManager BuildManager;

    public GameObject DragObject;
    public Button BuyButton;
    public int ButtonIndex = -1;

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        this.BuildManager = gameManager.buildManager;
        
        //reset purchased turrets: 
        //SaveManager.SavePurchasedTurrets(new List<TurretType>());
        PopulateGridV3();
    }

    void PopulateGridV3()
    {
        int i = 0;
        foreach (var mapping in turretMappings)
        {
            int currentIndex = i;
            GameObject frame = Instantiate(turretFramePrefab, gridParent);

            RawImage iconImage = frame.transform.Find("TurretImage").GetComponent<RawImage>();
            var displayScript = frame.transform.GetComponent<displayTurretCost>();

            iconImage.texture = mapping.data.turretIconTexture;

            // Properly assign and cache original texture
            displayScript.turretImage = iconImage;
            displayScript.AssignOriginalTexture(mapping.data.turretIconTexture);

            displayScript.turret = mapping.prefab;
            displayScript.DragObject = DragObject;

            
            displayScript.ButtonSprite = mapping.buttonSprite;
            displayScript.ChangedDragObject = mapping.DragObject;
            displayScript.turretType = mapping.data.turretType;
            displayScript.texture = mapping.data.turretIconTexture;

            // Check possession using correct TurretType
            bool isPosessed = SaveManager.LoadPurchasedTurrets().Contains(mapping.data.turretType);
            displayScript.showPosession(isPosessed);

            if(mapping.data.turretType == TurretType.DYNAMITE)
            {
                gameManager.bombButton = displayScript.gameObject;
            }else if(mapping.data.turretType == TurretType.FREEZE)
            {
                gameManager.freezeButton = displayScript.gameObject;
            }

            Button selectBtn = frame.GetComponent<Button>();
            selectBtn.onClick.AddListener(() =>
            {
                this.ButtonIndex = currentIndex;
                gameManager.buildManager.loadoutButtonPressed = true;
                turretInfoUIElem.DisplayFromData(mapping.data);

                if (!SaveManager.LoadPurchasedTurrets().Contains(mapping.data.turretType)) 
                {
                    BuyButton.gameObject.SetActive(true);

                }
            });

            BuyButton.onClick.AddListener(() =>
            {
                if (currentIndex == this.ButtonIndex) { 
                if (!SaveManager.LoadPurchasedTurrets().Contains(mapping.data.turretType) &&
                    mapping.prefab.GetComponent<TurretAI>().uhraniumPrice <= SaveManager.LoadUhranium())
                {
                    SaveManager.SaveUhranium(SaveManager.LoadUhranium() - mapping.prefab.GetComponent<TurretAI>().uhraniumPrice);

                    List<TurretType> purchasedTurrets = SaveManager.LoadPurchasedTurrets();
                    purchasedTurrets.Add(mapping.data.turretType);
                    SaveManager.SavePurchasedTurrets(purchasedTurrets);
                    this.gameManager.uiManager.UpdateUhraniumLoadoutDisplay();
                    displayScript.showPosession(true);
                        this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxTurretPurchased);
                }
                }
                BuyButton.gameObject.SetActive(false);
                gameManager.buildManager.loadoutButtonPressed = true;
            });

            if (6 > BuildManager.uiLoadoutList.Count)
            {
                BuildManager.addToUiLoadoutList(frame);
            }
            i++;
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
