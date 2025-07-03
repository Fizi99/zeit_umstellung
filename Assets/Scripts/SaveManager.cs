using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SaveManager
{
    public static void SaveUhraniumHighscore(float score)
    {
        PlayerPrefs.SetFloat("Highscore", score);
        PlayerPrefs.Save();
        //Debug.Log("Current All-Time Highscore: " + PlayerPrefs.GetFloat("Highscore", 0f));
    }

    public static float LoadUhraniumHighscore()
    {
        //Debug.Log("Loaded Highscore: " + PlayerPrefs.GetFloat("Highscore", 0f));
        return PlayerPrefs.GetFloat("Read of Highscore Value gives: ", 0f);
    }


    /*
     * Because PlayerPrefs can only handle primitives, we serialize a list of enum values to a string
     * and deserialize it when loading 
     */

    public static void SavePurchasedTurrets(List<TurretType> purchasedTurrets)
    {
        string data = string.Join(",", purchasedTurrets.Select(e => e.ToString()));
        PlayerPrefs.SetString("PurchasedTurrets", data);
        PlayerPrefs.Save();
    }

    public static List<TurretType> LoadPurchasedTurrets()
    {
        string serializedData = PlayerPrefs.GetString("PurchasedTurrets", "");

        if (string.IsNullOrEmpty(serializedData))
            return new List<TurretType>();

        return serializedData
            .Split(',')
            .Select(s => (TurretType)Enum.Parse(typeof(TurretType), s))
            .ToList();
    }

    public static void SaveLoadout(int loadOutIndex, List<TurretType> chosenTurrets)
    {
        if (4 >= loadOutIndex && loadOutIndex > 0)
        {

            string data = string.Join(",", chosenTurrets.Select(e => e.ToString()));
            PlayerPrefs.SetString("Loadout" + loadOutIndex, data);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.Log("loadoutindex out of range");
        }
    }

    public static List<TurretType> LoadLoadout(int loadOutIndex)
    {
        if (4 >= loadOutIndex && loadOutIndex > 0)
        {
            string serializedData = PlayerPrefs.GetString("Loadout" + loadOutIndex, "");

            if (string.IsNullOrEmpty(serializedData))
                return new List<TurretType>() {
                    TurretType.STANDARD,
                    TurretType.MISSILE,
                    TurretType.DRONEBASE,
                    TurretType.LASER };

            return serializedData
                .Split(',')
                .Select(s => (TurretType)Enum.Parse(typeof(TurretType), s))
                .ToList();
        }
        else
        {
            Debug.Log("loadoutindex out of range");
            return new List<TurretType>();
        }
    }
}
