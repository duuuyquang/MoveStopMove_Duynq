using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusType { Normal, Attacking, Dead, Win, Untouchable }

public class Character : GameUnit
{
    private string charName;
    public string Name { get { return charName; } set { if (value.Length < 25) { charName = value; } } }
    #region Skin ------------------------------------------------
    [SerializeField] protected Renderer charRenderer;
    [SerializeField] protected Transform headHolder;
    [SerializeField] protected Transform shieldHolder;
    [SerializeField] protected Renderer pantsHolder;
    [SerializeField] protected Transform wingHolder;
    [SerializeField] protected Transform tailHolder;
    [field: SerializeField] public WeaponHolder WeaponHolder { get; protected set; }
    public Item CurHead   { get; protected set; }
    public Item CurPants  { get; protected set; }
    public Item CurShield { get; protected set; }
    public Item CurTail   { get; protected set; }
    public Item CurWing   { get; protected set; }
    public Item CurSet    { get; protected set; }
    public ColorType ColorType { get; private set; }
    #endregion
    #region Booster
    public BoosterType BoosterType { get; protected set; } = BoosterType.None;
    public float BoosterSpdMultipler { get; set; } = 0.0f;
    public float BoosterAtkRange { get; private set; } = 0f;
    #endregion
    #region Movement ---------------------------------------------
    protected float bonusMoveSpeed;
    private string curAnim = Const.ANIM_NAME_IDLE;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    public float BaseMoveSpeed { get; protected set; } = Const.CHARACTER_DEFAULT_MOVE_SPD;
    public float MoveSpeed => Mathf.Min(Const.CHARACTER_MOVE_SPEED_MAX, (BaseMoveSpeed + bonusMoveSpeed + BaseMoveSpeed*BoosterSpdMultipler) * CurSize );
    public virtual bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;
    #endregion
    #region Combat -----------------------------------------------
    private Coroutine attackCoroutine;
    private Vector3 atkTargetPos;
    private float curSize;
    private int combatPoint;
    [SerializeField] protected Transform atkRangeTF;
    [field: SerializeField] public float BaseAtkRange { get; protected set; }
    [field: SerializeField] public float BaseAtkSpeed { get; protected set; }
    public  WeaponType WeaponType { get; protected set; }
    public float ItemBonusAtkRangeMultiplier { get; private set; }
    public float BonusGoldMultiplier { get; private set; }
    public float CurSize { get {  return curSize; } set { curSize = Mathf.Max(0, value); } }
    public int CombatPoint { get { return combatPoint; } set { combatPoint = Mathf.Max(0, value); } }
    protected float BonusAtkRange => WeaponHolder.CurWeapon.BonusAttackRange + BoosterAtkRange;
    public float CurAttackRange => (BaseAtkRange + BonusAtkRange) * ItemBonusAtkRangeMultiplier * CurSize;
    public float CurAttackRangeTF => (BaseAtkRange + BonusAtkRange) * ItemBonusAtkRangeMultiplier;
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
    #region Soul Collector ----------------------------------------
    protected List<Soul> soulList = new();
    #endregion
    #region DataSO ------------------------------------------------
    public ItemDataSO itemDataSO;
    public ColorDataSO colorDataSO;
    #endregion
    [SerializeField] ParticleSystem bulletPartical;
    [SerializeField] ParticleSystem sizeUpPartical;
    [SerializeField] ParticleSystem attackBoosterPartical;
    [SerializeField] ParticleSystem speedBoosterPartical;

    public AudioSource audioSource;
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
        SetAttackRangeTF(CurAttackRangeTF);
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
        UpdateBonusStatsFromItem(CurSet);
        if (type == ItemType.None)
        {
            ChangeColor(ColorType);
        }
        else
        {
            ChangeColorBySetItem(CurSet.CharColorType);
        }
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
        ItemBonusAtkRangeMultiplier = 1 + item.BonusAttackRange * 0.01f;
        BonusGoldMultiplier = 1 + item.BonusGold * 0.01f;
        SetAttackRangeTF(CurAttackRangeTF);
    }

    public void ChangeColor(ColorType type)
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
    public bool CheckToProcessAttack()
    {
        if (IsStanding)
        {
            if (CheckAttackableConditions)
            {
                ProcessAttack();
                return true;
            }
        }
        else 
        {
            StopAttack();
        }
        return false;
    }

    private void ProcessAttack()
    {
        ChangeAnimByStatus(StatusType.Attacking);
        attackCoroutine = StartCoroutine(IEAttack());
    }

    protected void StopAttack()
    {
        ChangeAnimByStatus(StatusType.Normal);
        if (attackCoroutine != null) 
        {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator IEAttack()
    {
        atkTargetPos = TF.position + (CurTargetChar.TF.position - TF.position).normalized * CurAttackRange;
        yield return Cache.GetWaitSecs(0.3f);
        if(!IsStatus(StatusType.Dead))
        {
            SoundManager.Instance.PlayThrowWeapon(audioSource, WeaponHolder.CurWeapon.IsReturn);
            WeaponHolder.OnShoot(atkTargetPos);
            ToggleWeapon(false);
            if (BoosterType == BoosterType.Attack)
            {
                DeactiveAtkBooster();
            }
            yield return Cache.GetWaitSecs(.5f);
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
        if (WeaponHolder) 
        {
            WeaponHolder.gameObject.SetActive(value);
        }
    }

    public virtual void OnTargetKilled(Character opponent)
    {
        TargetDetector.RemoveTargetInRange(this, opponent);
        ScaleUp.CheckToProcess(this, opponent.CombatPoint);
        if (indicator) 
        {
            indicator.UpdateCombatPoint(CombatPoint);
        }
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

    protected void ChangeStatus(StatusType type, bool forceChange = false)
    {
        if(!forceChange)
        {
            if (CurStatus == StatusType.Dead)
            {
                return;
            }

            if (CurStatus == StatusType.Untouchable)
            {
                return;
            }
        }

        if (CurStatus != type)
        {
            CurStatus = type;
        }
    }

    public virtual void ChangeAnimByCurStatus()
    {
        switch(CurStatus)
        {
            case StatusType.Untouchable:
            case StatusType.Normal:
                if (IsStanding)
                {
                    ChangeAnim(Const.ANIM_NAME_IDLE);
                }
                else
                {
                    ChangeAnim(Const.ANIM_NAME_RUN);
                }
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

    protected void ChangeAnimByStatus(StatusType type, bool forceChange = false)
    {
        ChangeStatus(type, forceChange);
        ChangeAnimByCurStatus();
    }
    #endregion
    #region Booster
    public bool IsBoosterType(BoosterType type) => BoosterType == type;
    protected virtual void UpdateBoosterStats(Booster booster)
    {
        if (BoosterType != booster.Type && !IsStatus(StatusType.Untouchable))
        {
            DeactiveAtkBooster();
            DeactiveSpeedBooster();
            CancelInvoke(nameof(DeactiveSpeedBooster));
            BoosterType = booster.Type;
            switch (BoosterType)
            {
                case BoosterType.Attack:
                    ActiveAtkBooster(booster);
                    break;
                case BoosterType.Speed:
                    ActiveSpeedBooster(booster);
                    Invoke(nameof(DeactiveSpeedBooster), Booster.SPEED_ACTIVE_SECS);
                    break;
            }
        }
    }

    private void ActiveAtkBooster(Booster booster)
    {
        BoosterAtkRange = booster.AtkRange;
        attackBoosterPartical.Play();
        SetAttackRangeTF(CurAttackRangeTF);
        SoundManager.Instance.PlayAtkBoosterEffect(audioSource);
    }

    private void DeactiveAtkBooster()
    {
        BoosterType = BoosterType.None;
        BoosterAtkRange = 0f;
        SetAttackRangeTF(CurAttackRangeTF);
        attackBoosterPartical.Stop();
    }

    private void ActiveSpeedBooster(Booster booster)
    {
        BoosterSpdMultipler = booster.MoveSpdMultiplier;
        speedBoosterPartical.Play();
    }

    protected virtual void DeactiveSpeedBooster()
    {
        BoosterSpdMultipler = 0f;
        speedBoosterPartical.Stop();
        BoosterType = BoosterType.None;
    }
    #endregion
    public virtual void OnDead(Character killer)
    {
        bulletPartical.Play();
        indicator = null;
        Invoke(nameof(OnDespawn), 2f);
        ChangeAnimByStatus(StatusType.Dead);
        StopMoving();
        ClearTargets();
        ChangeColorDeath(ColorType);
        ToggleTargetIndicator(false);
        DeactiveAtkBooster();
        DeactiveSpeedBooster();
        OnSoulsStolen(killer);
        GameManager.Instance.UpdateAliveCountText();
        SoundManager.Instance.PlayDead(audioSource);
    }

    private void OnSoulsStolen(Character killer)
    {
        for(int i = 0; i< soulList.Count; i++)
        {
            soulList[i].OnStolen(killer);
        }
        soulList.Clear();
    }

    public void TriggerScaleUpVFX(float delay)
    {
        Invoke(nameof(TriggerScaleUpVFX), delay);
    }

    private void TriggerScaleUpVFX()
    {
        if (!IsStatus(StatusType.Dead))
        {
            sizeUpPartical.Play();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character)
        {
            TargetDetector.AddTargetInRange(this, character);
        }

        Booster booster = Cache.GetBooster(other);
        if(booster)
        {
            UpdateBoosterStats(booster);
        }

        Bullet bullet = Cache.GetBullet(other);
        if(bullet && bullet.IsDropped && WeaponHolder.CurWeapon.IsGrab && !WeaponHolder.HasBullet)
        {
            //if(bullet.WeaponPrefab.WeaponType != WeaponHolder.CurWeapon.WeaponType)
            //{
            //    ChangeWeapon(bullet.WeaponPrefab.WeaponType);
            //}
            ChangeWeapon(bullet.WeaponPrefab.WeaponType);
            WeaponHolder.CancelReloadBase();
            bullet.OnDespawn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character) 
        {
            TargetDetector.RemoveTargetInRange(this, character);
        }
    }

    public void CollectSoul(ColorType type)
    {
        Soul soul = SimplePool.Spawn<Soul>(PoolType.Soul, Vector3.zero, Quaternion.identity);
        soul.Init(this, type);
        soulList.Add(soul);
    }

    #region empty virtual methods
    public virtual void ShowCombatPointGainned(int point){}
    public virtual void ToggleTargetIndicator(bool value){}
    public virtual void OnDespawn(){}
    public virtual void StopMoving(){}
    public virtual void InitSpeed(){}
    #endregion
}