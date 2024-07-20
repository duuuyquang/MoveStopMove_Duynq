using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] List<Level> levels;
    [SerializeField] Player playerPrefab;

    private Player player;

    public int initLevel;

    private Level curLevel;
    public Level CurLevel => curLevel;
    public Player Player => player;

    void Start()
    {
        OnInit(initLevel);
    }

    public void OnInit(int level)
    {
        SimplePool.ReleaseAll();
        WeaponPool.ReleaseAll();
        InitPlayer();
        LoadLevel(level);
        GameManager.Instance.OnInit();
        EnemyManager.Instance.OnInit();
    }

    public void OnInitNextLevel()
    {
        SimplePool.ReleaseAll();
        WeaponPool.ReleaseAll();
        InitPlayer();
        LoadLevel(curLevel.Index + 1);
        GameManager.Instance.OnInit();
        EnemyManager.Instance.OnInit();
    }

    public void InitPlayer()
    {
        if (player)
        {
            Destroy(player.gameObject);
        }
        player = Instantiate(playerPrefab);

        //if (player == null) {
        //    player = Instantiate(playerPrefab);
        //}
        player.OnInit();
    }

    public void OnDespawn()
    {

    }

    void LoadLevel(int level)
    {
        if(curLevel != null)
        {
            curLevel.OnDespawn();
        }

        if (levels[level])
        {
            Level newLevel = Instantiate(levels[level], transform);
            newLevel.Index = level;
            curLevel = newLevel;
        }
    }
}
