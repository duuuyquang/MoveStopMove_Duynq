using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType { Normal, Attacking, Dead, Win }

public class Character : GameUnit
{
    private string charName;
    public string Name { get { return charName; } set { if (value.Length < 25) { charName = value; } } }
    #region Skin ------------------------------------------------
    [SerializeField] protected Renderer charRenderer;
    [field: SerializeField] public WeaponHolder WeaponHolder { get; protected set; }
    [SerializeField] protected Transform headHolder;
    [SerializeField] protected Transform shieldHolder;
    [SerializeField] protected Renderer  pantsHolder;
    [SerializeField] protected Transform wingHolder;
    [SerializeField] protected Transform tailHolder;
    public Item CurHead   { get; protected set; }
    public Item CurPants  { get; protected set; }
    public Item CurShield { get; protected set; }
    public Item CurTail   { get; protected set; }
    public Item CurWing   { get; protected set; }
    public Item CurSet    { get; protected set; }
    public ColorType ColorType { get; private set; }
    #endregion
    #region Movement ---------------------------------------------
    protected float bonusMoveSpeed;
    private string curAnim = Const.ANIM_NAME_IDLE;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float baseMoveSpeed;
    public float MoveSpeed => baseMoveSpeed + bonusMoveSpeed;
    public virtual bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;
    #endregion
    #region Combat -----------------------------------------------
    protected static int[] SCALEUP_THRESHOLD = { 1, 5, 13, 25, 41, 61 }; // kill 4 enemies same scale to upscale: 4, 8, 12, 16, 20 (CombatPoint relatively)
    private Coroutine attackCoroutine;
    private Vector3 atkTargetPos;
    [SerializeField] protected Transform atkRangeTF;
    [field: SerializeField] public float BaseAtkRange { get; protected set; }
    [field: SerializeField] public float BaseAtkSpeed { get; protected set; }
    public  WeaponType WeaponType { get; protected set; }
    public float ItemBonusAtkRange { get; private set; }
    public float CurSize { get; protected set; }
    public float BonusGoldMultiplier { get; private set; }
    public int CombatPoint { get; protected set; }
    private float BonusAtkRange => ItemBonusAtkRange + WeaponHolder.CurWeapon.BonusAttackRange;
    public float CurAttackRange => (BaseAtkRange + BonusAtkRange) * CurSize;
    private bool CheckAttackableConditions => WeaponHolder.HasBullet && HasTargetInRange && !IsStatus(StatusType.Attacking) && !IsStatus(StatusType.Dead);
    #endregion
    #region Navigation --------------------------------------------
    private Indicator indicator;
    public List<Character> TargetsInRange { get; private set; } = new();
    public Character CurTargetChar { get; set; }
    public bool HasTargetInRange => CurTargetChar != null;
    #endregion
    #region Status ------------------------------------------------
    public StatusType CurStatus { get; protected set; }
    public bool IsStatus(StatusType status) => CurStatus == status;
    #endregion
    #region DataSO ------------------------------------------------
    public ItemDataSO itemDataSO;
    public ColorDataSO colorDataSO;
    #endregion
    [SerializeField] ParticleSystem bulletPartical;
    [SerializeField] ParticleSystem sizeUpPartical;

    protected virtual void Update()
    {
        TargetDetector.DetectNearestTarget(this, TargetsInRange);
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
        indicator = SimplePool.Spawn<Indicator>(PoolType.Indicator, Vector3.zero, Quaternion.identity);
        indicator.OnInit(this);
    }

    private void InitSize()
    {
        CurSize = 1f;
        TF.localScale = Vector3.one;
    }

    protected void ClearTargets()
    {
        TargetsInRange.Clear();
        CurTargetChar = null;
    }

    protected void InitStatus()
    {
        CurStatus = StatusType.Normal;
    }

    protected virtual void SetAttackRangeTF(float atkRange)
    {
        atkRangeTF.localScale = Cache.GetVector(atkRange * 2f, 0.1f, atkRange * 2f);
    }
    #endregion
    #region Change weapons and skins
    public void ChangeWeapon(WeaponType type)
    {
        WeaponType = type;
        WeaponHolder.OnInit(this);
        ToggleWeapon(true);
        SetAttackRangeTF(BaseAtkRange + BonusAtkRange);
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
        if (CurShield != null)Destroy(CurShield.gameObject);
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
        UpdateBonusStatsFromItem(CurSet);
        if (type == ItemType.None) ChangeColor(ColorType);
        else ChangeColorBySetItem(CurSet.CharColorType);
    }

    private void ChangeWing(ItemType type)
    {
        if (CurWing != null) Destroy(CurWing.gameObject);
        CurWing = Instantiate(itemDataSO.GetWing(type), wingHolder);
    }

    private void ChangeTail(ItemType type)
    {
        if (CurTail != null) Destroy(CurTail.gameObject);
        CurTail = Instantiate(itemDataSO.GetTail(type), tailHolder);
    }

    protected virtual void UpdateBonusStatsFromItem(Item item)
    {
        ItemBonusAtkRange = BaseAtkRange * item.BonusAttackRange * 0.01f;
        BonusGoldMultiplier = item.BonusGold;
        SetAttackRangeTF(BaseAtkRange + BonusAtkRange);
    }

    protected void ChangeColor(ColorType type)
    {
        ColorType = type;
        charRenderer.material = colorDataSO.GetMat(type);
    }

    protected void ChangeColorBySetItem(ColorType type)
    {
        if (type == ColorType.None) ChangeColor(ColorType);
        else
        {
            ColorType = type;
            charRenderer.material = colorDataSO.GetMat(type);
        }
    }

    protected void ChangeColorDeath(ColorType type)
    {
        charRenderer.material = colorDataSO.GetMatDeath(type);
    }
    #endregion
    #region Attack
    public void CheckToProcessAttack()
    {
        if (IsStanding)
        {
            if (CheckAttackableConditions) ProcessAttack();
        }
        else StopAttack();
    }

    private void ProcessAttack()
    {
        ChangeAnimByStatus(StatusType.Attacking);
        attackCoroutine = StartCoroutine(IEAttack());
    }

    protected void StopAttack()
    {
        ChangeAnimByStatus(StatusType.Normal);
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
    }

    IEnumerator IEAttack()
    {
        atkTargetPos = TF.position + (CurTargetChar.TF.position - TF.position).normalized * CurAttackRange;
        yield return Cache.GetWaitSecs(0.3f);
        if(!IsStatus(StatusType.Dead))
        {
            WeaponHolder.OnShoot(atkTargetPos);
            ToggleWeapon(false);
            yield return Cache.GetWaitSecs(1f);
            ChangeAnimByStatus(StatusType.Normal);
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
        if (WeaponHolder) WeaponHolder.gameObject.SetActive(value);
    }

    public virtual void OnTargetKilled(Character opponent)
    {
        TargetDetector.RemoveTargetInRange(this, opponent);
        CheckToScaleSizeUp(opponent.CombatPoint);
        if (indicator) indicator.UpdateCombatPoint(CombatPoint);
    }
    #endregion
    #region Scale
    public void CheckToScaleSizeUp(int opponentCombatPoint)
    {
        int oldCombatPoint = CombatPoint;
        int gainnedPoint = GetCombatPointInReturn(opponentCombatPoint);
        ShowCombatPointGainned(gainnedPoint);
        CombatPoint += gainnedPoint;

        for (int i = 0; i < SCALEUP_THRESHOLD.Length; i++)
        {
            if (oldCombatPoint < SCALEUP_THRESHOLD[i] && CombatPoint >= SCALEUP_THRESHOLD[i])
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
        if (vfxOn) Invoke(nameof(TriggerScaleUpVFX), 0.15f);
    }

    private void TriggerScaleUpVFX()
    {
        if (!IsStatus(StatusType.Dead)) sizeUpPartical.Play();
    }

    public int GetCombatPointInReturn(int point)
    {
        int returnPoint = SCALEUP_THRESHOLD.Length + 1;
        for (int i = 0; i < SCALEUP_THRESHOLD.Length; i++)
        {
            if (point <= SCALEUP_THRESHOLD[i])
            {
                returnPoint = i + 1;
                break;
            }
        }
        return returnPoint;
    }

    protected void SetupSizeByInitCombatPoint(int initPoint)
    {
        for (int i = 0; i < SCALEUP_THRESHOLD.Length; i++) if (initPoint >= SCALEUP_THRESHOLD[i]) ProcessScaleSizeUp(false);
    }
    #endregion
    #region Animation Controller
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
        if (CurStatus == StatusType.Dead) return;
        if (CurStatus != type) CurStatus = type;
    }

    public virtual void ChangeAnimByCurStatus()
    {
        switch(CurStatus)
        {
            case StatusType.Normal:
                if (IsStanding) ChangeAnim(Const.ANIM_NAME_IDLE);
                else ChangeAnim(Const.ANIM_NAME_RUN);
                break;
            case StatusType.Attacking:
                ChangeAnim(Const.ANIM_NAME_ATTACK);
                LookAtTarget(atkTargetPos);
                break;
            case StatusType.Dead:
                ChangeAnim(Const.ANIM_NAME_DIE);
                break;
        }
    }

    private void ChangeAnimByStatus(StatusType type)
    {
        ChangeStatus(type);
        ChangeAnimByCurStatus();
    }
    #endregion

    public virtual void OnDead()
    {
        Invoke(nameof(OnDespawn), 2f);
        ChangeAnimByStatus(StatusType.Dead);
        StopMoving();
        ClearTargets();
        bulletPartical.Play();
        indicator = null;
        ChangeColorDeath(ColorType);
        ToggleTargetIndicator(false);
        GameManager.Instance.UpdateAliveCountText();
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character) TargetDetector.AddTargetInRange(this, character);
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character) TargetDetector.RemoveTargetInRange(this, character);
    }

    #region empty virtual methods
    protected virtual void ShowCombatPointGainned(int point){}
    public virtual void ToggleTargetIndicator(bool value){}
    public virtual void OnDespawn(){}
    public virtual void StopMoving(){}
    #endregion
}