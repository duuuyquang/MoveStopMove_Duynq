using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Character;

public class Bullet : MonoBehaviour
{
    //[SerializeField] Collider bulletCollider;


    //------------------- Transform specs -------------------------
    private float speed;
    private float spinSpeed;
    private Vector3 rotateAxis;

    private Vector3 targetPos;
    private Weapon weapon;
    public Weapon Weapon { get { return weapon; } }

    private Transform weaponPrefab;
    private bool IsDestination => Vector3.Distance(TF.position + Vector3.up * (TF.position.y - targetPos.y), targetPos) < 0.1f;

    public Transform TF;

    //------------------- For Weapon returning back ---------------
    private bool isReturning = false;

    //------------------- For Weapon need to grab -----------------
    private bool isGrab = false;
    private bool isDropped = false;
    private float grabTimer = 0;
    public bool IsDropped { get { return isDropped; } }

    void Update()
    {
        ProcessBehavior();
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
        rotateAxis = weapon.RotateAxis;
        isGrab = weapon.IsGrab;
    }

    private void CreateWeaponPrefab()
    {
        weaponPrefab = Instantiate(weapon.WeaponPrefab, TF);

        TF.LookAt(targetPos);

        // modify actual direction to the target
        TF.eulerAngles += new Vector3(90f, 0f, 0f);

        //scale to current size
        TF.localScale += Vector3.one * (weapon.Owner.CurSize - 1f) * 0.5f;
    }

    private void ProcessBehavior()
    {
        MovingToTarget();
        if (IsDestination)
        {
            if(!ProcessForSpecicalCases())
            {
                OnDespawn();
            }
        }
    }

    private bool ProcessForSpecicalCases()
    {
        bool isSpecial = false;
        isSpecial |= ProcessReturnWeapon();
        isSpecial |= ProcessGrabWeapon();
        return isSpecial;
    }

    private bool ProcessReturnWeapon()
    {
        if (weapon && weapon.IsReturn && !isReturning)
        {
            SetTargetPos(weapon.Owner.TF.position);
            isReturning = true;
            return true;
        }
        return false;
    }

    private bool ProcessGrabWeapon()
    {
        if (isGrab)
        {
            if (grabTimer == 0)
            {
                //bulletCollider.enabled = false;
                isDropped = true;
                StopMoving();
                SetDroppedShape();
            }

            grabTimer += Time.deltaTime;

            if (!weapon || weapon && (Vector3.Distance(weapon.Owner.TF.position, TF.position) <= 1.5f || weapon.Owner.IsStatus(StatusType.Dead)) || grabTimer >= 5f)
            {
                OnDespawn();
            }
            return true;
        }
        return false;
    }

    private void MovingToTarget()
    {
        TF.position = Vector3.MoveTowards(TF.position, targetPos, speed * Time.deltaTime);
        weaponPrefab.Rotate(rotateAxis, spinSpeed * Time.deltaTime * 100f);
    }

    public void OnDespawn()
    {
        if(weapon)
        {
            weapon.Reload(Const.WEAPON_BASE_BULLET_AMOUNT);
        }
        Destroy(gameObject);
    }

    public void StopMoving()
    {
        spinSpeed = 0f;
        speed = 0f;
    }

    private void SetDroppedShape()
    {
        weaponPrefab.localPosition = new Vector3(0, 0, 1);
        weaponPrefab.localEulerAngles = new Vector3(-50, 180, 0);
    }

    private void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character && character == weapon.Owner)
        {
            return;
        }

        if (isGrab)
        {
            StopMoving();
            SetTargetPos(TF.position);
            return;
        }

        OnDespawn();
    }
}
