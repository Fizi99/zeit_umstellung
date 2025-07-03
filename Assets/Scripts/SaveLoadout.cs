using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SaveLoadout : MonoBehaviour
{

    private GameManager gameManager;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            this.gameManager.player.chosenLoadout = SaveManager.LoadLoadout(this.gameManager.uiManager.dropdown.value+1); 
            this.gameManager.uiManager.UpdateLoadoutVisualization();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
