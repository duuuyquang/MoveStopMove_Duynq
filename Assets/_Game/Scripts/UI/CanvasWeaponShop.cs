using TMPro;
using UnityEngine;

public enum WeaponState { IsNotPurchasable, IsPurchasable, IsSelect, IsEquipped };

public class CanvasWeaponShop : UICanvas
{
    [SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] TextMeshProUGUI weaponName;
    [SerializeField] TextMeshProUGUI weaponStats;
    [SerializeField] Transform[] buttons;
    [SerializeField] TextMeshProUGUI[] priceTexts;

    private Weapon curWeapon;
    private int curID;

    public ItemDataSO itemDataSO;

    public void OnOpen()
    {
        SetCoinText(GameManager.Instance.TotalCoin);
        DisplayData((int)LevelManager.Instance.Player.WeaponType);
        CameraFollower.Instance.SetupWeaponShopMode();
    }

    public void SetCoinText(float coin)
    {
        coinText.text = coin.ToString();
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        SoundManager.Instance.PlayBtnClick();
    }

    public void NextWeapon()
    {
        curID = Mathf.Min( ++curID, itemDataSO.TotalWeapons - 1);
        DisplayData(curID);
        SoundManager.Instance.PlayBtnClick();
    }

    public void PreviousWeapon()
    {
        curID = Mathf.Max(--curID, 1);
        DisplayData(curID);
        SoundManager.Instance.PlayBtnClick();
    }

    public void Purchase()
    {
        if(GameManager.Instance.TotalCoin >= curWeapon.Price)
        {
            GameManager.Instance.ReduceTotalCoin(curWeapon.Price);
            UpdateWeaponBought(curWeapon.WeaponType);
            SetCoinText(GameManager.Instance.TotalCoin);
            DisplayData(curID);
            SoundManager.Instance.PlayBtnClick();
        }
        else
        {
            SoundManager.Instance.PlayBtnClickError();
        }
    }

    public void Select()
    {
        LevelManager.Instance.Player.ChangeWeapon(curWeapon.WeaponType);
        PlayerData.Instance.weaponType = curWeapon.WeaponType;
        PlayerData.SaveData();

        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().SetCoinText(GameManager.Instance.TotalCoin);
        CameraFollower.Instance.SetupMenuMode();
    }

    public void DisplayData(int id)
    {
        curID = id;
        for (int i = 0; i < buttons.Length; i++) {
            buttons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < priceTexts.Length; i++)
        {
            priceTexts[i].text = "";
        }

        LevelManager.Instance.Player.WeaponHolder.ChangeWeapon((WeaponType)curID);

        curWeapon = itemDataSO.GetWeapon((WeaponType)curID);
        weaponName.text = curWeapon.name;
        DisplayStatsText();
        if (IsHoldingWeapon(curWeapon.WeaponType))
        {
            buttons[3].gameObject.SetActive(true);
        }
        else if(IsWeaponBought(curWeapon.WeaponType))
        {
            buttons[2].gameObject.SetActive(true);
        }
        else if (GameManager.Instance.TotalCoin >= curWeapon.Price)
        {
            buttons[1].gameObject.SetActive(true);
            priceTexts[1].text = curWeapon.Price.ToString();
        }
        else
        {
            buttons[0].gameObject.SetActive(true);
            priceTexts[0].text = curWeapon.Price.ToString();
        }
    }

    private bool IsHoldingWeapon(WeaponType type) => PlayerData.Instance.weaponType == type;
    private bool IsWeaponBought(WeaponType type) => PlayerData.Instance.weaponsState[type] == ItemState.Bought;
    private void UpdateWeaponBought(WeaponType type)
    {
        PlayerData.Instance.weaponsState[type] = ItemState.Bought;
        PlayerData.SaveData();
    }

    private void DisplayStatsText()
    {
        weaponStats.text = "";
        if (curWeapon.BonusSpeed > 0)
        {
            weaponStats.text += $"+{curWeapon.BonusSpeed.ToString()} speed \n";
        }
        if (curWeapon.BonusAttackRange > 0)
        {
            weaponStats.text += $"+{curWeapon.BonusAttackRange.ToString()}  attack range";
        }
    }
}
