using System.Collections.Generic;
using UnityEngine;
using KaimiraGames;
using System;
using System.Linq;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefabSupport;
    public Transform enemyPrefabSpeed;
    public Transform enemyPrefabStandard;
    public Transform enemyPrefabSplitter;
    public Transform enemyPrefabTank;

    public GameObject enemyContainer;

    public float timeBetweenWaves = 5f;
    public float countdownWaves = 0f;
    public float timeBetweenEnemies = 3f;
    public float countdownEnemies = 0f;

    public float minSpawnDistance = 5f;

    public int amountOfRoutes = 0;

    public int initialWaveBudget = 100;
    [HideInInspector]
    public int waveBudget = 100;
    public int budgetIncrease = 10;
    private int budgetSpent = 0;
    public int maxRoutesUsedByEnemies = 3;
    private List<EnemyType> currentWaveEnemies = new List<EnemyType>();
    private Dictionary<int, Queue<EnemyType>> wave = new Dictionary<int, Queue<EnemyType>>();
    private WeightedList<EnemyType> enemyWeightList = new WeightedList<EnemyType>();
    private List<EnemyType> enemyTypesFromEnum = new List<EnemyType>();
    private float waveHealth = 0;
    private int waveCount = 1;

    [Header("1=minimum zeitSandBuff needed to win wave")]
    public float zeitSandBuff = 2f;

    public Transform[] targetList;

    private GameManager gameManager;

    void Start()
    {
        //amountOfRoutes = routeManager.routeList.Length;
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.waveBudget = this.initialWaveBudget;
        InitEnemyTypes();
    }

    // Update is called once per frame
    void Update()
    {
        // countdown between waves
        if (this.wave.Count <= 0 && this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            if (this.countdownWaves <= 0)
            {
                InitWave();
                this.countdownWaves = this.timeBetweenWaves;
            }
            else
            {
                this.countdownWaves -= Time.deltaTime;
            }


        }

        // countdown between single enemies
        if (this.gameManager.gameState == GameState.LEVELPLAYING)
        {
            if (countdownEnemies <= 0)
            {
                SpawnWave();
                this.waveCount++;
                this.countdownEnemies = this.timeBetweenEnemies;
            }
            else
            {
                this.countdownEnemies -= Time.deltaTime;
            }
        }
    }

    // create a weighted list for enemy type. this list is later used to init waves
    private void InitEnemyTypes()
    {
        this.enemyWeightList = new WeightedList<EnemyType>();
        this.enemyTypesFromEnum = Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().ToList();

        foreach (EnemyType enemy in this.enemyTypesFromEnum)
        {
            switch (enemy)
            {
                case EnemyType.STANDARD:
                    this.enemyWeightList.Add(EnemyType.STANDARD, enemyPrefabStandard.GetComponent<EnemyAI>().randomWeight);
                    break;
                case EnemyType.SPEED:
                    this.enemyWeightList.Add(EnemyType.SPEED, enemyPrefabSpeed.GetComponent<EnemyAI>().randomWeight);
                    break;
                case EnemyType.TANK:
                    this.enemyWeightList.Add(EnemyType.TANK, enemyPrefabTank.GetComponent<EnemyAI>().randomWeight);
                    break;
                case EnemyType.SPLITTER:
                    this.enemyWeightList.Add(EnemyType.SPLITTER, enemyPrefabSplitter.GetComponent<EnemyAI>().randomWeight);
                    break;
                case EnemyType.SUPPORT:
                    this.enemyWeightList.Add(EnemyType.SUPPORT, enemyPrefabSupport.GetComponent<EnemyAI>().randomWeight);
                    break;
                default:
                    this.enemyWeightList.Add(EnemyType.STANDARD, enemyPrefabStandard.GetComponent<EnemyAI>().randomWeight);
                    break;
            }
        }
    }

    // init a wave: choose random enemys to spawn depending on weight and budget, and place those enemies on routes
    private void InitWave()
    {
        
        // randomly choose enemies to spawn depending on wave budget
        this.currentWaveEnemies = new List<EnemyType>();

        while (this.budgetSpent < this.waveBudget)
        {
            // add enemies depending on weight to list
            EnemyType i = this.enemyWeightList.Next();

            switch (i)
            {
                case EnemyType.STANDARD:
                    this.currentWaveEnemies.Add(EnemyType.STANDARD);
                    this.budgetSpent += enemyPrefabStandard.GetComponent<EnemyAI>().cost;
                    this.waveHealth += enemyPrefabStandard.GetComponent<EnemyAI>().initHealth;
                    break;
                case EnemyType.SPEED:
                    this.currentWaveEnemies.Add(EnemyType.SPEED);
                    this.budgetSpent += enemyPrefabSpeed.GetComponent<EnemyAI>().cost;
                    this.waveHealth += enemyPrefabSpeed.GetComponent<EnemyAI>().initHealth;
                    break;
                case EnemyType.TANK:
                    this.currentWaveEnemies.Add(EnemyType.TANK);
                    this.budgetSpent += enemyPrefabTank.GetComponent<EnemyAI>().cost;
                    this.waveHealth += enemyPrefabTank.GetComponent<EnemyAI>().initHealth;
                    break;
                case EnemyType.SPLITTER:
                    this.currentWaveEnemies.Add(EnemyType.SPLITTER);
                    this.budgetSpent += enemyPrefabSplitter.GetComponent<EnemyAI>().cost;
                    this.waveHealth += enemyPrefabSplitter.GetComponent<EnemyAI>().initHealth;
                    break;
                case EnemyType.SUPPORT:
                    this.currentWaveEnemies.Add(EnemyType.SUPPORT);
                    this.budgetSpent += enemyPrefabSupport.GetComponent<EnemyAI>().cost;
                    this.waveHealth += enemyPrefabSupport.GetComponent<EnemyAI>().initHealth;
                    break;
                default:
                    this.currentWaveEnemies.Add(EnemyType.STANDARD);
                    this.budgetSpent += enemyPrefabStandard.GetComponent<EnemyAI>().cost;
                    this.waveHealth += enemyPrefabStandard.GetComponent<EnemyAI>().initHealth;
                    break;
            }
        }
        float neededZeitsand =this.waveHealth * this.gameManager.buildManager.getLoadoutEfficiency();
        float neededZeitsandRate = zeitSandBuff * neededZeitsand/(5 + this.currentWaveEnemies.Count*2);
        //this.zeitSandBuff = 1 + (1 / waveCount);
        Debug.Log(waveCount);


        //this.gameManager.player.setZeitsandRatePerSec(neededZeitsandRate);
        this.waveHealth = 0;
        // randomly choose route for each enemy 
        this.wave = new Dictionary<int, Queue<EnemyType>>();


        // get eligable routes for spawning (so enemies dont spawn too close to busstop
        List<int> eligableRoutes = new List<int>();
        for(int i = 0; i < this.gameManager.routes.Count; i++)
        {
            if((Vector3.zero - this.gameManager.routes[i][0].transform.position).magnitude > minSpawnDistance)
            {
                eligableRoutes.Add(i);
      
            }
        }

        // choose how many routes enemys can walk on. If there are less possible routes on map than max route amounte, use list length as max.
        // int maxRoutes = Mathf.Min(this.maxRoutesUsedByEnemies, this.gameManager.routes.Count);
        int maxRoutes = Mathf.Min(this.maxRoutesUsedByEnemies, eligableRoutes.Count);
        // init spawn queues for routes
        // save possible indices of routes in list, to access them randomly. also save those indices, so enemies can be assigned to dictionary positions
        List<int> routeIndices = new List<int>();
        List<int> usedRouteIndices = new List<int>();
        for (int i = 0; i < eligableRoutes.Count; i++)
        {
            routeIndices.Add(eligableRoutes[i]);
        }
        for (int i = 0; i < maxRoutes; i++)
        {
            // randomly choose a route index and set it as possible spawn
            int j = UnityEngine.Random.Range(0, routeIndices.Count);
            this.wave.Add(routeIndices[j], new Queue<EnemyType>());
            // remove that index from possible indices, so value isnt overwritten
            usedRouteIndices.Add(routeIndices[j]);
            routeIndices.RemoveAt(j);


        }
        // distribute enemies to routes randomly
        foreach (EnemyType enemy in this.currentWaveEnemies)
        {
            int j = UnityEngine.Random.Range(0, usedRouteIndices.Count);
            this.wave[usedRouteIndices[j]].Enqueue(enemy);
        }


        this.budgetSpent = 0;
        this.waveBudget += this.budgetIncrease;
    }

    void SpawnWave()
    {

        
        // delete elements from dictionary dynamically
        List<int> toDelete = new List<int>();

        // loop through all routes and spawn next enemie of given route
        foreach (KeyValuePair<int, Queue<EnemyType>> currentWave in this.wave)
        {
            if (currentWave.Value.Count > 0)
            {
                switch (currentWave.Value.Peek())
                //switch(EnemyType.SUPPORT)
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
        foreach (int i in toDelete)
        {
            this.wave.Remove(i);
        }

    }

    public void setAmountOfRoutes(int amountOfRoutes)
    {
        this.amountOfRoutes = amountOfRoutes;
    }

    void SpawnEnemy(int routeIndex, Transform type)
    {
        if(this.gameManager.routes.Count > 0)
        {
            List<GameObject> targets = this.gameManager.routes[routeIndex];
            Transform newEnemy = Instantiate(type, targets[0].transform.position, targets[0].transform.rotation);
            newEnemy.GetComponent<EnemyAI>().targets = targets;
            newEnemy.GetComponent<EnemyAI>().currentTarget = newEnemy.GetComponent<EnemyAI>().targets[1];
            newEnemy.transform.parent = enemyContainer.transform;
        }
        


    }


}
