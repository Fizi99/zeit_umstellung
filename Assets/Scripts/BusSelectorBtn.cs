using UnityEngine;
using UnityEngine.UI;

public class BusSelectorBtn : MonoBehaviour
{

    private GameManager gameManager;
    public Bus bus;


    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnClick()
    {
        this.gameManager.selectedBus = bus;
    }

}
