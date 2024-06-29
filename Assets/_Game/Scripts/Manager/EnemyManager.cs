using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : Singleton<EnemyManager>
{
    private const int MAX_ENEMY_ON_RUNTIME = 10;

    private Vector3 lastSpawnedPos;

    private NavMeshHit navMeshHit;

    [SerializeField] Enemy enemyPrefab;

    private List<Enemy> enemiesReadyList = new List<Enemy>();
    private List<Enemy> spawnedEnemiesList = new List<Enemy>();
    public bool IsAllEnemiesDead => (enemiesReadyList.Count <= 0 && spawnedEnemiesList.Count <= 0);

    public void Update()
    {
        if (GameManager.IsState(GameState.GamePlay))
        {
            if (spawnedEnemiesList.Count < MAX_ENEMY_ON_RUNTIME && enemiesReadyList.Count > 0)
            {
                Spawn();
            }
        }
    }

    public void OnInit()
    {
        ClearAllList();
        PreLoadEnemyList();
    }
    
    private void ClearAllList()
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
        spawnedEnemiesList.Add(enemy);
        enemiesReadyList.Remove(enemy);
        enemy.TF.position = GetValidSpawnPos();
        enemy.gameObject.SetActive(true);
    }

    public Vector3 GetValidSpawnPos()
    {
        float   playerAtkRange = LevelManager.Instance.Player.CurAttackRange;
        Vector3 playerPos = LevelManager.Instance.Player.TF.position;
        if (NavMesh.SamplePosition(playerPos + Random.insideUnitSphere * playerAtkRange * 2f, out navMeshHit, playerAtkRange * 5f, NavMesh.AllAreas))
        {
            if(Vector3.Distance(navMeshHit.position, playerPos) < playerAtkRange)
            {
                GetValidSpawnPos();
            }

            foreach(Enemy enemy in spawnedEnemiesList) {
                if (Vector3.Distance(navMeshHit.position, enemy.TF.position) < 5f)
                {
                    GetValidSpawnPos();
                }
            }
        }

        return navMeshHit.position;
    }

    public bool Despawn(Enemy enemy)
    {
        return spawnedEnemiesList.Remove(enemy);
    }

    private void PreLoadEnemyList()
    {
        for (int i = 0; i < LevelManager.Instance.CurLevel.totalNum; i++)
        {
            Enemy enemy = Instantiate(enemyPrefab, transform);
            enemy.gameObject.SetActive(false);
            enemiesReadyList.Add(enemy);
        }
    }
}
