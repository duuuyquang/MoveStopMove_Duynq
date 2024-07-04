using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Weapon : MonoBehaviour
{
    private Character owner;
    public Character Owner { get { return owner; } }

    private Transform   curHoldingWeapon;
    private WeaponType  curType;

    private Transform weaponPrefab;
    public Transform WeaponPrefab { get { return weaponPrefab; } }

    //--------------- Movement Specs ------------------------

    private float speed;
    public float Speed { get { return speed; } }

    private float spinSpeed;
    public float SpinSpeed { get { return spinSpeed; } }

    private Vector3 rotateAxis;
    public Vector3 RotateAxis { get { return rotateAxis; } }

    private bool isReturn;
    public bool IsReturn { get { return isReturn; } }

    private bool isGrab;
    public bool IsGrab { get { return isGrab; } }

    //--------------- Bullet -------------------------------
    private int bulletCharge;
    public int BulletCharge { get { return bulletCharge; } set { bulletCharge = Mathf.Max(0, value); } }
    public bool HasBullet => bulletCharge > 0;

    public Bullet bulletPrefab;
    private Bullet curBullet;
    public Bullet CurBullet { get { return curBullet; } }
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
        owner.BonusAttackRange = 0;
        spinSpeed = 20f;
        rotateAxis = Vector3.right;
        isReturn = false;
        isGrab = false;
        switch (owner.WeaponType)
        {
            case WeaponType.Lollipop:
                spinSpeed = 8f;
                owner.BonusAttackRange += 3f;
                break;
            case WeaponType.Bloom:
                spinSpeed = 0f;
                owner.BonusAttackRange += 1f;
                break;
            case WeaponType.Boomerang:
                spinSpeed = 30f;
                rotateAxis = Vector3.forward;
                isReturn = true;
                break;
            case WeaponType.Umbrella:
                spinSpeed = 0f;
                speed += 2f;
                break;
            case WeaponType.Mace:
                spinSpeed = 2f;
                speed += 2f;
                break;
            case WeaponType.Hammer:
                spinSpeed = 1f;
                speed += 1f;
                break;
            case WeaponType.Axe:
                isGrab = true;
                speed += 1f;
                owner.BonusAttackRange += 3f;
                break;
        }
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

    public void OnShoot(Vector3 targetPos)
    {
        curBullet = Instantiate(bulletPrefab, TF.position, Quaternion.identity);
        curBullet.OnInit(this, targetPos);
        BulletCharge--;
    }

    public void Reload(int chargeNum)
    {
        bulletCharge += chargeNum;
        Owner.ToggleWeapon(true);
    }
}
