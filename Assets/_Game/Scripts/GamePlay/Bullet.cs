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
        this.weaponHolder = weaponHolder;

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
        TF.localScale = Vector3.one * weaponHolder.TF.localScale.x;
    }

    private void CheckBooster()
    {
        if(weaponHolder.Owner.IsBoosterType(BoosterType.Attack))
        {
            speed *= Booster.ATTACK_SPD_INDEX;
            TF.localScale = Vector3.one * Booster.ATTACK_BULLET_SIZE_INDEX;
            needGrab = false;
        }
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

            if (!weaponHolder || weaponHolder && (Vector3.Distance(weaponHolder.Owner.TF.position, TF.position) <= 1.5f) || grabTimer >= Const.WEAPON_DROP_TIME)
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
        weaponPrefab.TF.Rotate(rotateAxis, spinSpeed * Time.deltaTime);
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
        if (opponent != weaponHolder.Owner && CheckValidStatus(opponent) && !IsDropped)
        {
            OnHitOpponent(opponent);
            return true;
        }
        return false;
    }

    private bool CheckValidStatus(Character opponent) => opponent.IsStatus(StatusType.Normal) || opponent.IsStatus(StatusType.Attacking);

    private void OnHitOpponent(Character opponent)
    {
        SoundManager.Instance.PlayWeaponHit(weaponHolder.Owner.audioSource);
        weaponHolder.Owner.OnTargetKilled(opponent);
        EnemyManager.Instance.SetRecordHighestPoint(weaponHolder.Owner.CombatPoint);
        opponent.OnDead();
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

        if(CheckGrabWeapon())
        {
            return;
        }
        
        OnDespawn();
    }
}
