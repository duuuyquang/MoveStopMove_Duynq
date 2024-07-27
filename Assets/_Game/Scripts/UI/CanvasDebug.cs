using TMPro;
using UnityEngine;

public class CanvasDebug : UICanvas
{
    [SerializeField] TextMeshProUGUI logText;
    string content = "";

    string atkSpd;
    string baseAtkSpd;
    string baseMoveSpd;
    string weaponBonusAtkSpd;

    string atkRange;
    string baseAtkRange;
    string wpBonusAtkRange;
    string itemBonusAtkRangeMulti;

    string size;
    string moveSpd;
    string bonusGoldMulti;

    string boosterType;
    string curStatus;
    public void Update()
    {
        SetLogText();
    }

    public void SetLogText()
    {
        moveSpd                = LevelManager.Instance.Player.MoveSpeed.ToString();
        baseMoveSpd            = LevelManager.Instance.Player.BaseMoveSpeed.ToString();
        bonusGoldMulti         = ( LevelManager.Instance.Player.BonusGoldMultiplier).ToString();
        size                   = LevelManager.Instance.Player.CurSize.ToString();
        curStatus              = LevelManager.Instance.Player.CurStatus.ToString();
                               
        atkSpd                 = LevelManager.Instance.Player.WeaponHolder.Speed.ToString();
        baseAtkSpd             = LevelManager.Instance.Player.BaseAtkSpeed.ToString();
        weaponBonusAtkSpd      = LevelManager.Instance.Player.WeaponHolder.CurWeapon.BonusSpeed.ToString();
                               
        atkRange               = LevelManager.Instance.Player.CurAttackRange.ToString();
        baseAtkRange           = LevelManager.Instance.Player.BaseAtkRange.ToString();
        wpBonusAtkRange        = LevelManager.Instance.Player.WeaponHolder.CurWeapon.BonusAttackRange.ToString();
        itemBonusAtkRangeMulti = LevelManager.Instance.Player.ItemBonusAtkRangeMultiplier.ToString();

        boosterType            = LevelManager.Instance.Player.BoosterType.ToString();

        SetContent();

        logText.text = content;
    }

    private void SetContent()
    {
        content = "";
        content += $"Stt:{curStatus} - ";
        content += $"Size:{size} - ";
        content += $"Booster:{boosterType}\n";
        content += $"BonusGold:{bonusGoldMulti} \n";
        content += $"MoveSpd:{moveSpd} = [{baseMoveSpd} + Booster] * Size\n";
        content += $"AtkSpd:{atkSpd} = [{baseAtkSpd} + {weaponBonusAtkSpd}(w)] * Size\n";
        content += $"AtkRange:{atkRange} = [ {baseAtkRange} + {wpBonusAtkRange}(w) + booster] * {itemBonusAtkRangeMulti}(i) * Size\n";
    }
}
