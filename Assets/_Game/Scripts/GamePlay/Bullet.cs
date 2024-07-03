using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 targetPos;
    private Weapon weapon;
    public Weapon Weapon { get { return weapon; } }

    private Transform weaponPrefab;

    private float speed;
    private float spinSpeed;

    private bool IsDestination => Vector3.Distance(TF.position + Vector3.up * (TF.position.y - targetPos.y), targetPos) < 0.1f;

    public Transform TF;

    void Update()
    {
        MovingToTarget();
    }

    public void OnInit(Weapon weapon, Vector3 targetPos)
    {
        this.weapon = weapon;
        this.targetPos = targetPos;
        InitSize();
        InitStats();
        CreateWeaponPrefab();
    }

    private void InitSize()
    {
        TF.localScale = Vector3.one * weapon.TF.localScale.x;
    }

    private void InitStats()
    {
        speed = weapon.Speed;
        spinSpeed = weapon.SpinSpeed;
    }

    private void CreateWeaponPrefab()
    {
        weaponPrefab = Instantiate(weapon.WeaponPrefab, TF);
        TF.LookAt(targetPos);

        // modify actual direction to the target
        TF.eulerAngles += new Vector3(90f, 0f, 0f);
        TF.localScale += Vector3.one * (weapon.Owner.CurSize - 1f) * 0.5f;
    }

    private void MovingToTarget()
    {
        TF.position = Vector3.MoveTowards(TF.position, targetPos, speed * Time.deltaTime);
        if (spinSpeed > 0f)
        {
            weaponPrefab.Rotate(Vector3.right, spinSpeed * Time.deltaTime * 100f);
        }

        if (IsDestination)
        {
            OnDespawn();
        }
    }

    public void OnDespawn()
    {
        if(weapon)
        {
            weapon.Reload(Const.WEAPON_BASE_BULLET_AMOUNT);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character && character == weapon.Owner)
        {
            return;
        }

        OnDespawn();
    }
}
