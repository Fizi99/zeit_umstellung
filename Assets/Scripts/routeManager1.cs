using UnityEngine;

public class routeManager : MonoBehaviour
{
    public static Transform[] routeList;

    void Awake()
    {
        routeList = new Transform[transform.childCount];

        for (int i = 0; i < routeList.Length; i++)
        {
            //add list not child
            routeList[i] = transform.GetChild(i);
            Debug.Log("added Route number "+i);
        }
    }
}
