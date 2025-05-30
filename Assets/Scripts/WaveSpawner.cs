using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefab1;
    public Transform enemyPrefab2;
    public Transform enemyPrefab3;

    public GameObject enemyContainer;

    public float timeBetweenWaves = 5f;
    public float countdown = 3f;

    public int amountOfRoutes = 0;
    private int waveNumber = 1;

    public Transform[] targetList;

    private GameManager gameManager;

    void Start()
    {
        //amountOfRoutes = routeManager.routeList.Length;
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(countdown <=0 && this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            SpawnWave();
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime;
    }

    void SpawnWave()
    {
        if (waveNumber == 1)
        {
            for (int i = 0; i < this.gameManager.routes.Count; i++)
            {
                SpawnEnemy1(i);
            }
        }
        /*else if (waveNumber == 2)
        {
            for (int i = 0; i < amountOfRoutes; i++)
            {
                SpawnEnemy2(i);
            }
        }
        else if (waveNumber == 3)
        {
            for (int i = 0; i < amountOfRoutes; i++)
            {
                SpawnEnemy3(i);
            }
        }*/
        else
        {
            for (int i = 0; i < this.gameManager.routes.Count; i++)
            {
                SpawnEnemy1(i);
               /* if (i == 1)
                {
                    SpawnEnemy1(i);
                }
                else
                {
                    SpawnEnemy3(i);
                }*/
            }
        }
        waveNumber++;
    }

    public void setAmountOfRoutes(int amountOfRoutes)
    {
        this.amountOfRoutes = amountOfRoutes;
    }

    void SpawnEnemy1(int routeIndex)
    {
        List<GameObject> targets = this.gameManager.routes[routeIndex];
        Transform newEnemy = Instantiate(enemyPrefab1, targets[0].transform.position, targets[0].transform.rotation);
        newEnemy.GetComponent<EnemyAI>().targets = targets;
        newEnemy.GetComponent<EnemyAI>().currentTarget = newEnemy.GetComponent<EnemyAI>().targets[1];
        newEnemy.transform.parent = enemyContainer.transform;

        /*Transform[] targets = routeManager.routeList[routeIndex].GetComponent<enemyAI>().waypoints;
        Transform newEnemy = Instantiate(enemyPrefab1, targets[0].position, targets[0].rotation);
        newEnemy.GetComponent<whereToGo>().targets = targets;
        newEnemy.GetComponent<whereToGo>().currentTarget = newEnemy.GetComponent<whereToGo>().targets[1];*/
    }

    /*void SpawnEnemy2(int routeIndex)
    {
        Transform[] targets = routeManager.routeList[routeIndex].GetComponent<enemyAI>().waypoints;
        Transform newEnemy = Instantiate(enemyPrefab2, targets[0].position, targets[0].rotation);
        newEnemy.GetComponent<whereToGo>().targets = targets;
        newEnemy.GetComponent<whereToGo>().currentTarget = newEnemy.GetComponent<whereToGo>().targets[1];
    }

    void SpawnEnemy3(int routeIndex)
    {
        Transform[] targets = routeManager.routeList[routeIndex].GetComponent<enemyAI>().waypoints;
        Transform newEnemy = Instantiate(enemyPrefab3, targets[0].position, targets[0].rotation);
        newEnemy.GetComponent<whereToGo>().targets = targets;
        newEnemy.GetComponent<whereToGo>().currentTarget = newEnemy.GetComponent<whereToGo>().targets[1];
    }*/

}
