using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;

public class EnemyManager : Singleton<EnemyManager>
{
    private const int MAX_ENEMIES_ON_RUNTIME = 10;

    private Vector3 lastSpawnedPos;
    
    private NavMeshHit navMeshHit;

    [SerializeField] Enemy enemyPrefab;
    [SerializeField] GameObject test;
    [SerializeField] GameObject test2;

    private List<Enemy> enemiesReadyList = new List<Enemy>();
    private List<Enemy> spawnedEnemiesList = new List<Enemy>();
    public bool IsAllEnemiesDead => (enemiesReadyList.Count <= 0 && spawnedEnemiesList.Count <= 0);
    public bool IsSpawnable => spawnedEnemiesList.Count < MAX_ENEMIES_ON_RUNTIME && enemiesReadyList.Count > 0;

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
        for (int i = 0; i < MAX_ENEMIES_ON_RUNTIME; i++)
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
        while (enemiesReadyList.Count > 0)
        {
            Enemy enemy = enemiesReadyList[0];
            enemiesReadyList.Remove(enemy);
            Destroy(enemy.gameObject);
        }
        enemiesReadyList.Clear();

        while (spawnedEnemiesList.Count > 0)
        {
            Enemy enemy = spawnedEnemiesList[0];
            spawnedEnemiesList.Remove(enemy);
            Destroy(enemy.gameObject);
        }
        spawnedEnemiesList.Clear();
    }

    public void Spawn()
    {
        Enemy enemy = enemiesReadyList[enemiesReadyList.Count - 1];
        enemy.OnInit();
        enemy.TF.position = GetValidSpawnPos();
        enemy.gameObject.SetActive(true);

        spawnedEnemiesList.Add(enemy);
        enemiesReadyList.Remove(enemy);
    }

    public Vector3 GetValidSpawnPos()
    {
        float playerAtkRange = LevelManager.Instance.Player.CurAttackRange;
        float multiDist = 3f;
        Vector3 playerPos = LevelManager.Instance.Player.TF.position;
        Vector3 randDirection = new Vector3(Random.Range(-playerAtkRange, playerAtkRange), 0f, Random.Range(-playerAtkRange, playerAtkRange)) * multiDist;
        if (NavMesh.SamplePosition(playerPos + randDirection, out navMeshHit, playerAtkRange * multiDist, NavMesh.AllAreas))
        {
            if (Vector3.Distance(navMeshHit.position, playerPos) < playerAtkRange * multiDist * 0.5f)
            {
                GetValidSpawnPos();
            }
            else
            {
                foreach (Enemy enemy in spawnedEnemiesList)
                {
                    if (Vector3.Distance(navMeshHit.position, enemy.TF.position) < playerAtkRange)
                    {
                        GetValidSpawnPos();
                    }
                }
            }
        }

        return navMeshHit.position;
    }

    public bool Despawn(Enemy enemy)
    {
        return spawnedEnemiesList.Remove(enemy);
    }

    private void PreLoadEnemiesReadyList()
    {
        for (int i = 0; i < LevelManager.Instance.CurLevel.totalNum; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab, transform);
            enemy.gameObject.SetActive(false);
            enemiesReadyList.Add(enemy);
        }
    }

    public void SetRecordHighestPoint(int point)
    {
        if (point > recordHighestPoint)
        {
            recordHighestPoint = point;
        }
    }
}
