using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    public GameObject confirmationPopup; // das Popup-Panel
    public SaveLoadout saveLoadout;
    private GameManager gameManager;

    private void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnBackPressed()
    {
        if (saveLoadout.HasUnsavedChanges())
        {
            Debug.Log("Auf �nderungen hinweisen.");
            confirmationPopup.SetActive(true);
        }
        else
        {
            Debug.Log("Zur�ck ohne �nderungen.");
            this.gameManager.uiManager.NavigateToMainMenu();
        }
    }
}
