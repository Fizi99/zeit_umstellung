using System.IO;
using System.Text;
using UnityEngine;

public class ExportBalancing : MonoBehaviour
{
    private GameManager gameManager;
    private string path;
    public string version = "1";

    private void Start()
    {
        this.gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        this.path = Application.dataPath + "/Balancing/" + "balancing_v" + this.version + ".md";
    }

    /* saved stats:
     * Player:
     *      float maxZeitsand
     *      float zeitsandStartValue
     *      float zeitsandRatePerSec
     * Wave:
     *      float timeBetweenWaves
     *      float timeBetweenEnemies
     *      float minSpawnDistance
     *      int initialWaveBudget
     *      int budgetIncrease
     *      int maxWaveBudget
     *      int maxRoutesUsedByEnemies
     * Tower:
     *      STANDARD
     *      MISSILE
     *      DRONE
     *      LASER
     *      FREEZE
     *      DYNAMITE
     *          float range
     *          isSingleUse
     *          float useAmount
     *          float buildingCost
     *          bool isMoving
     *          float speed
     *          float stopAndShootRange
     *          float turretEfficiency
     *          float fireRate
     *          float fireCountdown
     *          BULLET:
     *              float speed
     *              float damage
     *              float explosionRadius
     *              bool useFreeze
     *              bool shootSelf
     *              float SlowMultiplier
     *              float freezeDuration
     * Enemies:
     *      float speed
     *      float health
     *      bool isSplitter
     *      splitAmount
     *      int auraRadius
     *      float auraEffectStrength
     *      float auraDuration
     *      int cost
     *      int randomWeight
     *      int damage
    */
    public void ExportBalancingToFile()
    {

        StringBuilder md = new StringBuilder();
        md.AppendLine("# game balancing");
        md.AppendLine();

        md.AppendLine("## player stats");
        md.AppendLine("maxZeitsand: " + this.gameManager.player.maxZeitsand);
        md.AppendLine("zeitsandStartValue: " + this.gameManager.player.zeitsandStartValue);
        md.AppendLine("zeitsandRatePerSec: " + this.gameManager.player.zeitsandRatePerSec);
        md.AppendLine();

        md.AppendLine("## wave stats");
        md.AppendLine("timeBetweenWaves: " + this.gameManager.waveSpawner.timeBetweenWaves);
        md.AppendLine("timeBetweenEnemies: " + this.gameManager.waveSpawner.timeBetweenEnemies);
        md.AppendLine("minSpawnDistance: " + this.gameManager.waveSpawner.minSpawnDistance);
        md.AppendLine("initialWaveBudget: " + this.gameManager.waveSpawner.initialWaveBudget);
        md.AppendLine("budgetIncrease: " + this.gameManager.waveSpawner.budgetIncrease);
        md.AppendLine("maxWaveBudget: " + this.gameManager.waveSpawner.maxWaveBudget);
        md.AppendLine("maxRoutesUsedByEnemies: " + this.gameManager.waveSpawner.maxRoutesUsedByEnemies);
        md.AppendLine();

        md.AppendLine("## tower stats");

        foreach(GameObject turretPrefab in this.gameManager.turretPrefabs)
        {
            TurretAI turret = turretPrefab.GetComponent<TurretAI>();
            bulletAI bullet = turret.bulletPrefab.GetComponent<bulletAI>();
        
        
            md.AppendLine("### " + turret.name);
            md.AppendLine("range: " + turret.range);
            md.AppendLine("isSingleUse: " + turret.isSingleUse);
            md.AppendLine("useAmount: " + turret.useAmount);
            md.AppendLine("buildingCost: " + turret.buildingCost);
            md.AppendLine("isMoving: " + turret.isMoving);
            md.AppendLine("speed: " + turret.speed);
            md.AppendLine("stopAndShootRange: " + turret.stopAndShootRange);
  //          md.AppendLine("turretEfficiency: " + turret.getTurretEfficiency());
            md.AppendLine("fireRate: " + turret.fireRate);
            if(bullet != null)
            {
                md.AppendLine("#### bullet stats");
                md.AppendLine("speed: " + bullet.speed);
                md.AppendLine("damage: " + bullet.damage);
                md.AppendLine("explosionRadius: " + bullet.explosionRadius);
                md.AppendLine("useFreeze: " + bullet.useFreeze);
                md.AppendLine("shootSelf: " + bullet.shootSelf);
                md.AppendLine("SlowMultiplier: " + bullet.SlowMultiplier);
                md.AppendLine("freezeDuration: " + bullet.freezeDuration);
            }
            
            md.AppendLine();
        }
        md.AppendLine();
        foreach (GameObject enemyPrefab in this.gameManager.enemyPrefabs)
        {
            EnemyAI enemy = enemyPrefab.GetComponent<EnemyAI>();

            md.AppendLine("### " + enemy.name);
            md.AppendLine("speed: " + enemy.initSpeed);
            md.AppendLine("health: " + enemy.initHealth);
            md.AppendLine("isSplitter: " + enemy.isSplitter);
            md.AppendLine("splitAmount: " + enemy.splitAmount);
            md.AppendLine("auraRadius: " + enemy.auraRadius);
            md.AppendLine("auraEffectStrength: " + enemy.auraEffectStrength);
            md.AppendLine("auraDuration: " + enemy.auraDuration);
            md.AppendLine("cost: " + enemy.cost);
            md.AppendLine("randomWeight: " + enemy.randomWeight);
            md.AppendLine("damage: " + enemy.damage);
            md.AppendLine();
        }

        File.WriteAllText(this.path, md.ToString());
    }


}
