using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    private Character owner;
    public Character Owner => owner;

    private Weapon weaponPrefab;

    private Weapon curHoldingWeapon;
    private WeaponType curType;

    private float speed;
    private float spinSpeed;
    private Vector3 rotateAxis;
    private bool isReturn;
    private bool isGrab;

    public Weapon WeaponPrefab => weaponPrefab;
    public float Speed => speed;
    public float SpinSpeed => spinSpeed;
    public Vector3 RotateAxis => rotateAxis;
    public bool IsReturn => isReturn;
    public bool IsGrab => isGrab;

    //----------------------- Bullet -----------------------
    public Bullet bulletPrefab;
    private Bullet curBullet;
    private int bulletCharge;

    public int BulletCharge => bulletCharge;
    public bool HasBullet => bulletCharge > 0;
    public Bullet CurBullet => curBullet;

    public Transform TF;

    public void OnInit(Character owner)
    {
        this.owner = owner;
        ChangeWeapon(owner.WeaponType);
        InitStats();
    }

    private void InitStats()
    {
        owner.BonusAttackRange = curHoldingWeapon.BonusAttackRange;
        bulletCharge = Const.WEAPON_BASE_BULLET_AMOUNT;

        speed = owner.BaseAttackSpeed + curHoldingWeapon.BonusSpeed;
        spinSpeed = curHoldingWeapon.SpinSpeed;
        rotateAxis = curHoldingWeapon.RotateAxis;
        isReturn = curHoldingWeapon.IsReturn;
        isGrab = curHoldingWeapon.IsGrab;
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

    public void OnShoot(Vector3 targetPos)
    {
        curBullet = Instantiate(bulletPrefab, TF.position, Quaternion.identity);
        curBullet.OnInit(this, targetPos);
        bulletCharge--;
    }

    public void Reload(int chargeNum)
    {
        bulletCharge += chargeNum;
        Owner.ToggleWeapon(true);
    }
}
