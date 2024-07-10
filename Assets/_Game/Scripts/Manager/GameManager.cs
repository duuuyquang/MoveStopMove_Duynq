using UnityEngine;

public enum GameState { MainMenu, GamePlay, Finish, Revive, Setting }

public class GameManager : Singleton<GameManager>
{
    private static GameState gameState;

    private int curAliveNum;
    public int CurAliveNum => curAliveNum;

    private int totalCoin;
    public int TotalCoin => totalCoin;

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
        //TODO: get from player data later
        totalCoin = 0;
        UIManager.Instance.OpenUI<CanvasMainMenu>().SetCoin(totalCoin);
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
        curAliveNum = LevelManager.Instance.CurLevel.TotalEnemies + 1;
        CameraFollower.Instance.OnInit();
    }

    public void UpdateTotalCoin(int coin) => totalCoin += Mathf.Max(coin, 0);
    public static void ChangeState(GameState state) => gameState = state;
    public static bool IsState(GameState state) => gameState == state;
    public void UpdateAliveNumText() => UIManager.Instance.OpenUI<CanvasGamePlay>().UpdateAliveNumText(--curAliveNum);

    private void CheckWinCondition()
    {
        if (EnemyManager.Instance.IsAllEnemiesDead && !LevelManager.Instance.Player.IsStatus(StatusType.Dead))
        {
            ChangeState(GameState.Finish);
            UIManager.Instance.OpenUI<CanvasWin>();
        }
    }
}

