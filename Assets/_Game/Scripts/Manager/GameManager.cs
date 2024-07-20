using Unity.VisualScripting;
using UnityEngine;

public enum GameState { MainMenu, GamePlay, Finish, Setting }

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
        totalCoin = PlayerData.Instance.totalCoin;
        curAliveNum = LevelManager.Instance.CurLevel.TotalEnemies + 1;
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        UIManager.Instance.OpenUI<CanvasMainMenu>().SetNameText(LevelManager.Instance.Player.Name);
    }

    public void UpdateTotalCoin(float coin) {
        totalCoin += Mathf.Max(coin, 0);
        PlayerData.Instance.totalCoin = totalCoin;
        PlayerData.SaveData();
    }
    public void ReduceTotalCoin(float coin) {
        totalCoin -= Mathf.Max(coin, 0);
        PlayerData.Instance.totalCoin = totalCoin;
        PlayerData.SaveData();
    }
    public static void ChangeState(GameState state) => gameState = state;
    public static bool IsState(GameState state) => gameState == state;
    public void UpdateAliveNumText() => UIManager.Instance.OpenUI<CanvasGamePlay>().UpdateAliveNumText(--curAliveNum);

    private void CheckWinCondition()
    {
        if (EnemyManager.Instance.IsAllEnemiesDead && !LevelManager.Instance.Player.IsStatus(StatusType.Dead))
        {
            ChangeState(GameState.Finish);
            UIManager.Instance.OpenUI<CanvasWin>();
            OnWin();
        }
    }

    public void OnWin()
    {
        //TODO: move to other manager class
        //if (GameManager.IsState(GameState.Finish) && IsStatus(StatusType.Normal))
        //{
        //    ChangeStatus(StatusType.Win);
        //    OnWin();
        //}
    }

    public void OnPlay()
    {

    }

    public void OnLose()
    {
        UIManager.Instance.OpenUI<CanvasLose>();
    }
}

