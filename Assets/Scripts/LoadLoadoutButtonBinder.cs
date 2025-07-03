using UnityEngine;
using UnityEngine.UI;

public class LoadLoadoutButtonBinder : MonoBehaviour
{

    public int index;
    private GameManager gameManager;

    public GameObject saveBtn;

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        GetComponent<Button>().onClick.AddListener(() =>
        {
            //setze das temporäre loadout auf das bis jetzt angezeigte loadout
            this.gameManager.uiManager.temporaryLoadouts[this.gameManager.uiManager.currentLoadoutIndex - 1] = this.gameManager.uiManager.shownLoadout;
            Debug.Log("saved the current loadout temporarily: "+string.Join(", ", this.gameManager.uiManager.temporaryLoadouts[this.gameManager.uiManager.currentLoadoutIndex - 1]));
            //aktualisiere das gezeigt loadout mit dem temporär gespeicherten loadout
            this.gameManager.uiManager.shownLoadout = this.gameManager.uiManager.temporaryLoadouts[index-1];
            //for (int i = 0; i < 4; i++)
            //{
            //    if (i + 1 != index)
            //    {
            //        this.gameManager.uiManager.temporaryLoadouts[i] = SaveManager.LoadLoadout(i + 1);
            //    }
            //}
            
            //zeige das aktuelle loadout auch an
            this.gameManager.uiManager.fillEmptySlots(this.gameManager.uiManager.shownLoadout);

            //aktualisiere den index, der das aktuelle loadout tracked
            this.gameManager.uiManager.currentLoadoutIndex = index;
        });
    }
}