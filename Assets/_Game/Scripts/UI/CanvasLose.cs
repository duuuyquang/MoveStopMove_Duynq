using UnityEngine;

public class CanvasLose : UICanvas
{
    [SerializeField] Animator animator;

    public void OnOpen()
    {
        animator.SetTrigger(Const.ANIM_NAME_CANVAS_LOSE);
        SoundManager.Instance.PlayLose();
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        LevelManager.Instance.OnInit(PlayerData.Instance.curLevel);
        SoundManager.Instance.PlayBtnClick();
    }

    public void ReviveButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasGamePlay>();
        SoundManager.Instance.PlayBtnClick();
    }
}
