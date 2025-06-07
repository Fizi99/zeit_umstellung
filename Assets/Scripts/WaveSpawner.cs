using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefabSupport;
    public Transform enemyPrefabSpeed;
    public Transform enemyPrefabStandard;
    public Transform enemyPrefabSplitter;
    public Transform enemyPrefabTank;

    public GameObject enemyContainer;

    public float timeBetweenWaves = 5f;
    public float countdown = 3f;

    public int amountOfRoutes = 0;
    private int waveNumber = 1;

    public int waveBudget = 100;
    private int budgetSpent = 0;
    private Dictionary<EnemyType, int> currentWaveEnemies = new Dictionary<EnemyType, int>();

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
            InitWave();
            SpawnWave();
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime;
    }

    private void InitWave()
    {
        this.currentWaveEnemies = new Dictionary<EnemyType, int>();

        this.currentWaveEnemies.Add(EnemyType.STANDARD, 0);
        this.currentWaveEnemies.Add(EnemyType.SPEED, 0);
        this.currentWaveEnemies.Add(EnemyType.TANK, 0);
        this.currentWaveEnemies.Add(EnemyType.SPLITTER, 0);
        this.currentWaveEnemies.Add(EnemyType.SUPPORT, 0);

        while (this.budgetSpent < this.waveBudget)
        {
            // currently adds enemies randomly
            int i = Random.Range(1, 5);

            switch (i)
            {
                case 1:
                    this.currentWaveEnemies[EnemyType.STANDARD] += 1;
                    this.budgetSpent += enemyPrefabStandard.GetComponent<EnemyAI>().cost;
                    break;
                case 2:
                    this.currentWaveEnemies[EnemyType.SPEED] += 1;
                    this.budgetSpent += enemyPrefabSpeed.GetComponent<EnemyAI>().cost;
                    break;
                case 3:
                    this.currentWaveEnemies[EnemyType.TANK] += 1;
                    this.budgetSpent += enemyPrefabTank.GetComponent<EnemyAI>().cost;
                    break;
                case 4:
                    this.currentWaveEnemies[EnemyType.SPLITTER] += 1;
                    this.budgetSpent += enemyPrefabSplitter.GetComponent<EnemyAI>().cost;
                    break;
                case 5:
                    this.currentWaveEnemies[EnemyType.SUPPORT] += 1;
                    this.budgetSpent += enemyPrefabSupport.GetComponent<EnemyAI>().cost;
                    break;
                default:
                    this.currentWaveEnemies[EnemyType.STANDARD] += 1;
                    this.budgetSpent += enemyPrefabStandard.GetComponent<EnemyAI>().cost;
                    break;
            }
        }



        this.budgetSpent = 0;
    }

    void SpawnWave()
    {

        foreach(KeyValuePair<EnemyType, int> kvp in currentWaveEnemies)
        {
            for(int i = 0; i < kvp.Value; i++)
            {
                int j = Random.Range(0, this.gameManager.routes.Count-1);
                switch (kvp.Key)
                {
                    case EnemyType.STANDARD:
                        SpawnEnemy(j, this.enemyPrefabStandard);
                        break;
                    case EnemyType.SPEED:
                        SpawnEnemy(j, this.enemyPrefabSpeed);
                        break;
                    case EnemyType.TANK:
                        SpawnEnemy(j, this.enemyPrefabTank);
                        break;
                    case EnemyType.SPLITTER:
                        SpawnEnemy(j, this.enemyPrefabSplitter);
                        break;
                    case EnemyType.SUPPORT:
                        SpawnEnemy(j, this.enemyPrefabSupport);
                        break;
                    default:
                        SpawnEnemy(j, this.enemyPrefabStandard);
                        break;

                }
            }
        }

        if (waveNumber == 1)
        {
            for (int i = 0; i < this.gameManager.routes.Count; i++)
            {
                //SpawnEnemy1(i);
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
                //SpawnEnemy1(i);
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

    void SpawnEnemy(int routeIndex, Transform type)
    {

        //switch (type)
        //{
        //    case EnemyType.STANDARD:
        //        Transform prefab = enemyPrefabStandard;
        //        break;
        //    case EnemyType.SPEED:
        //        Transform prefab = enemyPrefabStandard;
        //        break;
        //    case EnemyType.TANK:
        //        Transform prefab = enemyPrefabStandard;
        //        break;
        //    case EnemyType.SPLITTER:
        //        Transform prefab = enemyPrefabStandard;
        //        break;
        //    case EnemyType.SUPPORT:
        //        Transform prefab = enemyPrefabStandard;
        //        break;
        //}

        List<GameObject> targets = this.gameManager.routes[routeIndex];
        Transform newEnemy = Instantiate(type, targets[0].transform.position, targets[0].transform.rotation);
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
