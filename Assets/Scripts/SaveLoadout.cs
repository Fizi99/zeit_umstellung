using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SaveLoadout : MonoBehaviour
{
    private GameManager gameManager;

    private List<List<TurretType>> lastSavedLoadouts = new List<List<TurretType>>(); //*

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        UpdateLastSavedLoadouts();

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
            {
                SaveManager.SaveLoadout(loadoutIndex + 1, this.gameManager.uiManager.temporaryLoadouts[loadoutIndex]);
                //lastSavedLoadouts[loadoutIndex] = new List<TurretType>(this.gameManager.uiManager.temporaryLoadouts[loadoutIndex]); //*
                // Aus UI-Slots die sichtbaren Loadouts direkt lesen
                if (loadoutIndex == gameManager.uiManager.currentLoadoutIndex - 1)
                {
                    List<TurretType> fromSlots = new List<TurretType>();
                    for (int j = 0; j < 4; j++)
                        fromSlots.Add(gameManager.buildManager.emptySlots[j].GetComponent<EmptySlotHover>().turretType);

                    lastSavedLoadouts[loadoutIndex] = new List<TurretType>(fromSlots);
                }
                else
                {
                    // Andere Loadouts aus temporaryLoadouts übernehmen
                    lastSavedLoadouts[loadoutIndex] = new List<TurretType>(gameManager.uiManager.temporaryLoadouts[loadoutIndex]);
                }
            }

            //update dropdown visualization
            this.gameManager.player.chosenLoadout = SaveManager.LoadLoadout(this.gameManager.uiManager.dropdown.value+1); 
            this.gameManager.uiManager.UpdateLoadoutVisualization();
            UpdateSaveButtonUI(false); //*
        });
    }

    void UpdateLastSavedLoadouts()
    {
        lastSavedLoadouts.Clear();
        for (int i = 0; i < 4; i++)
            lastSavedLoadouts.Add(new List<TurretType>(SaveManager.LoadLoadout(i + 1)));
    }

    bool IsSameLoadout(List<TurretType> a, List<TurretType> b)
    {
        if (a == null || b == null || a.Count != b.Count) return false;
        for (int i = 0; i < a.Count; i++)
            if (a[i] != b[i]) return false;
        return true;
    }

    void UpdateSaveButtonUI(bool changed)
    {
        GetComponent<Button>().interactable = changed;
        GetComponentInChildren<TextMeshProUGUI>().text = changed ? "Speichern" : "Gespeichert";
    }

    void Update()
    {
        UpdateSaveButtonUI(HasUnsavedChanges());
    }


    List<TurretType> GetLiveLoadoutFromSlots()
    {
        var result = new List<TurretType>();
        for (int i = 0; i < 4; i++)
            result.Add(gameManager.buildManager.emptySlots[i].GetComponent<EmptySlotHover>().turretType);
        return result;
    }

    public bool HasUnsavedChanges()
    {
        for (int i = 0; i < 4; i++)
        {
            List<TurretType> current;

            if (i == gameManager.uiManager.currentLoadoutIndex - 1)
                current = GetLiveLoadoutFromSlots();
            else
                current = gameManager.uiManager.temporaryLoadouts[i];

            if (!IsSameLoadout(current, lastSavedLoadouts[i]))
                return true;
        }

        return false;
    }
}
