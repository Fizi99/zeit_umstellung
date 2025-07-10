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
        Debug.Log("�nderungen verworfen. Zur�ck.");
        this.gameManager.uiManager.NavigateToMainMenu();
    }

    public void OnConfirmNo()
    {
        Debug.Log("Zur�ck abgebrochen.");
        confirmationPopup.SetActive(false);
    }
}