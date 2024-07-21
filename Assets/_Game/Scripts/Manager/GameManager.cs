using Unity.VisualScripting;
using UnityEngine;

public enum GameState { MainMenu, GamePlay, Finish, Setting, Revive }

public class GameManager : Singleton<GameManager>
{
    private static GameState gameState;
    public int AliveCount { get; private set; }

    private float totalCoin;
    public float TotalCoin => totalCoin;

    private int reviveTimer = 5;

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
        AliveCount = LevelManager.Instance.CurLevel.TotalEnemies + 1;
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

    public void UpdateAliveCountText() => UIManager.Instance.OpenUI<CanvasGamePlay>().UpdateAliveCountText(--AliveCount);

    private void CheckWinCondition()
    {
        if (EnemyManager.Instance.IsAllEnemiesDead && !LevelManager.Instance.Player.IsStatus(StatusType.Dead))
        {
            OnWin();
        }
    }

    public void OnWin()
    {
        ChangeState(GameState.Finish);
        UIManager.Instance.OpenUI<CanvasWin>();
        LevelManager.Instance.Player.OnWin();
    }

    public void OnLose()
    {
        ChangeState(GameState.Finish);
        UIManager.Instance.OpenUI<CanvasLose>();
    }

    public void OnPlay()
    {
        ChangeState(GameState.GamePlay);
        LevelManager.Instance.Player.OnPlay();
        EnemyManager.Instance.OnPlay();
        CameraFollower.Instance.SetupGamePlayMode();
    }

    public void OnPlayRevive()
    {
        StopReviveTimer();
        ChangeState(GameState.GamePlay);
        LevelManager.Instance.Player.OnRevive();
    }

    public void OnSetting()
    {
        ChangeState(GameState.Setting);
        EnemyManager.Instance.StopMovingAll();
        LevelManager.Instance.Player.StopMoving();
    }

    public void OnMenu()
    {
        ChangeState(GameState.MainMenu);
        LevelManager.Instance.Player.SetMainMenuPose();
        LevelManager.Instance.Player.ChangeToSavedItems();
        LevelManager.Instance.Player.ChangeToSavedWeapon();
        CameraFollower.Instance.SetupMenuMode();
    }

    public void OnRevive()
    {
        ChangeState(GameState.Revive);
        UIManager.Instance.OpenUI<CanvasRevive>().SetCounterText(reviveTimer);
        InvokeRepeating(nameof(CountReviveTimer), 1, 1);
    }

    public void CountReviveTimer()
    {
        UIManager.Instance.OpenUI<CanvasRevive>().SetCounterText(--reviveTimer);
        if(reviveTimer <= 0)
        {
            StopReviveTimer();
            OnLose();
        }
    }

    public void StopReviveTimer()
    {
        reviveTimer = 5;
        CancelInvoke(nameof(CountReviveTimer));
        UIManager.Instance.OpenUI<CanvasRevive>().Close(0);
    }
}

