using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class buildManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{

    public static buildManager instance;

    public GameObject turretToBuild;

    public GameObject turretContainer;

    public GameObject turretArtillery;
    public GameObject turretLaser;
    public GameObject turretRocket;
    public GameObject turretFreeze;
    public GameObject turretBomb;
    public GameObject turretDrone; 
    public Camera mainCamera;
    private float turretLoadoutEfficiency = 1f;
    private float loadOutSize = 4f;
    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button Button4;

    public GameObject DragObject;

    public PlaceableZone placeableZone;
    private Button lastlySelectedButton;

    public bool isBuildPossible = false;
    private bool isDragging = false;

    private GameManager gameManager;

    private GameObject CurrentDragObject;
    private bool isHovering = false;
    private displayTurretCost buttonScript;

    public UIManager UiManager;

    public List<GameObject> uiLoadoutList;
    public List<GameObject> emptySlots = new List<GameObject>();
    public List<Button> ButtonList = new List<Button>();

    public Sprite currentButtonSprite;
    public Texture currentTexture;
    public GameObject CurrentTurret;
    public GameObject changedDragObject;
    public TurretType currentTurretType;

    public Canvas canvas;

    public GameObject frameObject;



    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("more than one BuildManager!");
            return;
        }
        instance = this;

    }

    void Start()
    {
        /*turretLoadoutEfficiency = ((turretArtillery.GetComponent <TurretAI>().getTurretEfficiency()+
            turretLaser.GetComponent<TurretAI>().getTurretEfficiency()+
            turretRocket.GetComponent<TurretAI>().getTurretEfficiency()+
            turretDrone.GetComponent<TurretAI>().getTurretEfficiency() )/ loadOutSize)/4;*/
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.UiManager = gameManager.uiManager;

        emptySlots = gameManager.uiManager.loadoutPanel
            .GetComponentsInChildren<Transform>()
            .Where(t => t.name.StartsWith("emptySlot"))
            .Select(t => t.gameObject)
            .ToList();
        UiManager.initTemporaryLoadouts();
        ButtonList.Add(Button1 );
        ButtonList.Add(Button2 );  
        ButtonList.Add(Button3 );
        ButtonList.Add(Button4 );
    }

    public float getLoadoutEfficiency()
    {
        return turretLoadoutEfficiency;
    }

    public void addToUiLoadoutList(GameObject gameObject)
    {
        uiLoadoutList.Add(gameObject);
    }

    void Update()
    {
        // only place turrets, when lvl is in session
        if (this.gameManager.gameState == GameState.LEVELPLAYING)
        {

            if (Input.GetMouseButtonUp(0) && 400f < Input.mousePosition.x)
            {
                spawnTurret(turretToBuild);
            }
            if (Input.GetMouseButtonDown(0) && Input.mousePosition.x < 400f)
            {
                clearHighlight();
            }
            if (Input.GetMouseButtonDown(0) && 400f < Input.mousePosition.x )
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    Vector3 spawnPosition = hit.point;
                    Collider[] collidersHit = Physics.OverlapSphere(spawnPosition, 0.03f);
                    foreach (Collider collider in collidersHit)
                    {
                        if (collider.tag == "Drop")
                        {
                            float dropAmount = collider.gameObject.GetComponent<DropProperties>().dropAmount;
                            this.gameManager.player.addZeitsand(dropAmount);
                            gameManager.SpawnFloatingText(collider.transform.position, "+"+ dropAmount+" Zeitsand!", Color.yellow);
                            Destroy(collider.gameObject);
                        }
                    }
                }
            }
        }


    }

    public void setIsBuild(bool isBuild)
    {
        isBuildPossible = isBuild;
    }

    public void SetTurretToBuild(GameObject turret)
    {
        turretToBuild = turret;
        setIsBuild(true);
        placeableZone.ShowPlaceableZone();
    }

    public void SetDragObject(GameObject DragObject)
    {
        this.DragObject = DragObject;
    }

    public void highlightTowerSelected(Button button)
    {
        clearHighlight();
        lastlySelectedButton = button;
        var childImage = button.GetComponentInChildren<RawImage>();
        button.GetComponent<displayTurretCost>().isTowerSelected = true;
        childImage.color = new Color(0.6f, 0.8f, 1f, 1f); // leichter blauer Tint
    }

    public void clearHighlight()
    {
        if (lastlySelectedButton != null)
        {
            var image = lastlySelectedButton.GetComponentInChildren<RawImage>();
            if (image != null)
                image.color = Color.white;

            lastlySelectedButton.GetComponent<displayTurretCost>().isTowerSelected = false;
            lastlySelectedButton = null;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        if (gameManager.gameState == GameState.LOADOUTCREATION)
        {
            for (int i = 0; i < uiLoadoutList.Count; i++)
            {
                if (uiLoadoutList[i].GetComponent<displayTurretCost>().isHovering && SaveManager.LoadPurchasedTurrets().Contains(uiLoadoutList[i].GetComponent<displayTurretCost>().turretType))
                {
                    currentButtonSprite = uiLoadoutList[i].GetComponent<displayTurretCost>().ButtonSprite;
                    
                    CurrentDragObject = Instantiate(uiLoadoutList[i].GetComponent<displayTurretCost>().DragObject, Input.mousePosition, Quaternion.identity);



                    CurrentDragObject.transform.Find("TurretImage").GetComponent<RawImage>().texture = uiLoadoutList[i].GetComponent<displayTurretCost>().texture;
                    CurrentTurret = uiLoadoutList[i].GetComponent<displayTurretCost>().turret;
                    CurrentDragObject.GetComponent<CarryTurretInfo>().costText.GetComponent<TMPro.TextMeshProUGUI>().text
                        = CurrentTurret.GetComponent<TurretAI>().buildingCost.ToString();


                    changedDragObject = uiLoadoutList[i].transform.GetComponent<displayTurretCost>().ChangedDragObject;
                    currentTurretType = uiLoadoutList[i].transform.GetComponent<displayTurretCost>().turretType;
                    //CurrentDragObject = Instantiate(frameObject, Input.mousePosition, Quaternion.identity);
                    RectTransform rt = CurrentDragObject.GetComponent<RectTransform>();
                        rt.anchoredPosition = Vector2.zero; // Mitte
                        rt.sizeDelta = new Vector2(100, 100); // Sichtbare Fl�che, falls leer
                        CurrentDragObject.transform.SetParent(canvas.transform, false);
                        CurrentDragObject.transform.SetAsLastSibling(); // Ganz oben zeichnen (sichtbar)

                        return;
                }
            }
        }

        //if (isBuildPossible)
        if (Button1.GetComponent<displayTurretCost>().isHovering)
        {
            isHovering = true;
            lastlySelectedButton = Button1;
            buttonScript = Button1.GetComponent<displayTurretCost>();
        }
        else if (Button2.GetComponent<displayTurretCost>().isHovering)
        {
            isHovering = true;
            lastlySelectedButton = Button2;
            buttonScript = Button2.GetComponent<displayTurretCost>();
        }
        else if(Button3.GetComponent<displayTurretCost>().isHovering)
        {
            isHovering = true;
            lastlySelectedButton = Button3;
            buttonScript = Button3.GetComponent<displayTurretCost>();
        }
        else if(Button4.GetComponent<displayTurretCost>().isHovering)
        {
            isHovering = true;
            lastlySelectedButton = Button4;
            buttonScript = Button4.GetComponent<displayTurretCost>();
        }

        if (isHovering)
        {
            SetTurretToBuild(buttonScript.turret);
            highlightTowerSelected(lastlySelectedButton);
            SetDragObject(buttonScript.DragObject);
            setIsBuild(true);
        }

        if(gameManager.gameState == GameState.LEVELPLAYING && isBuildPossible && buttonScript.turret.GetComponent<TurretAI>().buildingCost <= gameManager.player.zeitsand)
        {
            Debug.Log("Start drag");
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 spawnPosition = hit.point;
                spawnPosition.z = 0; // Make the tower be in the base depth
                CurrentDragObject = Instantiate(DragObject, spawnPosition, Quaternion.identity);
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CurrentDragObject != null && gameManager.gameState == GameState.LEVELPLAYING)
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 spawnPosition = hit.point;
                spawnPosition.z = 0; // Make the tower be in the base depth
                CurrentDragObject.transform.position = spawnPosition;
                Collider[] collidersHit = Physics.OverlapSphere(spawnPosition, 0.1f);
                bool turretOverlap = false;
                foreach (Collider collider in collidersHit)
                {
                    //make current dragobject red if in unplacable zone
                    //if (collider.tag == "Turret" || collider.tag == "Busstop" || Input.mousePosition.x < Screen.width
                    //    || 400f < Input.mousePosition.x || Input.mousePosition.y < Screen.height || 0 < Input.mousePosition.y)
                    //if (collider.tag == "Turret" || collider.tag == "Busstop")

                    if (collider.tag == "Turret" || collider.tag == "Busstop")
                    {
                        turretOverlap = true;
                    }
                }
                Color newColor;
                if (turretOverlap)
                {
                    newColor = Color.red;
                }
                else
                {
                    newColor = Color.white;
                    newColor.a = 0.5f;
                }
                CurrentDragObject.GetComponent<SpriteRenderer>().color = newColor;
            }
        }
        else if (CurrentDragObject != null && gameManager.gameState == GameState.LOADOUTCREATION)
        {
            CurrentDragObject.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (gameManager.gameState == GameState.LOADOUTCREATION && CurrentDragObject != null)
        {
            for (int i = 0; i < emptySlots.Count; i++)
            {
                if (emptySlots[i].GetComponent<EmptySlotHover>().isHovering)
                {
                    if (SaveManager.LoadPurchasedTurrets().Contains(CurrentTurret.GetComponent<TurretAI>().name) )
                    {

                    emptySlots[i].GetComponent<Image>().sprite = currentButtonSprite;
                    emptySlots[i].GetComponent<EmptySlotHover>().texture = CurrentDragObject.transform.Find("TurretImage").GetComponent<RawImage>().texture;
                    emptySlots[i].GetComponent<EmptySlotHover>().Turret = CurrentTurret;
                    emptySlots[i].GetComponent<EmptySlotHover>().changedDragObject = changedDragObject;
                        emptySlots[i].GetComponent<EmptySlotHover>().turretType = currentTurretType;
                        this.gameManager.uiManager.shownLoadout[i] = currentTurretType;
                    }
                    else
                    {
                        gameManager.SpawnFloatingText(new Vector3(0, 1, -1), "Turm erst kaufen!", Color.red);
                    }
                }
            }

            Destroy(CurrentDragObject);
        }
        if (gameManager.gameState == GameState.LEVELPLAYING) {
            spawnTurret(turretToBuild);
        }
        isDragging = false;
        CurrentDragObject = null;
        isHovering = false;
    }

    void spawnTurret(GameObject turret)
    {
        if (isBuildPossible)
        {
            Destroy(CurrentDragObject);

            if (Input.mousePosition.x < Screen.width && 400f < Input.mousePosition.x && Input.mousePosition.y < Screen.height && 0 < Input.mousePosition.y)
            {
                if (turret.GetComponent<TurretAI>().buildingCost <= gameManager.player.zeitsand)
                {
                    //check for turret overlap
                    Vector3 mousePosition = Input.mousePosition;
                    Ray ray = mainCamera.ScreenPointToRay(mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Vector3 spawnPosition = hit.point;
                        spawnPosition.z = 0; // Make the tower be in the base depth
                        Collider[] collidersHit = Physics.OverlapSphere(spawnPosition, 0.1f);
                        bool turretOverlap = false;
                        foreach (Collider collider in collidersHit)
                        {
                            if (collider.tag == "Turret" || collider.tag == "Busstop")
                            {
                                turretOverlap = true;
                            }
                        }

                        if (!turretOverlap)
                        {
                            gameManager.player.SetZeitsand(gameManager.player.zeitsand - turret.GetComponent<TurretAI>().buildingCost);
                            GameObject newTurret = Instantiate(turret, spawnPosition, Quaternion.identity);
                            newTurret.transform.parent = turretContainer.transform;
                        }
                        else
                        {
                            gameManager.SpawnFloatingText(spawnPosition, "Turm bereits hier!", Color.red);
                        }
                    }
                    else
                    {
                        Debug.Log("Ray had no collision!");
                    }
                }
                else
                {
                    gameManager.SpawnFloatingText(new Vector3(0, 1, -1), "kein Zeitsand!", Color.red);
                }
            }
            else
            {
                gameManager.SpawnFloatingText(new Vector3(0, 1, -1), "Hier nicht!", Color.red);
            }

        }
        else
        {
            Debug.Log("Build not possible!");
        }
        placeableZone.HidePlaceableZone();
        clearHighlight();
        setIsBuild(false);
    }


    public void SetBuyButtons()
    {
        List <Button> buttonList = new List<Button> ();
        buttonList.Add(Button1);
        buttonList.Add(Button2);
        buttonList.Add(Button3);
        buttonList.Add(Button4);
        for (int buttonIndex = 0; buttonIndex < buttonList.Count; buttonIndex++)
        {
            List<TurretPrefabMapping> turretMappings = this.gameManager.uiManager.loadoutPanel.GetComponentInChildren<TurretGridFiller>().turretMappings;
            var result = turretMappings.FirstOrDefault(x => x.data.turretType == this.gameManager.player.chosenLoadout[buttonIndex]);


            buttonList[buttonIndex].transform.Find("TurretImage").GetComponent<RawImage>().texture = result.data.turretIconFramed;
            buttonList[buttonIndex].GetComponent<displayTurretCost>().turret = result.prefab;
            buttonList[buttonIndex].GetComponent<displayTurretCost>().DragObject = result.DragObject;
            buttonList[buttonIndex].GetComponent<displayTurretCost>().updateTurretCostText();
        }
    }

}
