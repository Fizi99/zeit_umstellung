using UnityEngine;

public class ConfirmPopupHandler : MonoBehaviour
{
    public GameObject confirmationPopup;
    private GameManager gameManager;

    private void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnConfirmYes()
    {
        confirmationPopup.SetActive(false);
        Debug.Log("Änderungen verworfen. Zurück.");
        this.gameManager.uiManager.NavigateToMainMenu();
    }

    public void OnConfirmNo()
    {
        Debug.Log("Zurück abgebrochen.");
        confirmationPopup.SetActive(false);
    }
}