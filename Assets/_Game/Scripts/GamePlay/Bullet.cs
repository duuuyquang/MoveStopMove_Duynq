using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Bullet : GameUnit
{
    [SerializeField] ParticleSystem dropPartical;

    //------------------- Transform specs -------------------------
    private float speed;
    private float spinSpeed;
    private Vector3 rotateAxis;
    private Vector3 targetPos;

    private WeaponHolder weaponHolder;
    public WeaponHolder WeaponHolder => weaponHolder;

    private Weapon weaponPrefab;

    private bool IsDestination => Vector3.Distance(TF.position + Vector3.up * (TF.position.y - targetPos.y), targetPos) < 0.1f;

    //------------------- For Weapon returning back ---------------
    private bool isReturning = false;

    //------------------- For Weapon need to grab -----------------
    private float grabTimer = 0f;
    private bool isGrab = false;
    private bool isDropped = false;
    public bool IsDropped => isDropped;

    void Update()
    {
        ProcessBehaviours();
    }

    public void OnInit(WeaponHolder weaponHolder, Vector3 targetPos)
    {
        this.weaponHolder = weaponHolder;
        this.targetPos = targetPos;

        InitSize();
        InitStats();
        InitSpecialStats();
        SpawnWeapon();
    }

    private void InitSize()
    {
        TF.localScale = Vector3.one * weaponHolder.TF.localScale.x;
    }

    private void InitStats()
    {
        speed = weaponHolder.Owner.BaseAttackSpeed + weaponHolder.CurWeapon.BonusSpeed;
        spinSpeed = weaponHolder.CurWeapon.SpinSpeed;
        rotateAxis = weaponHolder.CurWeapon.RotateAxis;
        isGrab = weaponHolder.CurWeapon.IsGrab;
    }

    private void InitSpecialStats()
    {
        isDropped = false;
        isReturning = false;
        grabTimer = 0f;
    }

    private void SpawnWeapon()
    {
        //weaponPrefab = Instantiate(weapon.WeaponPrefab, TF);
        weaponPrefab = WeaponPool.Spawn<Weapon>(WeaponHolder.Owner.WeaponType, TF.position, Quaternion.identity);
        weaponPrefab.TF.SetParent(TF, false);
        weaponPrefab.OnInit();

        TF.LookAt(targetPos);

        // modify actual direction to the target
        TF.eulerAngles += new Vector3(90f, 0f, 0f);

        //scale to current size
        TF.localScale += Vector3.one * (weaponHolder.Owner.CurSize - 1f) * 0.5f;
    }

    private void ProcessBehaviours()
    {
        MovingToTarget();
        if (IsDestination)
        {
            if(!ProcessForSpecificBehaviours())
            {
                OnDespawn();
            }
        }
    }

    private bool ProcessForSpecificBehaviours()
    {
        bool isTriggered = false;
        isTriggered |= ProcessReturnWeapon();
        isTriggered |= ProcessGrabWeapon();
        return isTriggered;
    }

    private bool ProcessReturnWeapon()
    {
        if (weaponHolder && weaponHolder.CurWeapon.IsReturn && !isReturning)
        {
            SetTargetPos(weaponHolder.Owner.TF.position);
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
                dropPartical.Play();
                isDropped = true;
                StopMoving();
                SetDroppedShape();
            }

            grabTimer += Time.deltaTime;

            if (!weaponHolder || weaponHolder && (Vector3.Distance(weaponHolder.Owner.TF.position, TF.position) <= 1.5f) || grabTimer >= 5f)
            {
                dropPartical.Stop();
                OnDespawn();
            }
            return true;
        }
        return false;
    }

    private void MovingToTarget()
    {
        TF.position = Vector3.MoveTowards(TF.position, targetPos, speed * Time.deltaTime);
        weaponPrefab.TF.Rotate(rotateAxis, spinSpeed * Time.deltaTime * 100f);
    }

    public void OnDespawn()
    {
        if(weaponHolder)
        {
            weaponHolder.Reload(Const.WEAPON_BASE_BULLET_AMOUNT);
        }

        //Destroy(weaponPrefab.gameObject);
        weaponPrefab.TF.SetParent(PoolControl.Instance.WeaponPoolTF);
        weaponPrefab.TF.localScale = Vector3.one;
        WeaponPool.Despawn(weaponPrefab);

        //Destroy(gameObject);
        SimplePool.Despawn(this);
    }

    public void StopMoving()
    {
        spinSpeed = 0f;
        speed = 0f;
    }

    private void SetDroppedShape()
    {
        weaponPrefab.TF.localPosition = new Vector3(0, 0, 1);
        weaponPrefab.TF.localEulerAngles = new Vector3(-50, 180, 0);
    }

    private void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character && character == weaponHolder.Owner)
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
