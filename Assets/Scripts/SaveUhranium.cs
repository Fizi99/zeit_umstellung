using UnityEngine;
using UnityEngine.UI;

public class SaveUhranium : MonoBehaviour
{
    private GameManager gameManager;

    
    public GameObject popupText;
    public GameObject particleEffect;
    public GameObject fillableUhraniumBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVisualEffect();
        if (Input.GetMouseButtonDown(0))
            CheckRaycastHit();
    }

    // check if busstop got tapped
    private void CheckRaycastHit()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        LayerMask mask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            if (hit.collider.tag == "Busstop")
            {
                SaveUhraniumClicked();
            }
        }

    }

    // save currently collected uhranium
    private void SaveUhraniumClicked()
    {
        if(this.gameManager.player.uhranium >= this.gameManager.player.savableThreshhold)
        {
            this.gameManager.SpawnFloatingText(new Vector3(0, 1, -1), "+" + (int)this.gameManager.player.uhranium, new Color(0.5800107f, 0.9245283f, 0.8310998f));
            this.gameManager.player.SaveUhranium();
            
        }

    }

    // update particle system, popup text and fillbar every frame, depending on collected uhranium
    private void UpdateVisualEffect()
    {
        float uhraniumRatio = this.gameManager.player.uhranium / this.gameManager.player.savableThreshhold;
        this.fillableUhraniumBar.GetComponent<Image>().fillAmount = uhraniumRatio;

        if (this.gameManager.player.uhranium >= this.gameManager.player.savableThreshhold)
        {
            this.popupText.SetActive(true);
            this.particleEffect.SetActive(true);
        }
        else
        {
            this.popupText.SetActive(false);
            this.particleEffect.SetActive(false);
        }

    }

}
