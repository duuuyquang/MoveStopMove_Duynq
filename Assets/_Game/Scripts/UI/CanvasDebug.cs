using TMPro;
using UnityEngine;

public class CanvasDebug : UICanvas
{
    [SerializeField] TextMeshProUGUI logText;
    string content = "";

    string atkSpd;
    string baseAtkSpd;
    string weaponBonusAtkSpd;

    string atkRange;
    string baseAtkRange;
    string wpBonusAtkRange;
    string itemBonusAtkRange;

    string size;
    string moveSpd;
    string bonusGoldMulti;

    string curStatus;
    public void Update()
    {
        SetLogText();
    }

    public void SetLogText()
    {
        moveSpd             = LevelManager.Instance.Player.MoveSpeed.ToString();
        bonusGoldMulti      = ( LevelManager.Instance.Player.BonusGoldMultiplier * 0.01f ).ToString();
        size                = LevelManager.Instance.Player.CurSize.ToString();
        curStatus           = LevelManager.Instance.Player.CurStatus.ToString();

        atkSpd              = (LevelManager.Instance.Player.BaseAtkSpeed + LevelManager.Instance.Player.WeaponHolder.CurWeapon.BonusSpeed).ToString();
        baseAtkSpd          = LevelManager.Instance.Player.BaseAtkSpeed.ToString();
        weaponBonusAtkSpd   = LevelManager.Instance.Player.WeaponHolder.CurWeapon.BonusSpeed.ToString();

        atkRange            = LevelManager.Instance.Player.CurAttackRange.ToString();
        baseAtkRange        = LevelManager.Instance.Player.BaseAtkRange.ToString();
        wpBonusAtkRange     = LevelManager.Instance.Player.WeaponHolder.CurWeapon.BonusAttackRange.ToString();
        itemBonusAtkRange   = LevelManager.Instance.Player.ItemBonusAtkRange.ToString();

        SetContent();

        logText.text = content;
    }

    private void SetContent()
    {
        content = "";
        content += $"Stt: {curStatus} \n";
        content += $"Size: {size} \n";
        content += $"MoveSpd: {moveSpd} \n";
        content += $"BonusGoldMul: {bonusGoldMulti} \n";
        content += $"AtkSpd: {atkSpd} = {baseAtkSpd} + {weaponBonusAtkSpd}(w) \n";
        content += $"AtkRange: {atkRange} = [ {baseAtkRange} + {itemBonusAtkRange}(i) + {wpBonusAtkRange}(w) ] * Size \n";
    }
}
