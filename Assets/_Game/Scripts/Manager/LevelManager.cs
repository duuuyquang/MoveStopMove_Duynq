using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] List<Level> levels;
    [SerializeField] Player playerPrefab;
    public Level CurLevel { get; private set; }
    public Player Player { get; private set; }

    void Start()
    {
        OnInit();
    }

    public void OnInit(int level = 0)
    {
        PlayerData.LoadData();
        if (level == 0)
        {
            level = PlayerData.Instance.curLevel;
        }
        SimplePool.ReleaseAll();
        WeaponPool.ReleaseAll();
        InitPlayer();
        LoadLevel(level);
        GameManager.Instance.OnInit();
        EnemyManager.Instance.OnInit();
    }

    public void OnInitNextLevel()
    {
        OnInit(CurLevel.Index + 1);
    }

    public void OnInitCurLevel()
    {
        OnInit(CurLevel.Index);
    }

    public void InitPlayer()
    {
        if (Player)
        {
            Destroy(Player.gameObject);
        }
        Player = Instantiate(playerPrefab);
        Player.OnInit();
    }

    public void OnDespawn()
    {

    }

    void LoadLevel(int level)
    {
        if(CurLevel != null)
        {
            CurLevel.OnDespawn();
        }

        if (levels[level])
        {
            Level newLevel = Instantiate(levels[level], transform);
            newLevel.Index = level;
            CurLevel = newLevel;
        }
    }
}
