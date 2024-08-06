using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasMainMenu : UICanvas
{
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TMP_Dropdown levelsDropdown;
    [SerializeField] Animator animator;

    private List<string> levelOptions = new();

    public void OnOpen()
    {
        SetCoinText(GameManager.Instance.TotalCoin);
        GameManager.Instance.OnMenu();
        CameraFollower.Instance.SetupMenuMode();
        animator.SetTrigger(Const.ANIM_NAME_CANVAS_MENU_IN);
        GetLevelOptions();
    }

    public void PlayButton()
    {
        if (CameraFollower.Instance.IsState(CameraState.Normal))
        {
            Close(1f);
            animator.SetTrigger(Const.ANIM_NAME_CANVAS_MENU_OUT);
            UIManager.Instance.OpenUI<CanvasGamePlay>().OnOpen();
            GameManager.Instance.OnPlayDelay(1f);
            CameraFollower.Instance.SetupGamePlayMode();
            SoundManager.Instance.PlayBtnClick();
        }
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
        GameManager.Instance.OnSetting();
        SoundManager.Instance.PlayBtnClick();
    }

    public void WeaponShopButton()
    {
        if (CameraFollower.Instance.IsState(CameraState.Normal))
        {
            Close(0);
            UIManager.Instance.OpenUI<CanvasWeaponShop>().OnOpen();
            SoundManager.Instance.PlayBtnClick();
        }
    }

    public void SkinShopButton()
    {
        if (CameraFollower.Instance.IsState(CameraState.Normal))
        {
            Close(0);
            UIManager.Instance.OpenUI<CanvasSkinShop>().OnOpen();
            SoundManager.Instance.PlayBtnClick();
        }
    }

    public void SetCoinText(float coin)
    {
        coinText.text = coin.ToString();
    }

    public void SetNameText(string text)
    {
        nameInputField.text = text;
        LevelManager.Instance.Player.Name = text;
        PlayerData.Instance.name = text;
        PlayerData.SaveData();
    }

    public void SetLevel(int level)
    {
        LevelManager.Instance.SelectingLevel = level;
        LevelManager.Instance.OnInit(level);
        SoundManager.Instance.PlayBtnClick();
    }

    private void GetLevelOptions()
    {
        levelsDropdown.ClearOptions();
        levelOptions.Clear();
        for( int i = 1; i <= LevelManager.Instance.TotalLevel; i++ )
        {
            if( i <= PlayerData.Instance.curLevel )
            {
                levelOptions.Add($"Level {i}");
            }
        }

        levelsDropdown.AddOptions(levelOptions);
        levelsDropdown.SetValueWithoutNotify(LevelManager.Instance.SelectingLevel);
    }
}
