using UnityEngine;
using UnityEngine.UI;

public class DropDownChangeLoadout : MonoBehaviour
{

    [SerializeField] private TMPro.TMP_Dropdown dropdown;
    private GameManager gameManager;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (dropdown != null)
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }


    // Update is called once per frame
    void Update()
    {

    }


    void OnDropdownValueChanged(int index)
    {
        string selectedOption = dropdown.options[index].text;
        Debug.Log("Ausgew�hlt: " + selectedOption);

        // *Bsp:* Logik je nach Auswahl
        switch (index)
        {
            case 0:
                Debug.Log("Erste Option gew�hlt");
                this.gameManager.player.chosenLoadout = SaveManager.LoadLoadout(1);
                this.gameManager.uiManager.UpdateLoadoutVisualization();
                break;
            case 1:
                Debug.Log("Zweite Option gew�hlt");
                this.gameManager.player.chosenLoadout = SaveManager.LoadLoadout(2);
                this.gameManager.uiManager.UpdateLoadoutVisualization();
                break;
            case 2:
                Debug.Log("Dritte Option gew�hlt");
                this.gameManager.player.chosenLoadout = SaveManager.LoadLoadout(3);
                this.gameManager.uiManager.UpdateLoadoutVisualization();
                break;
            case 3:
                Debug.Log("Vierte Option gew�hlt");
                this.gameManager.player.chosenLoadout = SaveManager.LoadLoadout(4);
                this.gameManager.uiManager.UpdateLoadoutVisualization();
                break;
            default:
                Debug.Log("Kein Arsenal gew�hlt");
                break;
        }
    }

    void OnDestroy()
    {
        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }
}
