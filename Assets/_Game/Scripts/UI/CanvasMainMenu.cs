public class CanvasMainMenu : UICanvas
{
    public void PlayButton()
    {
        Close(0);
        UIManager.Instance.OpenUI<CanvasGamePlay>().UpdateLevelText(LevelManager.Instance.CurLevel.Index);
        UIManager.Instance.OpenUI<CanvasGamePlay>().UpdateAliveNumText(GameManager.Instance.CurAliveNum);
        GameManager.ChangeState(GameState.GamePlay);
        CameraFollower.Instance.SetupGamePlayMode();
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
        GameManager.ChangeState(GameState.Setting);
    }

    public void ShopButton()
    {
        Close(0);
        UIManager.Instance.OpenUI<CanvasWeaponShop>();
    }
}
