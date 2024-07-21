using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusType { Normal, Attacking, Dead, Win }

public class Character : GameUnit
{
    private string charName;
    public string Name { get { return charName; } set { if (value.Length < 25) { charName = value; } } }

    #region Skin Related
    [SerializeField] protected Renderer charRenderer;
    [field: SerializeField] public WeaponHolder WeaponHolder { get; protected set; }
    [SerializeField] protected Transform headHolder;
    [SerializeField] protected Transform shieldHolder;
    [SerializeField] protected Renderer pantsHolder;
    [SerializeField] protected Transform wingHolder;
    [SerializeField] protected Transform tailHolder;
    public Item CurHead { get; protected set; }
    public Item CurPants { get; protected set; }
    public Item CurShield { get; protected set; }
    public Item CurTail { get; protected set; }
    public Item CurWing { get; protected set; }
    public Item CurSet { get; protected set; }
    public ColorType ColorType { get; private set; }
    #endregion

    #region Movement
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float baseMoveSpeed;
    protected float bonusMoveSpeed;
    private string curAnim = Const.ANIM_NAME_IDLE;
    public float MoveSpeed => baseMoveSpeed + bonusMoveSpeed;
    public virtual bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;
    #endregion

    //------------------ Combat props ------------------------
    protected int[] upSizeThresholds = { 1, 5, 13, 25, 41, 61 }; // kill 4 enemies same scale to up scale: 4, 8, 12, 16, 20 (combatPoint relatively)

    private int combatPoint;
    public int CombatPoint { get { return combatPoint; } set { combatPoint = Mathf.Max(0, value); } }

    private Coroutine attackCoroutine;

    public  WeaponType WeaponType { get; protected set; }

    [SerializeField] protected Transform atkRangeTF;
    [SerializeField] protected float baseAtkRange;
    [SerializeField] protected float baseAtkSpeed;

    private float ItemBonusAtkRange { get; set; }
    public float WeaponBonusAtkRange { get; set; }
    protected float BonusAtkRange => ItemBonusAtkRange + WeaponBonusAtkRange;
    public float CurAttackRange => (baseAtkRange + BonusAtkRange) * CurSize;
    public float CurSize { get; protected set; }
    public float BaseAttackSpeed => baseAtkSpeed;
    public float BonusGoldMultiplier { get; private set; }

    #region Navigation
    [SerializeField] protected Image targetIndicatorImage;
    private Indicator Indicator;
    protected List<Character> targetsInRange = new List<Character>();
    protected Character curTargetChar = null;
    public bool HasTargetInRange => curTargetChar != null;
    #endregion

    #region Status
    protected StatusType curStatus;
    public StatusType CurStatus => curStatus;
    public bool IsStatus(StatusType status) => curStatus == status;
    #endregion

    #region DataSO
    public ItemDataSO itemDataSO;
    public ColorDataSO colorDataSO;
    #endregion

    [SerializeField] ParticleSystem bulletPartical;
    [SerializeField] ParticleSystem sizeUpPartical;

    protected virtual void Update()
    {
        if (GameManager.IsState(GameState.GamePlay))
        {
            DetectNearestTarget();
            UpdateAnimation();
        }
    }

    #region Init
    public virtual void OnInit()
    {
        InitSize();
        InitStatus();
        ClearTargets();
    }

    public void OnPlay()
    {
        InitIndicator();
        ToggleAtkRangeTF(true);
    }

    protected virtual void InitIndicator()
    {
        Indicator = SimplePool.Spawn<Indicator>(PoolType.Indicator, Vector3.zero, Quaternion.identity);
        Indicator.OnInit(this);
    }

    private void InitSize()
    {
        CurSize = 1f;
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
    #endregion

    #region Change weapons and skins
    public void ChangeWeapon(WeaponType type)
    {
        WeaponType = type;
        WeaponHolder.OnInit(this);
        ToggleWeapon(true);
        SetAttackRangeTF(baseAtkRange + BonusAtkRange);
    }

    public void ChangeHead(ItemType type)
    {
        if (CurHead != null)
        {
            Destroy(CurHead.gameObject);
        }
        CurHead = Instantiate(itemDataSO.GetHead(type), headHolder);
        UpdateBonusStatsFromItem(CurHead);
    }

    public virtual void ChangePants(ItemType type)
    {
        CurPants = itemDataSO.GetPants(type);
        pantsHolder.material = CurPants.GetComponent<Renderer>().sharedMaterial;
        UpdateBonusStatsFromItem(CurPants);
    }

    public void ChangeShield(ItemType type)
    {
        if (CurShield != null)
        {
            Destroy(CurShield.gameObject);
        }
        CurShield = Instantiate(itemDataSO.GetShield(type), shieldHolder);
        UpdateBonusStatsFromItem(CurShield);
    }

    public void ChangeSet(ItemType type)
    {
        CurSet = itemDataSO.GetSet(type);
        ChangeHead(CurSet.HeadItem.Type);
        ChangePants(CurSet.PantsItem.Type);
        ChangeShield(CurSet.ShieldItem.Type);
        ChangeWing(CurSet.WingItem.Type);
        ChangeTail(CurSet.TailItem.Type);

        if(type == ItemType.None)
        {
            ChangeColor(ColorType);
        } 
        else
        {
            ChangeColorBySetItem(CurSet.CharColorType);
        }
        UpdateBonusStatsFromItem(CurSet);
    }

    private void ChangeWing(ItemType type)
    {
        if (CurWing != null)
        {
            Destroy(CurWing.gameObject);
        }
        CurWing = Instantiate(itemDataSO.GetWing(type), wingHolder);
    }

    private void ChangeTail(ItemType type)
    {
        if (CurTail != null)
        {
            Destroy(CurTail.gameObject);
        }
        CurTail = Instantiate(itemDataSO.GetTail(type), tailHolder);
    }

    protected virtual void UpdateBonusStatsFromItem(Item item)
    {
        ItemBonusAtkRange = baseAtkRange * item.BonusAttackRange * 0.01f;
        BonusGoldMultiplier = item.BonusGold;
        SetAttackRangeTF(baseAtkRange + BonusAtkRange);
    }

    protected void ChangeColor(ColorType type)
    {
        ColorType = type;
        charRenderer.material = colorDataSO.GetMat(type);
    }

    protected void ChangeColorBySetItem(ColorType type)
    {
        if (type == ColorType.None)
        {
            ChangeColor(ColorType);
        }
        else
        {
            charRenderer.material = colorDataSO.GetMat(type);
        }
    }

    protected void ChangeColorDeath(ColorType type)
    {
        charRenderer.material = colorDataSO.GetMatDeath(type);
    }
    #endregion

    #region Target detection
    private bool IsInvalidTarget(Character enemy) => enemy.IsStatus(StatusType.Dead) || Vector3.Distance(enemy.TF.position, TF.position) > CurAttackRange;

    protected virtual void DetectNearestTarget()
    {
        if(!IsStatus(StatusType.Normal))
        {
            return;
        }
        float nearestDist = 0;
        curTargetChar = null;
        float checkingDist;
        foreach (Character enemy in targetsInRange) {
            if (IsInvalidTarget(enemy))
            {
                continue;
            }
            checkingDist = Vector3.Distance(enemy.TF.position, TF.position);
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
    #endregion

    #region Attack
    private bool CheckAttackableConditions => WeaponHolder.HasBullet && HasTargetInRange && !IsStatus(StatusType.Attacking) && !IsStatus(StatusType.Dead);

    public void CheckToProcessAttack()
    {
        if(IsStanding)
        {
            if (CheckAttackableConditions)
            {
                ProcessAttack();
            }
        }
        else
        {
            StopAttack();
        }
    }

    private void ProcessAttack()
    {
        ChangeStatus(StatusType.Attacking);
        ChangeAnim(Const.ANIM_NAME_ATTACK);

        attackCoroutine = StartCoroutine(IEAttack());
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
        yield return Cache.GetWaitSecs(0.3f);
        if(!IsStatus(StatusType.Dead))
        {
            WeaponHolder.OnShoot(targetPos);
            ToggleWeapon(false);
            yield return Cache.GetWaitSecs(1f);
            ChangeStatus(StatusType.Normal);
        }
    }

    protected void LookAtTarget(Vector3 targetPos)
    {
        // we don't want character looks down when its size scales bigger
        targetPos.y = TF.position.y;
        TF.LookAt(targetPos);
    }

    protected void ToggleAtkRangeTF(bool value)
    {
        atkRangeTF.gameObject.SetActive(value);
    }

    public void ToggleWeapon(bool value)
    {
        if (WeaponHolder)
        {
            WeaponHolder.gameObject.SetActive(value);
        }
    }

    public virtual void ProcessOnTargetKilled(Character opponent)
    {
        RemoveTargetInRange(opponent);
        CheckToScaleSizeUp(opponent.CombatPoint);
        if (Indicator)
        {
            Indicator.UpdateCombatPoint(CombatPoint);
        }
    }
    #endregion

    #region Scale
    public void CheckToScaleSizeUp(int opponentCombatPoint)
    {
        int oldCombatPoint = CombatPoint;
        int gainnedPoint = GetCombatPointInReturn(opponentCombatPoint);
        ShowCombatPointGainned(gainnedPoint);
        CombatPoint += gainnedPoint;

        for (int i = 0; i < upSizeThresholds.Length; i++)
        {
            if (oldCombatPoint < upSizeThresholds[i] && CombatPoint >= upSizeThresholds[i])
            {
                ProcessScaleSizeUp();
                return;
            }
        }
    }

    protected void ProcessScaleSizeUp(bool vfxOn = true)
    {
        CurSize += Const.CHARACTER_UPSCALE_UNIT;
        TF.localScale = Vector3.one * CurSize;
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
            if (point <= upSizeThresholds[i])
            {
                returnPoint = i + 1;
                break;
            }
        }
        return returnPoint;
    }
    #endregion

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
        if (curStatus == StatusType.Dead)
        {
            return;
        }

        if (curStatus != type)
        {
            curStatus = type;
        }
    }

    //TODO: optimize other structure
    protected virtual void UpdateAnimation()
    {
        if (IsStatus(StatusType.Dead))
        {
            ChangeAnim(Const.ANIM_NAME_DIE);
        }

        if (IsStatus(StatusType.Normal))
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

    public virtual void OnDead()
    {
        Invoke(nameof(OnDespawn), 2f);
        ChangeStatus(StatusType.Dead);
        StopMoving();
        ClearTargets();

        bulletPartical.Play();
        Indicator = null;
        ChangeColorDeath(ColorType);
        ToggleTargetIndicator(false);
        GameManager.Instance.UpdateAliveCountText();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character)
        {
            if (!character.IsStatus(StatusType.Dead))
            {
                //TODO: move to other manager
                targetsInRange.Add(character);
            }
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

    public virtual void StopMoving()
    {
        if (IsStatus(StatusType.Dead))
        {
            return;
        }
    }

    #region empty virtual methods
    protected virtual void ShowCombatPointGainned(int point)
    {
    }
    public virtual void ToggleTargetIndicator(bool value)
    {
    }
    public virtual void OnDespawn()
    {
    }
    #endregion
}
