using UnityEngine;
using UnityEngine.UI;

public class CanvasSettings : UICanvas
{
    [SerializeField] GameObject[] buttons;
    [SerializeField] Animator animator;

    [SerializeField] Image[] volumeImgs;
    [SerializeField] Slider volume;

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

        volume.value = SoundManager.Instance.Volume;
        UpdateVolumeIcon();
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        LevelManager.Instance.OnInit(PlayerData.Instance.curLevel);
        SoundManager.Instance.PlayBtnClick();
    }

    public override void Close(float time)
    {
        base.Close(time);
        animator.SetTrigger(Const.ANIM_NAME_CANVAS_SETTING_OUT);
        SoundManager.Instance.PlayBtnClick();
    }

    public override void CloseDirectly()
    {
        base.CloseDirectly();
    }

    public void ContinueButton()
    {
        Close(1f);
        GameManager.Instance.OnPlayDelay(1f);
        SoundManager.Instance.PlayBtnClick();
    }

    public void ChangeVolume(float value)
    {
        volume.value = value;
        SoundManager.Instance.SetVolume(value);
        UpdateVolumeIcon();
    }

    private void UpdateVolumeIcon()
    {
        foreach (Image volumeImg in volumeImgs)
        {
            volumeImg.gameObject.SetActive(false);
        }

        if (volume.value <= 0f)
        {
            volumeImgs[0].gameObject.SetActive(true);
        }
        else
        {
            volumeImgs[1].gameObject.SetActive(true);
        }
    }
}
