using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] int MaxEnemiesOnRunTime = 10;
    [SerializeField] Enemy enemyPrefab;
    
    private NavMeshHit navMeshHit;

    private List<Enemy> enemiesReadyList = new List<Enemy>();
    private List<Enemy> spawnedEnemiesList = new List<Enemy>();

    private int enemiesReadyCount;
    //public bool IsAllEnemiesDead => (enemiesReadyList.Count <= 0 && spawnedEnemiesList.Count <= 0);
    public bool IsAllEnemiesDead => (enemiesReadyCount <= 0 && spawnedEnemiesList.Count <= 0);
    //public bool IsSpawnable => spawnedEnemiesList.Count < MaxEnemiesOnRunTime && enemiesReadyList.Count > 0;
    public bool IsSpawnable => spawnedEnemiesList.Count < MaxEnemiesOnRunTime && enemiesReadyCount > 0;

    private int recordHighestPoint;
    public int RecordHighestPoint { get { return recordHighestPoint; } }

    public void Update()
    {
        CheckToSpawn();
    }

    public void OnInit()
    {
        recordHighestPoint = 0;
        ClearAllLists();
        PreLoadEnemiesReadyList();
        InitSpawn();
    }

    private void InitSpawn()
    {
        for (int i = 0; i < MaxEnemiesOnRunTime; i++)
        {
            Spawn();
        }
    }

    private void CheckToSpawn()
    {
        if (GameManager.IsState(GameState.GamePlay) && IsSpawnable)
        {
            Spawn();
        }
    }
    
    private void ClearAllLists()
    {
        //while (enemiesReadyList.Count > 0)
        //{
        //    Enemy enemy = enemiesReadyList[0];
        //    enemiesReadyList.Remove(enemy);
        //    Destroy(enemy.gameObject);
        //}
        //enemiesReadyList.Clear();

        //while (spawnedEnemiesList.Count > 0)
        //{
        //    Enemy enemy = spawnedEnemiesList[0];
        //    spawnedEnemiesList.Remove(enemy);
        //    SimplePool.Despawn(enemy);
        //}
        SimplePool.Collect(PoolType.Enemy);
        spawnedEnemiesList.Clear();
    }

    public void Spawn()
    {
        //    Enemy enemy = enemiesReadyList[enemiesReadyList.Count - 1];
        //    enemy.OnInit();
        //    enemy.TF.position = GetValidSpawnPos();
        //    enemy.gameObject.SetActive(true);

        //    spawnedEnemiesList.Add(enemy);
        //    enemiesReadyList.Remove(enemy);

        Enemy enemy = SimplePool.Spawn<Enemy>(PoolType.Enemy, GetValidSpawnPos(), Quaternion.identity);
        enemy.OnInit();

        spawnedEnemiesList.Add(enemy);
        enemiesReadyCount--;
    }

    public Vector3 GetValidSpawnPos()
    {
        float playerAtkRange = LevelManager.Instance.Player.CurAttackRange;
        float multiDist = 3f;
        Vector3 playerPos = LevelManager.Instance.Player.TF.position;
        Vector3 randDirection = new Vector3(Random.Range(-playerAtkRange, playerAtkRange), 0f, Random.Range(-playerAtkRange, playerAtkRange)) * multiDist;
        if (NavMesh.SamplePosition(Vector3.zero + randDirection, out navMeshHit, playerAtkRange * multiDist, NavMesh.AllAreas))
        {
            if (Vector3.Distance(navMeshHit.position, playerPos) < playerAtkRange * multiDist * 0.5f)
            {
                return GetValidSpawnPos();
            }

            foreach (Enemy enemy in spawnedEnemiesList)
            {
                if (Vector3.Distance(navMeshHit.position, enemy.TF.position) < 5f)
                {
                     return GetValidSpawnPos();
                }
            }
        }

        return navMeshHit.position;
    }

    public void Despawn(Enemy enemy)
    {
        SimplePool.Despawn(enemy);
        spawnedEnemiesList.Remove(enemy);
    }

    private void PreLoadEnemiesReadyList()
    {
        enemiesReadyCount = LevelManager.Instance.CurLevel.TotalEnemies;
        //for (int i = 0; i < LevelManager.Instance.CurLevel.TotalEnemies; i++)
        //{
        //    Enemy enemy = Instantiate(enemyPrefab, transform);
        //    enemy.gameObject.SetActive(false);
        //    enemiesReadyList.Add(enemy);
        //}
    }

    public void SetRecordHighestPoint(int point)
    {
        if (point > recordHighestPoint)
        {
            recordHighestPoint = point;
        }
    }
}
