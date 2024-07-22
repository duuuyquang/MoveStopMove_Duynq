using UnityEngine;

public class Bullet : GameUnit
{
    [SerializeField] ParticleSystem dropPartical;

    #region Transform
    private float speed;
    private float spinSpeed;

    private Vector3 rotateAxis;
    private Vector3 targetPos;

    private WeaponHolder weaponHolder;
    private Weapon weaponPrefab;

    private bool IsDestination => Vector3.Distance(TF.position + Vector3.up * (TF.position.y - targetPos.y), targetPos) < 0.1f;
    #endregion

    #region Weapon Return
    private bool isReturning = false;
    #endregion

    #region Weapon Grab
    private float grabTimer = 0f;
    private bool isGrab = false;
    public bool IsDropped { get; private set; }
    #endregion

    void Update()
    {
        ProcessBehaviour();
    }

    public void OnInit(WeaponHolder weaponHolder, Vector3 targetPos)
    {
        this.targetPos = targetPos;
        this.weaponHolder = weaponHolder;

        speed       = weaponHolder.Owner.BaseAtkSpeed + weaponHolder.CurWeapon.BonusSpeed;
        spinSpeed   = weaponHolder.CurWeapon.SpinSpeed;
        rotateAxis  = weaponHolder.CurWeapon.RotateAxis;
        isGrab      = weaponHolder.CurWeapon.IsGrab;

        IsDropped = false;
        isReturning = false;
        grabTimer = 0f;

        InitSize();

        SpawnWeapon();
    }

    private void InitSize()
    {
        TF.localScale = Vector3.one * weaponHolder.TF.localScale.x;
    }

    private void SpawnWeapon()
    {
        weaponPrefab = WeaponPool.Spawn<Weapon>(weaponHolder.Owner.WeaponType, TF.position, Quaternion.identity);
        weaponPrefab.TF.SetParent(TF, false);
        weaponPrefab.OnInit();

        TF.LookAt(targetPos);

        // modify actual direction to the target
        TF.eulerAngles += Cache.GetVector(90f, 0f, 0f);

        //scale to current size
        TF.localScale += Vector3.one * (weaponHolder.Owner.CurSize - 1f) * 0.5f;
    }

    private void ProcessBehaviour()
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
                IsDropped = true;
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

        weaponPrefab.TF.SetParent(PoolControl.Instance.WeaponPoolTF);
        weaponPrefab.TF.localScale = Vector3.one;
        WeaponPool.Despawn(weaponPrefab);
        SimplePool.Despawn(this);
    }

    public void StopMoving()
    {
        spinSpeed = 0f;
        speed = 0f;
    }

    private void SetDroppedShape()
    {
        weaponPrefab.TF.localPosition = Cache.GetVector(0f, 0f, 1f);
        weaponPrefab.TF.localEulerAngles = Cache.GetVector(-50f, 180f, 0f);
    }

    private void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }

    //TODO: tach code
    private void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character && character == weaponHolder.Owner)
        {
            return;
        }

        if(character && !character.IsStatus(StatusType.Dead) && !IsDropped)
        {
            OnHitOpponent(character);
        }

        if (isGrab)
        {
            SetStayAtCurPos();
            return;
        }

        OnDespawn();
    }

    private void OnHitOpponent(Character opponent)
    {
        weaponHolder.Owner.OnTargetKilled(opponent);
        EnemyManager.Instance.SetRecordHighestPoint(weaponHolder.Owner.CombatPoint);
        opponent.OnDead();
    }

    private void SetStayAtCurPos()
    {
        StopMoving();
        SetTargetPos(TF.position);
    }
}
