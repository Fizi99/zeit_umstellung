using UnityEngine;
using System.Collections.Generic;


public class EnemyAI : MonoBehaviour
{
    public float initSpeed = 3f;
    private float speed = 3f;
    public int health = 100;

    public List<Transform> targets = new List<Transform>();
    public Transform currentTarget;
    private int waypointIndex = 0;
    private float slowCountdown = 0f;

    public int damage = 1;

    private GameManager gameManager;

    void Awake()
    {
        speed=initSpeed;
    }

    private void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void setTargetList(List<Transform> targetList)
    {
        targets = targetList;
        currentTarget = targetList[waypointIndex];
    }

    void Update()
    {
        //might be Vector2

        Vector3 direction = currentTarget.position - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if (slowCountdown > 0)
        {
            slowCountdown -= Time.deltaTime;
            if (0 >= slowCountdown)
            {
                speed = initSpeed;
            }
        }


        if (Vector3.Distance(transform.position, currentTarget.position) < 0.2f)
        {
            GetNextWaypoint();
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
        Destroy(gameObject);
    }

}
