using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] int MaxEnemiesOnRunTime = 1;
    private int enemiesReadyCount;
    private NavMeshHit navMeshHit;
    public List<Enemy> SpawnedEnemiesList { get; private set; } = new();
    public bool IsAllEnemiesDead => (enemiesReadyCount <= 0 && SpawnedEnemiesList.Count <= 0);
    private bool IsSpawnable => SpawnedEnemiesList.Count < MaxEnemiesOnRunTime && enemiesReadyCount > 0;
    public int RecordHighestPoint { get; private set; }

    public readonly static string[] NAMES = { 
        "Dazzle", 
        "Crystal", 
        "Lina", 
        "Clink", 
        "Axe", 
        "Phantom", 
        "Sniper", 
        "TrollWarlord", 
        "BrewMaster", 
        "Hoodwink", 
        "Winranger", 
        "Traxex", 
        "Enchantress", 
        "Luna" 
    };


    public readonly static ItemType[] HEAD_ITEM_TYPES = {
        ItemType.None,
        ItemType.Head_Arrow,
        ItemType.Head_Beared,
        ItemType.Head_CowBoy,
        ItemType.Head_Crown,
        ItemType.Head_Ear,
        ItemType.Head_NormalHat,
        ItemType.Head_Cap,
        ItemType.Head_Yellow,
        ItemType.Head_Headphone,
    };

    public readonly static ItemType[] PANTS_ITEM_TYPES = {
        ItemType.None,
        ItemType.Pants_Batman,
        ItemType.Pants_ChamBi,
        ItemType.Pants_Comy,
        ItemType.Pants_Dabao,
        ItemType.Pants_Onion,
        ItemType.Pants_Pokemon,
        ItemType.Pants_Rainbow,
        ItemType.Pants_Skull,
        ItemType.Pants_Vantim,
    };

    public readonly static ItemType[] SHIELD_ITEM_TYPES = {
        ItemType.None,
        ItemType.Shield_CaptainAmerican,
        ItemType.Shield_Normal,
    };

    public readonly static ItemType[] SET_ITEM_TYPES = {
        ItemType.Set_1,
        ItemType.Set_2,
        ItemType.Set_3,
        ItemType.Set_4,
        ItemType.Set_5,
    };

    public void Update()
    {
        CheckToSpawn();
    }

    public void OnInit()
    {
        RecordHighestPoint = 0;
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
        SimplePool.Collect(PoolType.Enemy);
        SpawnedEnemiesList.Clear();
    }

    public void Spawn()
    {
        Enemy enemy = SimplePool.Spawn<Enemy>(PoolType.Enemy, GetValidSpawnPos(), Quaternion.identity);
        enemy.OnInit();
        if(GameManager.IsState(GameState.GamePlay))
        {
            enemy.OnPlay();
        }

        SpawnedEnemiesList.Add(enemy);
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

            foreach (Enemy enemy in SpawnedEnemiesList)
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
        SpawnedEnemiesList.Remove(enemy);
    }

    private void PreLoadEnemiesReadyList()
    {
        enemiesReadyCount = LevelManager.Instance.CurLevel.TotalEnemies;
    }

    public void SetRecordHighestPoint(int point)
    {
        if (point > RecordHighestPoint)
        {
            RecordHighestPoint = point;
        }
    }

    public void StopMovingAll()
    {
        foreach (Enemy enemy in SpawnedEnemiesList)
        {
            enemy.StopMoving();
        }
    }

    public void OnPlay()
    {
        foreach (Enemy enemy in SpawnedEnemiesList)
        {
            enemy.OnPlay();
        }
    }
}
