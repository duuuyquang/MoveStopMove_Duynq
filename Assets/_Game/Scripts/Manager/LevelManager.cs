using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] List<Level> levels;
    [SerializeField] Player playerPrefab;
    public Level CurLevel { get; private set; }
    public Player Player { get; private set; }

    public int TotalLevel => levels.Count;

    public int SelectingLevel {get; set;}

    void Start()
    {
        Debug.Log(PlayerData.FileDirectory);
        OnInit();
    }

    public void OnInit(int level = -1)
    {
        PlayerData.LoadData();
        if (level == -1)
        {
            level = PlayerData.Instance.curLevel;
        }
        SelectingLevel = level;
        SimplePool.ReleaseAll();
        WeaponPool.ReleaseAll();
        OnLoad(level);
        OnInitPlayer();
        GameManager.Instance.OnInit();
        EnemyManager.Instance.OnInit();
        BoosterManager.Instance.OnInit();
    }

    public void OnInitNextLevel()
    {
        OnInit(CurLevel.Index + 1);
    }

    public void OnInitCurLevel()
    {
        OnInit(CurLevel.Index);
    }

    public void OnInitPlayer()
    {
        if (Player)
        {
            Destroy(Player.gameObject);
        }
        Player = Instantiate(playerPrefab);
        Player.OnInit();
    }

    void OnLoad(int level)
    {
        if(CurLevel != null)
        {
            CurLevel.OnDespawn();
        }

        level = Mathf.Min(levels.Count - 1, level);

        Level newLevel = Instantiate(levels[level], transform);
        newLevel.Index = level;
        CurLevel = newLevel;
    }
}
