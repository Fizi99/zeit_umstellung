using UnityEngine;
using UnityEngine.UI;

public class TurretAI : MonoBehaviour
{

    private Transform target;
    public float range = 3f;
    public string enemyTag = "Enemy";
    public bool isSingleUse=false;
    public bool useTimeInsteadOfAmmo = false;
    public int initUseAmount = 1;
    public int useAmount = 1;
    public Image useBar;

    public bool isMoving = false;
    public float speed = 30f;
    public float stopAndShootRange = 5f;


    public float fireRate = 15f;
    private float fireCountdown = 0f;

    public GameObject bulletPrefab;
    //bullet spawn position
    public Transform firePoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        useAmount = initUseAmount;
       InvokeRepeating("UpdateTarget", 0f, 0.5f);

    }

    void UpdateTarget()
    {
        GameObject[]enemies=GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;
        target = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (shortestDistance > distanceToEnemy)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

            if (nearestEnemy != null && shortestDistance <= range)
            {
                target=nearestEnemy.transform;
            }
    }

    // Update is called once per frame
    void Update()
    {
        if(isSingleUse && useTimeInsteadOfAmmo )
        {
            UpdateUseAmount((int)Time.deltaTime);
        }
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            //langsamere rotation in Video, allerdings nicht hingekriegt
            Vector3 rotation = lookRotation.eulerAngles;
            //anpassen in 3D
            transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
            if (isMoving)
            {
                float distanceThisFrame = speed * Time.deltaTime;


                transform.LookAt(target);
                if (dir.magnitude > stopAndShootRange)
                {
                    transform.Translate(dir.normalized * distanceThisFrame, Space.World);
                }
                else if (fireCountdown <= 0f && dir.magnitude <= stopAndShootRange)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;

                }
            }
            else if (fireCountdown <= 0f && !isMoving)
            {
                Shoot();
                fireCountdown = 1f / fireRate;
            }
            fireCountdown -=Time.deltaTime;
            }
        else
        {
            return;
        }

        UpdateTarget();
    }

    void Shoot()
    {
        //define firepoint depending on turret before using this
        //Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        GameObject currentBullet=(GameObject)Instantiate(bulletPrefab, transform.position, transform.rotation);
        bulletAI bullet = currentBullet.GetComponent<bulletAI>();
        if (bullet != null)
        {
            bullet.SetBulletTarget(target);
        }
        if (isSingleUse && !useTimeInsteadOfAmmo)
        {
            UpdateUseAmount(1);
        }
    }

    void UpdateUseAmount(int usageUsed)
    {
        useAmount=useAmount-usageUsed;
        useBar.fillAmount= useAmount/initUseAmount;
        if (useAmount <= 0)
        {
            Destroy(gameObject);
            return;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

    }
}
