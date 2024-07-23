using UnityEngine;

public class CanvasLose : UICanvas
{
    [SerializeField] Animator animator;

    public void OnOpen()
    {
        animator.SetTrigger(Const.ANIM_NAME_CANVAS_LOSE);
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        LevelManager.Instance.OnInit(PlayerData.Instance.curLevel);
    }

    public void ReviveButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasGamePlay>();
    }
}
