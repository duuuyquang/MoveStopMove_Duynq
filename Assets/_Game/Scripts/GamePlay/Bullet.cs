using UnityEngine;

public class Bullet : GameUnit
{
    [SerializeField] ParticleSystem dropPartical;

    #region Transform
    private float speed;
    private float spinSpeed;

    private Vector3 rotateAxis;
    private Vector3 targetPos;

    public WeaponHolder WeaponHolder;
    public Weapon WeaponPrefab { get; private set; }

    private bool IsDestination => Vector3.Distance(TF.position + Vector3.up * (TF.position.y - targetPos.y), targetPos) < 0.1f;
    #endregion

    #region Weapon Return
    private bool isReturning = false;
    #endregion

    #region Weapon Grab
    private float grabTimer = 0f;
    private bool needGrab = false;
    public bool IsDropped { get; private set; }
    #endregion

    void Update()
    {
        ProcessBehaviour();
    }

    public void OnInit(WeaponHolder weaponHolder, Vector3 targetPos)
    {
        this.targetPos = targetPos;
        this.WeaponHolder = weaponHolder;

        speed       = weaponHolder.Speed;
        spinSpeed   = weaponHolder.CurWeapon.SpinSpeed;
        rotateAxis  = weaponHolder.CurWeapon.RotateAxis;
        needGrab    = weaponHolder.CurWeapon.IsGrab;

        IsDropped = false;
        isReturning = false;
        grabTimer = 0f;

        InitSize();
        SpawnWeapon();
        CheckBooster();
    }

    private void InitSize()
    {
        TF.localScale = Vector3.one * WeaponHolder.TF.localScale.x;
    }

    private void CheckBooster()
    {
        if(WeaponHolder.Owner.IsBoosterType(BoosterType.Attack))
        {
            speed *= Booster.ATTACK_SPD_INDEX;
            TF.localScale = Vector3.one * Booster.ATTACK_BULLET_SIZE_INDEX;
        }
    }

    private void SpawnWeapon()
    {
        WeaponPrefab = WeaponPool.Spawn<Weapon>(WeaponHolder.Owner.WeaponType, TF.position, Quaternion.identity);
        WeaponPrefab.TF.SetParent(TF, false);
        WeaponPrefab.OnInit();

        TF.LookAt(targetPos);

        // modify actual direction to the target
        TF.eulerAngles += Cache.GetVector(90f, 0f, 0f);

        //scale to current size
        TF.localScale += Vector3.one * (WeaponHolder.Owner.CurSize - 1f) * 0.5f;
    }

    private void ProcessBehaviour()
    {
        MovingToTarget();
        if (IsDestination)
        {
            if(!ProcessForSpecificBehaviours())
            {
                WeaponHolder.ReloadBase();
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
        if (WeaponHolder && WeaponHolder.CurWeapon.IsReturn && !isReturning)
        {
            SetTargetPos(WeaponHolder.Owner.TF.position);
            isReturning = true;
            return true;
        }
        return false;
    }

    private bool ProcessGrabWeapon()
    {
        if (needGrab)
        {
            if (grabTimer == 0)
            {
                dropPartical.Play();
                IsDropped = true;
                StopMoving();
                SetDroppedShape();
            }

            grabTimer += Time.deltaTime;
            if(grabTimer >= Const.WEAPON_DROP_TIME)
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
        WeaponPrefab.TF.Rotate(rotateAxis, spinSpeed * Time.deltaTime);
    }

    public void OnDespawn()
    {
        IsDropped = false;
        isReturning = false;
        grabTimer = 0f;

        WeaponPrefab.TF.SetParent(PoolControl.Instance.WeaponPoolTF);
        WeaponPrefab.TF.localScale = Vector3.one;
        WeaponPool.Despawn(WeaponPrefab);
        SimplePool.Despawn(this);
    }

    public void StopMoving()
    {
        spinSpeed = 0f;
        speed = 0f;
    }

    private void SetDroppedShape()
    {
        WeaponPrefab.TF.localPosition = Cache.GetVector(0f, 0f, 1f);
        WeaponPrefab.TF.localEulerAngles = Cache.GetVector(-50f, 180f, 0f);
    }

    private void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }

    private bool CheckGrabWeapon()
    {
        if (needGrab)
        {
            SetStayAtCurPos();
            return true;
        }
        return false;
    }

    private bool CheckValidToHit(Character opponent)
    {
        if (opponent != WeaponHolder.Owner && CheckValidStatus(opponent) && !IsDropped)
        {
            OnHitOpponent(opponent);
            return true;
        }
        return false;
    }

    private bool CheckValidStatus(Character opponent) => opponent.IsStatus(StatusType.Normal) || opponent.IsStatus(StatusType.Attacking);

    private void OnHitOpponent(Character opponent)
    {
        SoundManager.Instance.PlayWeaponHit(WeaponHolder.Owner.audioSource);
        WeaponHolder.Owner.OnTargetKilled(opponent);
        EnemyManager.Instance.SetRecordHighestPoint(WeaponHolder.Owner.CombatPoint);
        opponent.OnDead(WeaponHolder.Owner);
    }

    private void SetStayAtCurPos()
    {
        StopMoving();
        SetTargetPos(TF.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        Character opponent = Cache.GetChar(other);
        if(opponent && !CheckValidToHit(opponent))
        {
            return;
        }

        if (CheckGrabWeapon())
        {
            return;
        }

        //if(!needGrab)
        //{
        //    WeaponHolder.ReloadBase();
        //    OnDespawn();
        //}
        WeaponHolder.ReloadBase();
        OnDespawn();
    }
}
