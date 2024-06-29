using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] List<Level> levels;
    [SerializeField] Player player;

    public int initLevel;

    private Level curLevel;

    public Level CurLevel { get { return curLevel; } }
    public Player Player { get { return player; } }

    void Awake()
    {
        OnInit(initLevel);
    }

    public void OnInit(int level)
    {
        LoadLevel(level);
        player.OnInit();
        EnemyManager.Instance.OnInit();
        GameManager.Instance.OnInit();
    }

    public void OnInitNextLevel()
    {
        LoadLevel(curLevel.Index + 1);
        player.OnInit();
        EnemyManager.Instance.OnInit();
        GameManager.Instance.OnInit();
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
