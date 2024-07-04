using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static Character;

public enum GameState { MainMenu, GamePlay, Finish, Revive, Setting }

public class GameManager : Singleton<GameManager>
{
    private static GameState gameState;

    private int curAliveNum;
    public int CurAliveNum { get { return curAliveNum; } }

    private int recordHighestPoint;
    public int RecordHighestPoint { get { return recordHighestPoint; } }

    public static void ChangeState(GameState state)
    {
        gameState = state;
    }

    public static bool IsState(GameState state) => gameState == state;

    private void Awake()
    {
        Input.multiTouchEnabled = false;
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        int maxScreenHeight = 1920;
        float ratio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
        if (Screen.currentResolution.height > maxScreenHeight)
        {
            Screen.SetResolution(Mathf.RoundToInt(ratio * (float)maxScreenHeight), maxScreenHeight, true);
        }
    }

    void Start()
    {
        UIManager.Instance.OpenUI<CanvasMainMenu>();
        UIManager.Instance.GetUI<CanvasGamePlay>().CloseDirectly();
        OnInit();
    }

    private void Update()
    {
        if (IsState(GameState.GamePlay))
        {
            CheckWinCondition();
        }
    }

    public void OnInit()
    {
        ChangeState(GameState.MainMenu);
        curAliveNum = LevelManager.Instance.CurLevel.totalNum + 1;
        recordHighestPoint = 0;
        CameraFollower.Instance.OnInit();
    }

    public void UpdateAliveNumText()
    {
        UIManager.Instance.OpenUI<CanvasGamePlay>().UpdateAliveNumText(--curAliveNum);
    }

    private void CheckWinCondition()
    {
        if (EnemyManager.Instance.IsAllEnemiesDead && !LevelManager.Instance.Player.IsStatus(StatusType.Dead))
        {
            ChangeState(GameState.Finish);
            UIManager.Instance.OpenUI<CanvasWin>();
        }
    }

    public void SetRecordHighestPoint(int point)
    {
        if(point > recordHighestPoint)
        {
            recordHighestPoint = point;
        }
    }
}

