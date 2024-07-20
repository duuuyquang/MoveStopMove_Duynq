public class CanvasLose : UICanvas
{
    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        LevelManager.Instance.OnInit(PlayerData.Instance.curLevel);
    }

    //TODO: add revive functionality
    public void ReviveButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasGamePlay>();
    }
}
