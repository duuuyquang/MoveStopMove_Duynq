using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

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

    public float speed;
    public float spinSpeed;

    public Bullet bulletPrefab;
    public Transform TF;

    public void OnInit(Character owner, int bulletNum)
    {
        this.owner = owner;
        bulletCharge = bulletNum;
        ChangeWeapon(owner.WeaponType);
    }

    private void ChangeWeapon(WeaponType type)
    {
        Debug.Log(curType);
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
        BulletCharge += chargeNum;
        Owner.ToggleWeapon(true);
    }
}
