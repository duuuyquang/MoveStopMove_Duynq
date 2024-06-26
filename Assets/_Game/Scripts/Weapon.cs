using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    private GameObject weaponPrefab;
    private Character owner;
    public Bullet bulletPrefab;
    public float speed;
    public float spinAngle;
    private WeaponType type;

    private int bulletCharge;
    public int BulletCharge { get { return bulletCharge; } set { bulletCharge = Mathf.Max(0, value); } }

    public bool HasBullet => bulletCharge > 0;

    public GameObject WeaponPrefab {  get { return weaponPrefab; } }
    public Character Owner { get { return owner; } }

    public void OnInit(Character owner, int bulletNum)
    {
        this.owner = owner;
        this.type = owner.WeaponType;
        bulletCharge = bulletNum;
        CreatePrefab();
    }

    private void CreatePrefab()
    {
        GetWeaponByType();
        Instantiate(weaponPrefab, transform);
    }

    public void GetWeaponByType()
    {
        weaponPrefab = owner.itemDataSO.GetWeapon(owner.WeaponType);
    }

    public void Throw(Vector3 targetPos)
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.OnInit(this, targetPos);
    }

    public void Reload(int chargeNum)
    {
        BulletCharge += chargeNum;
    }
}
