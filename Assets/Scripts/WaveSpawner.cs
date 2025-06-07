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
    private List<EnemyType> currentWaveEnemies = new List<EnemyType>();
    private Dictionary<int, Queue<EnemyType>> wave = new Dictionary<int, Queue<EnemyType>>();

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
        if(this.wave.Count <= 0 && this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            InitWave();
        }

        if(countdown <=0 && this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            SpawnWave();
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime;
    }

    private void InitWave()
    {

        // randomly choose enemies to spawn depending on wave budget
        this.currentWaveEnemies = new List<EnemyType>();

        while (this.budgetSpent < this.waveBudget)
        {
            // currently adds enemies randomly to wave
            int i = Random.Range(1, 5);

            switch (i)
            {
                case 1:
                    this.currentWaveEnemies.Add(EnemyType.STANDARD);
                    this.budgetSpent += enemyPrefabStandard.GetComponent<EnemyAI>().cost;
                    break;
                case 2:
                    this.currentWaveEnemies.Add(EnemyType.SPEED);
                    this.budgetSpent += enemyPrefabSpeed.GetComponent<EnemyAI>().cost;
                    break;
                case 3:
                    this.currentWaveEnemies.Add(EnemyType.TANK);
                    this.budgetSpent += enemyPrefabTank.GetComponent<EnemyAI>().cost;
                    break;
                case 4:
                    this.currentWaveEnemies.Add(EnemyType.SPLITTER);
                    this.budgetSpent += enemyPrefabSplitter.GetComponent<EnemyAI>().cost;
                    break;
                case 5:
                    this.currentWaveEnemies.Add(EnemyType.SUPPORT);
                    this.budgetSpent += enemyPrefabSupport.GetComponent<EnemyAI>().cost;
                    break;
                default:
                    this.currentWaveEnemies.Add(EnemyType.STANDARD);
                    this.budgetSpent += enemyPrefabStandard.GetComponent<EnemyAI>().cost;
                    break;
            }
        }

        // randomly choose route for each enemy 
        //////////////////// CHANGE HERE IF WE WANT TO REDUCE ROUTE AMOUNT/////////////////////////
        this.wave = new Dictionary<int, Queue<EnemyType>>();
        for(int i = 0; i < this.gameManager.routes.Count; i++)
        {
            this.wave.Add(i, new Queue<EnemyType>());
        }
        // distribute enemies to routes randomly
        foreach (EnemyType enemy in this.currentWaveEnemies)
        {
            int j = Random.Range(0, this.gameManager.routes.Count - 1);
            this.wave[j].Enqueue(enemy);
        }
        //////////////////// CHANGE HERE IF WE WANT TO REDUCE ROUTE AMOUNT/////////////////////////

        this.budgetSpent = 0;
    }

    void SpawnWave()
    {
        // delete elements from dictionary dynamically
        List<int> toDelete = new List<int>();
        
        // loop through all routes and spawn next enemie of given route
        foreach(KeyValuePair<int, Queue<EnemyType>> currentWave in this.wave)
        {
            if(currentWave.Value.Count > 0)
            {
                switch(currentWave.Value.Peek())
                {
                    case EnemyType.STANDARD:
                        SpawnEnemy(currentWave.Key, this.enemyPrefabStandard);
                        break;
                    case EnemyType.SPEED:
                        SpawnEnemy(currentWave.Key, this.enemyPrefabSpeed);
                        break;
                    case EnemyType.TANK:
                        SpawnEnemy(currentWave.Key, this.enemyPrefabTank);
                        break;
                    case EnemyType.SPLITTER:
                        SpawnEnemy(currentWave.Key, this.enemyPrefabSplitter);
                        break;
                    case EnemyType.SUPPORT:
                        SpawnEnemy(currentWave.Key, this.enemyPrefabSupport);
                        break;
                    default:
                        SpawnEnemy(currentWave.Key, this.enemyPrefabStandard);
                        break;

                }

                currentWave.Value.Dequeue();
            }
            else
            {
                // later remove that key value pair
                toDelete.Add(currentWave.Key);
            }
            
        }

        // Remove routes that spawned all enemies
        foreach(int i in toDelete)
        {
            this.wave.Remove(i);
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
