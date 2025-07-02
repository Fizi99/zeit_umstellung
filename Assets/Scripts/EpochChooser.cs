using System.Collections.Generic;
using UnityEngine;

public class EpochChooser
{
    private List<int> epochBag = new List<int>();
    private readonly int[] allEpochs = { 0, 1, 2 };
    private const int totalPerCycle = 5;

    public int GetNextEpoch()
    {
        if (epochBag.Count == 0)
            RefillEpochBag();

        int next = epochBag[0];
        epochBag.RemoveAt(0);
        return next;
    }

    private void RefillEpochBag()
    {
        // Fuge jede Epoche garantiert einmal hinzu
        List<int> newEpochs = new List<int>(allEpochs);

        // Fuge weitere (zufallige) Epochen hinzu, bis wir 5 haben
        while (newEpochs.Count < totalPerCycle)
        {
            newEpochs.Add(RandomEpoch());
        }

        Shuffle(newEpochs);
        epochBag = newEpochs;

        Debug.Log("EpochBag neu gefullt: " + string.Join(", ", epochBag));
    }

    private int RandomEpoch()
    {
        return Random.Range(0, allEpochs.Length); // UnityEngine.Random
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = 0; i < n - 1; i++)
        {
            int j = Random.Range(i, n);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}