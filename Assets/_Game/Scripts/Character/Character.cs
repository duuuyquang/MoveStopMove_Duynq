using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusType { Normal, Attacking, Dead, Win }

public class Character : GameUnit
{
    //------------------ Movement props ---------------------
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float baseMoveSpeed;
    protected float bonusMoveSpeed;
    private string curAnim = Const.ANIM_NAME_IDLE;
    //public Transform TF;

    public float MoveSpeed => baseMoveSpeed + bonusMoveSpeed;
    public virtual bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;

    //------------------ Basic props -------------------------
    private string charName;
    public string Name { get { return charName; } set { if (value.Length < 25) { charName = value; } } }

    [SerializeField] protected Renderer charRenderer;
    private ColorType colorType;
    public ColorType ColorType { get { return colorType; } set { colorType = value; } }

    //------------------ Combat props ------------------------
    protected int[] upSizeThresholds = { 1, 5, 13, 25, 41, 61 }; // kill 4 enemies same scale to up scale: 4, 8, 12, 16, 20

    private int combatPoint;
    public int CombatPoint { get { return combatPoint; } set { combatPoint = Mathf.Max(0, value); } }

    private IEnumerator attackCoroutine;

    [SerializeField] protected WeaponHolder weaponHolder;
    [SerializeField] protected Transform headHolder;
    [SerializeField] protected Transform shieldHolder;
    [SerializeField] protected Renderer pantsHolder;
    protected Item curHead;
    protected Item curPants;
    protected Item curShield;
    public WeaponHolder WeaponHolder => weaponHolder;
    public Item CurHead => curHead;
    public Item CurPants => curPants;
    public Item CurShield  => curShield;

    protected WeaponType weaponType;
    public  WeaponType WeaponType => weaponType;

    [SerializeField] protected Transform atkRangeTF;
    [SerializeField] protected float baseAtkRange;
    [SerializeField] protected float baseAtkSpeed;
    protected float curSize;
    protected float bonusAtkRange;
    public float CurSize => curSize;
    public float CurAttackRange => (baseAtkRange + bonusAtkRange) * curSize;
    public float BaseAttackSpeed => baseAtkSpeed;
    public float BonusAttackRange { get { return bonusAtkRange; } set { bonusAtkRange = Mathf.Max(value, 0); } }

    //------------------ Navigation props --------------------
    [SerializeField] protected Image targetIndicatorImage;
    //[SerializeField] Indicator indicatorPrefab;
    protected Indicator indicator;
    public Indicator Indicator => indicator;

    protected List<Character> targetsInRange = new List<Character>();
    protected Character curTargetChar = null;
    public bool HasTargetInRange => curTargetChar != null;

    //------------------ Status props ------------------------
    protected StatusType curStatus;
    public StatusType CurStatus => curStatus;
    public bool IsStatus(StatusType status) => curStatus == status;

    //------------------ Data props --------------------------
    public ItemDataSO itemDataSO;
    public ColorDataSO colorDataSO;

    [SerializeField] ParticleSystem bulletPartical;
    [SerializeField] ParticleSystem sizeUpPartical;

    protected virtual void Update()
    {
        if (GameManager.IsState(GameState.GamePlay))
        {
            if (IsStatus(StatusType.Normal))
            {
                DetectNearestTarget();
            }

            if (!IsStatus(StatusType.Dead))
            {
                if (indicator == null)
                {
                    InitIndicator();
                }
            }

            UpdateAnimation();
        }

        if (GameManager.IsState(GameState.Setting))
        {
            if (!IsStatus(StatusType.Dead))
            {
                StopMoving();
            }
        }
    }

    public virtual void OnInit()
    {
        InitBasicStats();
        InitDependentFactors();
        InitIndependentFactors();
    }

    protected virtual void InitBasicStats()
    {
        InitSize();
    }

    private void InitDependentFactors()
    {
        SetupSizeByInitCombatPoint(CombatPoint);
    }

    private void InitIndependentFactors()
    {
        InitStatus();
        ClearTargets();
    }

    protected virtual void InitIndicator()
    {
        //indicator = Instantiate(indicatorPrefab, UIManager.Instance.GetUI<CanvasGamePlay>().transform);
        indicator = SimplePool.Spawn<Indicator>(PoolType.Indicator, Vector3.zero, Quaternion.identity);
        indicator.OnInit(this);
    }

    public void ChangeWeapon(WeaponType type)
    {
        weaponType = type;
        weaponHolder.OnInit(this);
        ToggleWeapon(true);
        SetAttackRangeTF(baseAtkRange + bonusAtkRange);
    }

    public void ChangeHead(ItemType type)
    {
        if(curHead != null)
        {
            bonusAtkRange -= baseAtkRange * curHead.BonusAttackRange * 0.01f;
            Destroy(curHead.gameObject);
        }
        curHead = Instantiate(itemDataSO.GetHead(type), headHolder.transform);

        bonusAtkRange += baseAtkRange * curHead.BonusAttackRange * 0.01f;
        SetAttackRangeTF(baseAtkRange + bonusAtkRange);
    }

    public virtual void ChangePants(ItemType type)
    {
        curPants = itemDataSO.GetPants(type);
        pantsHolder.material = curPants.GetComponent<Renderer>().sharedMaterial;
        bonusMoveSpeed = baseMoveSpeed * curPants.BonusMoveSpeed * 0.01f;
    }

    public void ChangeShield(ItemType type)
    {
        if (curShield != null)
        {
            Destroy(curShield.gameObject);
        }
        curShield = Instantiate(itemDataSO.GetShield(type), shieldHolder.transform);
        curShield.gameObject.SetActive(true);
    }

    private void InitSize()
    {
        curSize = 1f;
        TF.localScale = Vector3.one;
    }

    protected void SetupSizeByInitCombatPoint(int initPoint)
    {
        for (int i = 0; i < upSizeThresholds.Length; i++)
        {
            if (initPoint >= upSizeThresholds[i])
            {
                ProcessScaleSizeUp(false);
            }
        }
    }

    protected void ClearTargets()
    {
        targetsInRange.Clear();
        curTargetChar = null;
    }

    protected void InitStatus()
    {
        curStatus = StatusType.Normal;
    }

    protected virtual void SetAttackRangeTF(float atkRange)
    {
        atkRangeTF.localScale = new Vector3(atkRange * 2f, 0.1f, atkRange * 2f);
    }

    public virtual void OnDespawn()
    {
        if (gameObject)
        {
            Destroy(gameObject);
        }
    }

    protected void ChangeColor(ColorType type)
    {
        ColorType = type;
        charRenderer.material = colorDataSO.GetMat(type);
    }

    protected void ChangeColorDeath(ColorType type)
    {
        charRenderer.material = colorDataSO.GetMatDeath(type);
    }

    public void ChangeAnim(string animName)
    {
        if (curAnim != animName)
        {
            animator.ResetTrigger(curAnim);
            curAnim = animName;
            animator.SetTrigger(animName);
        }
    }

    protected void ChangeStatus(StatusType type)
    {
        if(curStatus == StatusType.Dead)
        {
            return;
        }

        if (curStatus != type)
        {
            curStatus = type;
        }
    }

    protected virtual void DetectNearestTarget()
    {
        float nearestDist = 0;
        curTargetChar = null;

        foreach (Character enemy in targetsInRange) {
            if (enemy.IsStatus(StatusType.Dead) || Vector3.Distance(enemy.TF.position, TF.position) > CurAttackRange)
            {
                continue;
            }
            float checkingDist = Vector3.Distance(enemy.TF.position, TF.position);
            if (nearestDist == 0)
            {
                nearestDist = checkingDist;
                curTargetChar = enemy;
            }

            if (checkingDist < nearestDist)
            {
                nearestDist = checkingDist;
                curTargetChar = enemy;
            }
        }
    }

    public void RemoveTargetInRange(Character character)
    {
        character.ToggleTargetIndicator(false);
        targetsInRange.Remove(character);
    }

    public void CheckAndProcessAttack()
    {
        if (weaponHolder.HasBullet && HasTargetInRange && !IsStatus(StatusType.Attacking))
        {
            ProcessAttack();
        }
    }

    private void ProcessAttack()
    {
        ChangeStatus(StatusType.Attacking);
        ChangeAnim(Const.ANIM_NAME_ATTACK);
        attackCoroutine = IEAttack();
        StartCoroutine(attackCoroutine);
    }

    protected void StopAttack()
    {
        ChangeStatus(StatusType.Normal);
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator IEAttack()
    {
        Vector3 targetPos = TF.position + (curTargetChar.TF.position - TF.position).normalized * CurAttackRange;
        yield return new WaitForSeconds(0.3f);
        if(!IsStatus(StatusType.Dead))
        {
            weaponHolder.OnShoot(targetPos);
            ToggleWeapon(false);
            yield return new WaitForSeconds(1f);
            ChangeStatus(StatusType.Normal);
        }
    }

    protected void LookAtTarget(Vector3 targetPos)
    {
        // we don't want character looks down when its size scales bigger
        Vector3 targetDir = new Vector3(targetPos.x, TF.position.y, targetPos.z);
        TF.LookAt(targetDir);
    }

    protected virtual void OnDead()
    {
        Invoke(nameof(OnDespawn), 2f);
        ChangeStatus(StatusType.Dead);
        ChangeColorDeath(ColorType);
        ToggleTargetIndicator(false);
        StopMoving();
        bulletPartical.Play();
        indicator = null;
        GameManager.Instance.UpdateAliveNumText();
    }

    protected virtual void UpdateAnimation()
    {
        if (IsStatus(StatusType.Dead))
        {
            ChangeAnim(Const.ANIM_NAME_DIE);
        }

        if(IsStatus(StatusType.Normal))
        {
            if (IsStanding)
            {
                ChangeAnim(Const.ANIM_NAME_IDLE);
            } 
            else
            {
                ChangeAnim(Const.ANIM_NAME_RUN);
            }
        }

        if (IsStatus(StatusType.Attacking))
        {
            ChangeAnim(Const.ANIM_NAME_ATTACK);
            if (curTargetChar)
            {
                LookAtTarget(curTargetChar.TF.position);
            }
        }
    }

    protected void ToggleAtkRangeTF(bool value)
    {
        atkRangeTF.gameObject.SetActive(value);
    }

    public void ToggleWeapon(bool value)
    {
        if(weaponHolder)
        {
            weaponHolder.gameObject.SetActive(value);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character)
        {
            if(!character.IsStatus(StatusType.Dead))
            {
                targetsInRange.Add(character);
            }
        }
        
        Bullet bullet = Cache.GetBullet(other);
        if(bullet && this != bullet.WeaponHolder.Owner && !bullet.IsDropped && !IsStatus(StatusType.Dead))
        {
            bullet.WeaponHolder.Owner.ProcessOnTargetKilled(this);
            EnemyManager.Instance.SetRecordHighestPoint(bullet.WeaponHolder.Owner.CombatPoint);
            OnDead();
        }
    }

    public virtual void ProcessOnTargetKilled(Character opponent)
    {
        RemoveTargetInRange(opponent);
        CheckToScaleSizeUp(opponent.CombatPoint);
        if (indicator)
        {
            indicator.UpdateCombatPoint(CombatPoint);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character)
        {
            RemoveTargetInRange(character);
        }
    }

    public void CheckToScaleSizeUp(int opponentCombatPoint)
    {
        bool onPointToScale = false;
        int oldCombatPoint = CombatPoint;
        int gainnedPoint = GetCombatPointInReturn(opponentCombatPoint);
        ShowCombatPointGainned(gainnedPoint);
        CombatPoint += gainnedPoint;

        for (int i = 0; i < upSizeThresholds.Length; i++)
        {
            if (oldCombatPoint < upSizeThresholds[i] && CombatPoint >= upSizeThresholds[i])
            {
                onPointToScale = true;
                break;
            }
        }

        if (onPointToScale)
        {
            ProcessScaleSizeUp();
        }
    }

    protected void ProcessScaleSizeUp(bool vfxOn = true)
    {
        curSize += Const.CHARACTER_UPSCALE_UNIT;
        TF.localScale = Vector3.one * curSize;
        TF.position += Vector3.up * Const.CHARACTER_UPSCALE_UNIT;
        if (vfxOn)
        {
            Invoke(nameof(TriggerScaleUpVFX), 0.15f);
        }
    }

    private void TriggerScaleUpVFX()
    {
        if (!IsStatus(StatusType.Dead))
        {
            sizeUpPartical.Play();
        }
    }

    public int GetCombatPointInReturn(int point)
    {
        int returnPoint = upSizeThresholds.Length + 1;
        for (int i = 0; i < upSizeThresholds.Length; i++)
        {
            if(point <= upSizeThresholds[i])
            {
                returnPoint = i + 1;
                break;
            }
        }
        return returnPoint;
    }

    protected virtual void ShowCombatPointGainned(int point)
    {
    }
    public virtual void ToggleTargetIndicator(bool value)
    {
    }

    public virtual void StopMoving()
    {
    }
}
