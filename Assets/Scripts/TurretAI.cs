using UnityEngine;

public class TurretAI : MonoBehaviour
{

    private Transform target;
    public float range = 3f;
    public string enemyTag = "Enemy";
    public bool isSingleUse=false;
    public int useAmount = 1;


    public float fireRate = 15f;
    private float fireCountdown = 0f;

    public GameObject bulletPrefab;
    //bullet spawn position
    public Transform firePoint;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            //langsamere rotation in Video, allerdings nicht hingekriegt
            Vector3 rotation = lookRotation.eulerAngles;
            //anpassen in 3D
            transform.rotation = Quaternion.Euler(0f, 0f, rotation.y);
            if (fireCountdown <= 0f)
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
        if (isSingleUse)
        {
            useAmount--;
            if (useAmount <= 0)
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

    }
}
