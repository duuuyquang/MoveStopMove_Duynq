using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoosterManager : Singleton<BoosterManager>
{
    [SerializeField] int MaxBoostersOnRunTime = 2;
    [SerializeField] Booster prefab;
    public List<Booster> SpawnedBoostersList { get; private set; } = new();
    private NavMeshHit navMeshHit;

    private bool IsSpawnable => SpawnedBoostersList.Count < MaxBoostersOnRunTime;

    // Update is called once per frame
    void Update()
    {
        CheckToSpawn();
    }

    public void OnInit()
    {
        ClearAllLists();
    }

    private void ClearAllLists()
    {
        SimplePool.Collect(PoolType.Booster);
        SpawnedBoostersList.Clear();
    }

    private void CheckToSpawn()
    {
        if (GameManager.IsState(GameState.GamePlay) && IsSpawnable)
        {
            Spawn();
        }
    }

    public void Spawn()
    {
        Booster booster = SimplePool.Spawn<Booster>(PoolType.Booster, GetValidSpawnPos(), Quaternion.identity);
        booster.OnInit();

        SpawnedBoostersList.Add(booster);
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

            foreach (Booster booster in SpawnedBoostersList)
            {
                if (Vector3.Distance(navMeshHit.position, booster.TF.position) < 5f)
                {
                    return GetValidSpawnPos();
                }
            }

            foreach (Enemy enemy in EnemyManager.Instance.SpawnedEnemiesList)
            {
                if (Vector3.Distance(navMeshHit.position, enemy.TF.position) < 5f)
                {
                    return GetValidSpawnPos();
                }
            }
        }

        return new Vector3(navMeshHit.position.x, 0, navMeshHit.position.z);
    }

}
