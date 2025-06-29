using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    private bool isHovering;
    private displayTurretCost buttonScript;
    

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
    }

    public float getLoadoutEfficiency()
    {
        return turretLoadoutEfficiency;
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
            if (Input.GetMouseButtonDown(0) &&  Input.mousePosition.x < 400f)
            {
                clearHighlight();
            }
        }


    }

    public void setIsBuild(bool isBuild)
    {
        isBuildPossible = isBuild;
    }

    public void SetTurretToBuild(GameObject turret)
    {
        Debug.Log("button press");
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

        if((isBuildPossible || isHovering) && buttonScript.turret.GetComponent<TurretAI>().buildingCost <= gameManager.player.zeitsand)
        {
            SetTurretToBuild(buttonScript.turret);
            highlightTowerSelected(lastlySelectedButton);
            SetDragObject(buttonScript.DragObject);
            isBuildPossible = true;
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
        if (CurrentDragObject != null)
        {
            Debug.Log("Dragging");
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 spawnPosition = hit.point;
                spawnPosition.z = 0; // Make the tower be in the base depth
                CurrentDragObject.transform.position = spawnPosition;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        spawnTurret(turretToBuild);
        CurrentDragObject = null;
        isHovering = false;
    }

    void spawnTurret(GameObject turret)
    {
        if (isBuildPossible)
        {
            Debug.Log("stop drag");
            Debug.Log("Pointer position: " + Input.mousePosition);
            Destroy(CurrentDragObject);

            if (Input.mousePosition.x < 1915f && 400f < Input.mousePosition.x && Input.mousePosition.y < 1075 && 0 < Input.mousePosition.y)
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
                            Debug.Log("Turret already at this location!");
                        }
                    }
                    else
                    {
                        Debug.Log("Ray had no collision!");
                    }
                }
                else
                {
                    Debug.Log("Not enough Zeitsand!");
                }
            }
            else
            {
                Debug.Log("Turret outside of placeable Zone!");
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


}
