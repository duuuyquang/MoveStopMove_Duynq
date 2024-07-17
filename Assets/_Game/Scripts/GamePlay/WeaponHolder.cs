using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    private Character owner;
    public Character Owner => owner;

    private Weapon curWeapon;
    public Weapon CurWeapon => curWeapon;

    private Bullet curBullet;
    public Bullet CurBullet => curBullet;

    private int bulletCharge;
    public int BulletCharge => bulletCharge;
    public bool HasBullet => bulletCharge > 0;

    public Transform TF;

    public void OnInit(Character owner)
    {
        this.owner = owner;
        ChangeWeapon(owner.WeaponType);
        InitStats();
    }

    private void InitStats()
    {
        owner.WeaponBonusAtkRange = curWeapon.BonusAttackRange;
        bulletCharge = Const.WEAPON_BASE_BULLET_AMOUNT;
    }

    public void ChangeWeapon(WeaponType type)
    {
        if(curWeapon != null)
        {
            Destroy(curWeapon.gameObject);
            //curHoldingWeapon.TF.SetParent(PoolControl.Instance.WeaponPoolTF);
            //WeaponPool.Despawn(curHoldingWeapon);
        }

        curWeapon = Instantiate(owner.itemDataSO.GetWeapon(type), TF);
        curWeapon.gameObject.SetActive(true);
        //curHoldingWeapon = WeaponPool.Spawn<Weapon>(curType, TF.position, Quaternion.identity);
        //curHoldingWeapon.TF.SetParent(TF, false);
        //curHoldingWeapon.OnInit();
    }

    public void OnShoot(Vector3 targetPos)
    {
        //curBullet = Instantiate(bulletPrefab, TF.position, Quaternion.identity);
        curBullet = SimplePool.Spawn<Bullet>(PoolType.Bullet, TF.position, Quaternion.identity);
        curBullet.OnInit(this, targetPos);
        bulletCharge--;
    }

    public void Reload(int chargeNum)
    {
        bulletCharge += chargeNum;
        Owner.ToggleWeapon(true);
    }
}
