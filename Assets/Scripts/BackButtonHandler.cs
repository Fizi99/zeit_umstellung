using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    public GameObject confirmationPopup; // das Popup-Panel
    public SaveLoadout saveLoadout;
    private GameManager gameManager;
    private AudioManager audioManager;


    private void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    public void OnBackPressed()
    {
        if (saveLoadout.HasUnsavedChanges())
        {
            Debug.Log("Auf Änderungen hinweisen.");
            confirmationPopup.SetActive(true);
            this.audioManager.PlaySfx(this.audioManager.soundLibrary.sfxUIError);
        }
        else
        {
            Debug.Log("Zurück ohne Änderungen.");
            this.gameManager.uiManager.NavigateToMainMenu();
        }
    }
}
