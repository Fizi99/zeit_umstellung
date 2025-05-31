using UnityEngine;


public class buildManager : MonoBehaviour
{

    public static buildManager instance;

    public GameObject turretContainer;

    public GameObject turretArtillery;
    public GameObject turretLaser;
    public GameObject turretRocket;
    public GameObject turretFreeze;
    public GameObject turretBomb;
    public GameObject turretDrone;
    public Camera mainCamera;

    private GameManager gameManager;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("more than one BuildManager!");
            return;
        }
        instance = this;

    }

    void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // only place turrets, when lvl is in session
        if(this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            if (Input.GetMouseButtonDown(0))
            {
                spawnTurret(turretArtillery);
                //spawnTurret(turretFreeze);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                //spawnTurret(turretLaser);
                spawnTurret(turretBomb);
            }
            else if (Input.GetMouseButtonDown(2))
            {
                //spawnTurret(turretRocket);
                spawnTurret(turretDrone);
            }
        }

       
    }

    //implement logic with shop
    public void GetTurretToBuild()
    {
        return;
    }

  void spawnTurret(GameObject turret)
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 spawnPosition = hit.point; 
            
            Collider[] collidersHit = Physics.OverlapSphere(spawnPosition, 1f);
            bool turretOverlap = false;
            foreach (Collider collider in collidersHit)
            {
                if (collider.tag == "Turret")
                {
                    turretOverlap = true;
                }
            }
            if (!turretOverlap)
            {
                GameObject newTurret = Instantiate(turret, spawnPosition, Quaternion.identity);
                newTurret.transform.parent = turretContainer.transform;
            }
            else
            {
                Debug.LogWarning("Turret already there");
            }
        }

    }
}
