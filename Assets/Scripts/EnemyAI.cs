using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class EnemyAI : MonoBehaviour
{
    public float initSpeed = 3f;
    private float speed = 3f;
    public int initHealth = 100;
    private int health = 100;
    public Image healthbar;
    public bool isSplitter = false;
    public int splitAmount = 0;
    public Transform splitUnit;
    private GameManager gameManager;
    public int auraRadius =0;
    public float auraEffectStrength = 2f;
    public float auraDuration = 2f;

    public List<GameObject> targets = new List<GameObject>();
    public GameObject currentTarget;
    public int waypointIndex = 0;
    private float slowCountdown = 0f;

    public int cost = 10;

    public int damage = 1;

    void Awake()
    {
        speed=initSpeed;
        health = initHealth;
    }

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void setTargetList(List<GameObject> targetList)
    {
        targets = targetList;
        currentTarget = targetList[waypointIndex];
    }

    void Update()
    {
        //might be Vector2

        Vector3 direction = currentTarget.transform.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if (slowCountdown > 0)
        {
            slowCountdown -= Time.deltaTime;
            if (0 >= slowCountdown)
            {
                speed = initSpeed;
            }
        }


        if (Vector3.Distance(transform.position, currentTarget.transform.position) < 0.2f)
        {
            GetNextWaypoint();
        }
        if (auraRadius > 0)
        {
            AuraCollider();

        }
    }

    void AuraCollider()
    {
        Collider[] collidersHit = Physics.OverlapSphere(transform.position, auraRadius);
        foreach (Collider collider in collidersHit)
        {
            if (collider.tag == "Enemy")
            {
                AuraEffect(collider.transform);

            }
        }

    }

    void AuraEffect(Transform enemy)
    {
        EnemyAI e = enemy.GetComponent<EnemyAI>();
        if (e != null)
        {
            e.Slow(auraEffectStrength, Time.deltaTime);
        }
    }

    void GetNextWaypoint()
    {
        if (waypointIndex >= targets.Count - 1)
        {
            EndPath();
            return;
        }
        waypointIndex++;
        currentTarget = targets[waypointIndex];
    }

    void EndPath()
    {
        this.gameManager.player.TakeDamage(damage);
        //Debug.Log("lives: " + playerStats.lives);
        Destroy(gameObject);
        return;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        healthbar.fillAmount = (float) health / (float) initHealth;
        if(health <= 0)
        {
            Die();
        }
    }

    public void Slow(float slowMultiplier, float freezeDuration) { 
        speed *= slowMultiplier;
        slowCountdown = freezeDuration;
    }


    void Die()
    {
        if (isSplitter)
        {
            for(int i = 0; i < splitAmount; i++)
            {
                Transform newEnemy = Instantiate(splitUnit, targets[waypointIndex].transform.position, targets[waypointIndex].transform.rotation);
                newEnemy.GetComponent<EnemyAI>().targets = targets;
                newEnemy.GetComponent<EnemyAI>().currentTarget = newEnemy.GetComponent<EnemyAI>().targets[waypointIndex];
                newEnemy.GetComponent<EnemyAI>().waypointIndex = waypointIndex;
                newEnemy.transform.parent = gameManager.enemyContainer.transform;
            }
        }
        Destroy(gameObject);
    }

}
