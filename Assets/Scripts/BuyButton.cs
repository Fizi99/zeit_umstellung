using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/*
 * Damit Änderungen am Verhalten vom Save Button möglich waren, musste ich dessen Skript ändern. 
 * Der Buy Button hatte jedoch dasselbe Skript auf sich, weshalb ich das Skript kopiert und als BuyButton.cs benannt habe.
 * (Tino)
 */
public class BuyButton : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Add pure functionality to save button
        GetComponent<Button>().onClick.AddListener(() =>
        {
            //save the shown turrets in a the temporary loadout list
            List<TurretType> loadoutToSave = new List<TurretType>();
            for (int emptySlotIndex = 0; emptySlotIndex < 4; emptySlotIndex++)
                loadoutToSave.Add(this.gameManager.buildManager.emptySlots[emptySlotIndex].GetComponent<EmptySlotHover>().turretType);

            this.gameManager.uiManager.temporaryLoadouts[this.gameManager.uiManager.currentLoadoutIndex - 1] = loadoutToSave;

            //save temporary loadouts permanently
            for (int loadoutIndex = 0; loadoutIndex < 4; loadoutIndex++)
                SaveManager.SaveLoadout(loadoutIndex + 1, this.gameManager.uiManager.temporaryLoadouts[loadoutIndex]);

            //update dropdown visualization
            this.gameManager.player.chosenLoadout = SaveManager.LoadLoadout(this.gameManager.uiManager.dropdown.value + 1);
            this.gameManager.uiManager.UpdateLoadoutVisualization();
        });
    }

    void Update()
    {

    }
}