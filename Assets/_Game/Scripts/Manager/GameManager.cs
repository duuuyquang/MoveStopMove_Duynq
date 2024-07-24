using UnityEngine;

public enum GameState { MainMenu, GamePlay, Finish, Setting, Revive }

public class GameManager : Singleton<GameManager>
{
    private static GameState gameState;
    public int AliveCount  { get; private set; }
    public float TotalCoin { get; private set; }

    private int reviveCounter = Const.CHARACTER_REVIVE_COUNTDOWN_SECS;

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

    private void Update()
    {
        if (IsState(GameState.GamePlay))
        {
            CheckWinCondition();
        }
    }

    public void OnInit()
    {
        TotalCoin = PlayerData.Instance.totalCoin;
        AliveCount = LevelManager.Instance.CurLevel.TotalEnemies + 1;
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        UIManager.Instance.OpenUI<CanvasMainMenu>().SetNameText(LevelManager.Instance.Player.Name);
    }

    public void UpdateTotalCoin(float coin) {
        TotalCoin += Mathf.Max(coin, 0);
        PlayerData.Instance.totalCoin = TotalCoin;
        PlayerData.SaveData();
    }

    public void ReduceTotalCoin(float coin) {
        TotalCoin -= Mathf.Max(coin, 0);
        PlayerData.Instance.totalCoin = TotalCoin;
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
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasWin>().OnOpen();
        LevelManager.Instance.Player.OnWin();
    }

    public void OnLose()
    {
        ChangeState(GameState.Finish);
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasLose>().OnOpen();
    }

    public void OnLoseRevive()
    {
        ChangeState(GameState.Finish);
        StopReviveTimer();
    }

    public void OnPlayDelay(float delay, bool cameraMove = false)
    {
        Invoke(nameof(OnPlay), delay);
    }

    private void OnPlay()
    {
        ChangeState(GameState.GamePlay);
        LevelManager.Instance.Player.OnPlay();
        EnemyManager.Instance.OnPlay();
    }

    public void OnPlayRevive()
    {
        ChangeState(GameState.GamePlay);
        StopReviveTimer();
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
    }

    public void OnRevive()
    {
        ChangeState(GameState.Revive);
        UIManager.Instance.OpenUI<CanvasRevive>().SetCounterText(reviveCounter);
        InvokeRepeating(nameof(CountReviveTimer), 1, 1);
    }

    private void CountReviveTimer()
    {
        UIManager.Instance.OpenUI<CanvasRevive>().SetCounterText(--reviveCounter);
        if(reviveCounter <= 0)
        {
            StopReviveTimer();
            OnLose();
        }
    }

    private void StopReviveTimer()
    {
        reviveCounter = Const.CHARACTER_REVIVE_COUNTDOWN_SECS;
        CancelInvoke(nameof(CountReviveTimer));
        UIManager.Instance.OpenUI<CanvasRevive>().Close(0);
    }
}

