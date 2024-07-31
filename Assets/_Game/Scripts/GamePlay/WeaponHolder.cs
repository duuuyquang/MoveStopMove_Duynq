using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    public Character Owner  { get; private set; }
    public Weapon CurWeapon { get; private set; }
    public Bullet CurBullet { get; private set; }
    public int BulletCharge { get; private set; }
    public bool HasBullet => BulletCharge > 0;

    public float Speed => (Owner.BaseAtkSpeed + CurWeapon.BonusSpeed) * Owner.CurSize;

    public Transform TF;

    public void OnInit(Character owner)
    {
        CurBullet = null;
        this.Owner = owner;
        ChangeWeapon(owner.WeaponType);
        InitStats();
    }

    private void InitStats()
    {
        BulletCharge = Const.WEAPON_BASE_BULLET_AMOUNT;
    }

    public void ChangeWeapon(WeaponType type)
    {
        if(CurWeapon != null)
        {
            Destroy(CurWeapon.gameObject);
        }

        CurWeapon = Instantiate(Owner.itemDataSO.GetWeapon(type), TF);
        CurWeapon.gameObject.SetActive(true);
    }

    public void OnShoot(Vector3 targetPos)
    {
        CurBullet = SimplePool.Spawn<Bullet>(PoolType.Bullet, TF.position, Quaternion.identity);
        CurBullet.OnInit(this, targetPos);
        BulletCharge--;
        if(CurWeapon.IsGrab)
        {
            Invoke(nameof(ReloadBase), Const.WEAPON_DROP_TIME);
        }
    }

    public void Reload(int chargeNum)
    {
        BulletCharge += chargeNum;
        Owner.ToggleWeapon(true);
    }

    public void ReloadBase()
    {
        BulletCharge = Const.WEAPON_BASE_BULLET_AMOUNT;
        Owner.ToggleWeapon(true);
    }

    public void CancelReloadBase()
    {
        CancelInvoke(nameof(ReloadBase));
    }

}
