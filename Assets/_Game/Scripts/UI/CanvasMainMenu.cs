using TMPro;
using UnityEngine;

public class CanvasMainMenu : UICanvas
{
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TMP_InputField nameInputField;

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
            UIManager.Instance.OpenUI<CanvasWeaponShop>().DisplayData((int)LevelManager.Instance.Player.WeaponType);
            CameraFollower.Instance.SetupWeaponShopMode();
        }
    }

    public void SkinShopButton()
    {
        if (CameraFollower.Instance.IsState(CameraState.Normal))
        {
            Close(0);
            UIManager.Instance.OpenUI<CanvasSkinShop>();
            UIManager.Instance.OpenUI<CanvasSkinShop>().CachedPlayerItems();
            CameraFollower.Instance.SetupSkinShopMode();
        }
    }

    public void SetCoin(float coin)
    {
        coinText.text = coin.ToString();
    }

    public void SetName(string text)
    {
        nameInputField.text = text;
    }

    public void UpdatePlayerName()
    {
        LevelManager.Instance.Player.Name = nameInputField.text;
    }
}
