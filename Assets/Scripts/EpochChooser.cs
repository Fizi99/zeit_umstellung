using System.Collections.Generic;
using UnityEngine;

public class EpochChooser
{
    private List<Epoch> epochBag = new List<Epoch>();
    private readonly Epoch[] allEpochs = { Epoch.PREHISTORIC, Epoch.PHARAOH, Epoch.MEDIEVAL };
    private const int totalPerCycle = 5;

    public Epoch currentEpoch = Epoch.PHARAOH;

    public Epoch GetNextEpoch()
    {
        if (epochBag.Count == 0)
            RefillEpochBag();

        Epoch next = epochBag[0];
        epochBag.RemoveAt(0);
        this.currentEpoch = next;
        return next;
    }

    private void RefillEpochBag()
    {
        // Fuge jede Epoche garantiert einmal hinzu
        List<Epoch> newEpochs = new List<Epoch>(allEpochs);

        // Fuge weitere (zufallige) Epochen hinzu, bis wir 5 haben
        while (newEpochs.Count < totalPerCycle)
        {
            newEpochs.Add(RandomEpoch());
        }

        Shuffle(newEpochs);
        epochBag = newEpochs;

        Debug.Log("EpochBag neu gefullt: " + string.Join(", ", epochBag));
    }

    private Epoch RandomEpoch()
    {
        return allEpochs[Random.Range(0, allEpochs.Length)]; // UnityEngine.Random
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