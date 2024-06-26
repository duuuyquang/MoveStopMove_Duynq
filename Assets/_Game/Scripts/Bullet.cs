using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Transform bulletObj;
    private Vector3 targetPos;
    private Weapon weapon;

    private bool IsDestination => Vector3.Distance(TF.position, targetPos) < 0.1f;

    public Transform TF;

    void Update()
    {
        MovingToTarget();
    }

    private void MovingToTarget()
    {
        TF.position = Vector3.MoveTowards(TF.position, targetPos, weapon.speed * Time.deltaTime);
        if(weapon.spinAngle > 0f)
        {
            bulletObj.Rotate(Vector3.back, weapon.spinAngle);
        }
        if (IsDestination)
        {
            OnDespawn();
        }
    }

    public void OnInit(Weapon weapon, Vector3 pos)
    {
        this.weapon = weapon;
        targetPos = pos;
        CreateWeaponPrefab();
    }

    private void CreateWeaponPrefab()
    {
        GameObject weaponPrefab = Instantiate(weapon.WeaponPrefab, bulletObj);
        bulletObj.transform.LookAt(targetPos);
        // fix actual direction to the target
        bulletObj.transform.eulerAngles += new Vector3(90f, 0f, 0f);

        float ownerCurSize = weapon.Owner.CurSize;
        transform.localScale += Vector3.one * (ownerCurSize - 1f) * 0.5f;
    }

    public void OnDespawn()
    {
        weapon.Reload(1);
        weapon.Owner.ToggleWeapon(true);
        weapon.Owner.SetSizeBigger();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetChars(other);
        if (character)
        {
            weapon.Owner.RemoveTargetInRange(character);
            OnDespawn();
        }
    }
}
