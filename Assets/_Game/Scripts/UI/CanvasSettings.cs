using UnityEngine;

public class CanvasSettings : UICanvas
{
    [SerializeField] GameObject[] buttons;
    [SerializeField] Animator animator;

    public void SetState(UICanvas canvas)
    {
        animator.SetTrigger(Const.ANIM_NAME_CANVAS_SETTING_IN);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }

        if (canvas is CanvasMainMenu)
        {
            buttons[2].gameObject.SetActive(true);
        }
        else if (canvas is CanvasGamePlay)
        {
            buttons[0].gameObject.SetActive(true);
            buttons[1].gameObject.SetActive(true);
        }
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        LevelManager.Instance.OnInit(PlayerData.Instance.curLevel);
    }

    public override void Close(float time)
    {
        base.Close(time);
        animator.SetTrigger(Const.ANIM_NAME_CANVAS_SETTING_OUT);
    }

    public override void CloseDirectly()
    {
        base.CloseDirectly();
    }

    public void ContinueButton()
    {
        Close(1f);
        GameManager.Instance.OnPlayDelay(1f);
    }
}
