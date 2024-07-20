using OpenCover.Framework.Model;
using TMPro;
using UnityEngine;

public class CanvasMainMenu : UICanvas
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TMP_InputField nameInputField;

    public void OnOpen()
    {
        GameManager.ChangeState(GameState.MainMenu);
        SetCoinText(GameManager.Instance.TotalCoin);
        LevelManager.Instance.Player.SetMainMenuPose();
        LevelManager.Instance.Player.ChangeToSavedItems();
        LevelManager.Instance.Player.ChangeToSavedWeapon();
        CameraFollower.Instance.SetupMenuMode();
    }

    public void PlayButton()
    {
        if (CameraFollower.Instance.IsState(CameraState.Normal))
        {
            Close(0);
            UIManager.Instance.OpenUI<CanvasGamePlay>().UpdateLevelText(LevelManager.Instance.CurLevel.Index);
            UIManager.Instance.OpenUI<CanvasGamePlay>().UpdateAliveNumText(GameManager.Instance.CurAliveNum);
            GameManager.ChangeState(GameState.GamePlay);
            LevelManager.Instance.Player.OnPlay();
            CameraFollower.Instance.SetupGamePlayMode();
        }
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
        GameManager.ChangeState(GameState.Setting);
    }

    public void WeaponShopButton()
    {
        if (CameraFollower.Instance.IsState(CameraState.Normal))
        {
            Close(0);
            UIManager.Instance.OpenUI<CanvasWeaponShop>().OnOpen();
            CameraFollower.Instance.SetupWeaponShopMode();
        }
    }

    public void SkinShopButton()
    {
        if (CameraFollower.Instance.IsState(CameraState.Normal))
        {
            Close(0);
            UIManager.Instance.OpenUI<CanvasSkinShop>().OnOpen();
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
}
