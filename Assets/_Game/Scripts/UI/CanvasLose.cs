using TMPro;
using UnityEngine;

public class CanvasLose : UICanvas
{
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI killInfoText;

    public void OnOpen()
    {
        animator.SetTrigger(Const.ANIM_NAME_CANVAS_LOSE);
        SetKillerText(LevelManager.Instance.Player.Killer.Name);
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

    public void SetKillerText(string enemyName)
    {
        killInfoText.text = $"#{enemyName} killed you.";
    }
}
