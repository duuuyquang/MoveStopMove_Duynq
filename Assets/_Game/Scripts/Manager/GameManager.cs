using UnityEngine;

public enum GameState { MainMenu, SkinShop, GamePlay, Finish, Setting }

public class GameManager : Singleton<GameManager>
{
    private static GameState gameState;

    private int curAliveNum;
    public int CurAliveNum => curAliveNum;

    private float totalCoin;
    public float TotalCoin => totalCoin;

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
        //TODO: get from player data later
        totalCoin = 999;
        curAliveNum = LevelManager.Instance.CurLevel.TotalEnemies + 1;

        ChangeState(GameState.MainMenu);
        UIManager.Instance.GetUI<CanvasMainMenu>().SetCoin(totalCoin);
        UIManager.Instance.GetUI<CanvasMainMenu>().SetName(LevelManager.Instance.Player.Name);
        CameraFollower.Instance.OnInit();
    }

    public void UpdateTotalCoin(float coin) => totalCoin += Mathf.Max(coin, 0);
    public void ReduceTotalCoin(float coin) => totalCoin -= Mathf.Max(coin, 0);
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

