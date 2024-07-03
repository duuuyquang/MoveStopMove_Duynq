using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Character owner;
    public Character Owner { get { return owner; } }

    private Transform   curHoldingWeapon;
    private WeaponType  curType;

    private Transform weaponPrefab;
    public Transform WeaponPrefab { get { return weaponPrefab; } }

    private int bulletCharge;
    public int BulletCharge { get { return bulletCharge; } set { bulletCharge = Mathf.Max(0, value); } }
    public bool HasBullet => bulletCharge > 0;

    private float speed;
    public float Speed { get { return speed; } }

    private float spinSpeed;
    public float SpinSpeed { get { return spinSpeed; } }

    public Bullet bulletPrefab;
    public Transform TF;

    public void OnInit(Character owner)
    {
        this.owner = owner;
        ChangeWeapon(owner.WeaponType);
        InitStats();
    }

    private void InitStats()
    {
        speed = owner.BaseAttackSpeed;
        spinSpeed = 10f;
        bulletCharge = Const.WEAPON_BASE_BULLET_AMOUNT;
    }

    private void ChangeWeapon(WeaponType type)
    {
        if (curType != type)
        {
            curType = type;
            weaponPrefab = owner.itemDataSO.GetWeapon(type);
            if(curHoldingWeapon)
            {
                Destroy(curHoldingWeapon.gameObject);
            }
            curHoldingWeapon = Instantiate(weaponPrefab, TF);
        }
    }

    public void Throw(Vector3 targetPos)
    {
        Bullet bullet = Instantiate(bulletPrefab, TF.position, Quaternion.identity);
        bullet.OnInit(this, targetPos);
    }

    public void Reload(int chargeNum)
    {
        bulletCharge += chargeNum;
        Owner.ToggleWeapon(true);
    }
}
