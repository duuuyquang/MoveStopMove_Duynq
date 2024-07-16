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
    private int curID = 1;

    public ItemDataSO itemDataSO;

    public void Start()
    {
        DisplayData(curID);
    }

    public void SetCoin(float coin)
    {
        coinText.text = coin.ToString();
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>();
        LevelManager.Instance.Player.ChangeWeaponHolderMesh(LevelManager.Instance.Player.WeaponType);
        CameraFollower.Instance.SetupMenuMode();

    }

    public void NextWeapon()
    {
        curID = Mathf.Min(++curID, itemDataSO.TotalWeapons-1);
        DisplayData(curID);
    }

    public void PreviousWeapon()
    {
        curID = Mathf.Max(--curID, 1);
        DisplayData(curID);
    }

    public void Purchase()
    {
        if(GameManager.Instance.TotalCoin >= curWeapon.Price)
        {
            GameManager.Instance.ReduceTotalCoin(curWeapon.Price);
            LevelManager.Instance.Player.UpdateOwnedWeapon(curWeapon.WeaponType);
            UIManager.Instance.GetUI<CanvasMainMenu>().SetCoin(GameManager.Instance.TotalCoin);
            SetCoin(GameManager.Instance.TotalCoin);
            DisplayData(curID);
        }
    }

    public void Select()
    {
        LevelManager.Instance.Player.ChangeWeapon(curWeapon.WeaponType);
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>();
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

        LevelManager.Instance.Player.ChangeWeaponHolderMesh((WeaponType)curID);

        curWeapon = itemDataSO.GetWeapon((WeaponType)curID);
        weaponName.text = curWeapon.name;
        DisplayStatsText();
        if (LevelManager.Instance.Player.WeaponType == curWeapon.WeaponType)
        {
            buttons[(int)WeaponState.IsEquipped].gameObject.SetActive(true);
        }
        else if(LevelManager.Instance.Player.IsOwnedWeapon(curWeapon.WeaponType))
        {
            buttons[(int)WeaponState.IsSelect].gameObject.SetActive(true);
        }
        else if (GameManager.Instance.TotalCoin >= curWeapon.Price)
        {
            buttons[(int)WeaponState.IsPurchasable].gameObject.SetActive(true);
            priceTexts[(int)WeaponState.IsPurchasable].text = curWeapon.Price.ToString();
        }
        else
        {
            buttons[(int)WeaponState.IsNotPurchasable].gameObject.SetActive(true);
            priceTexts[(int)WeaponState.IsNotPurchasable].text = curWeapon.Price.ToString();
        }
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
