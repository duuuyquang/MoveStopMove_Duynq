using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public enum GameState { MainMenu, GamePlay, Finish, Revive, Setting }

public class GameManager : Singleton<GameManager>
{
    [SerializeField] CameraFollower cameraFollower;

    private static GameState gameState;

    private int curAliveNum;
    public int CurAliveNum { get { return curAliveNum; } }

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
        cameraFollower.OnInit();
    }

    public void UpdateAliveNumText()
    {
        UIManager.Instance.OpenUI<CanvasGamePlay>().UpdateAliveNumText(--curAliveNum);
    }

    private void CheckWinCondition()
    {
        if (EnemyManager.Instance.IsAllEnemiesDead && !CameraFollower.Instance.Player.IsDead)
        {
            ChangeState(GameState.Finish);
            UIManager.Instance.OpenUI<CanvasWin>();
        }
    }
}

