using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefab1;
    public Transform enemyPrefab2;
    public Transform enemyPrefab3;

    public float timeBetweenWaves = 5f;
    public float countdown = 3f;

    public int amountOfRoutes = 0;
    private int waveNumber = 1;

    private int j = 9;
    private int k = 0;

    public Transform[] targetList;

    void Start()
    {
        amountOfRoutes = routeManager.routeList.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(countdown <=0)
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
            for (int i = 0; i < amountOfRoutes; i++)
            {
                SpawnEnemy1(i);
            }
        }
        else if (waveNumber == 2)
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
        }
        else
        {
            for (int i = 0; i < amountOfRoutes; i++)
            {
                if (i == 1)
                {
                    SpawnEnemy1(i);
                }
                else
                {
                    SpawnEnemy3(i);
                }
            }
        }
        waveNumber++;
        Debug.Log("Wave Incoming!");
    }

    void setAmountOfRoutes(int amountOfRoutes)
    {
        this.amountOfRoutes = amountOfRoutes;
    }

    void SpawnEnemy1(int routeIndex)
    {
        Transform[] targets = routeManager.routeList[routeIndex].GetComponent<enemyAI>().waypoints;
        Transform newEnemy = Instantiate(enemyPrefab1, targets[0].position, targets[0].rotation);
        newEnemy.GetComponent<whereToGo>().targets = targets;
        newEnemy.GetComponent<whereToGo>().currentTarget = newEnemy.GetComponent<whereToGo>().targets[1];
    }

    void SpawnEnemy2(int routeIndex)
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
    }

}
