using UnityEngine;
using TMPro;

public class displayTurretCost : MonoBehaviour
{
    public TMP_Text costText;
    public float cost;
    public GameObject turret;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        costText.text = turret.GetComponent<TurretAI>().name+" cost: "+(turret.GetComponent<TurretAI>().getCalculatedBuildingCost()).ToString();
    }
}
