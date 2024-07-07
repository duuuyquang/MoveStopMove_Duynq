using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusType { Normal, Attacking, Dead }

public class Character : MonoBehaviour
{
    //------------------ Movement props ---------------------
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float speed;
    private string curAnim = Const.ANIM_NAME_IDLE;
    public Transform TF;

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

    [SerializeField] protected WeaponHolder curWeapon;
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected Transform atkRangeTF;
    [SerializeField] protected float baseAtkRange;
    [SerializeField] protected float baseAtkSpeed;
    protected float curSize;
    protected float bonusAtkRange;

    public WeaponHolder CurWeapon => curWeapon;
    public float CurSize => curSize;
    public float CurAttackRange => (baseAtkRange + bonusAtkRange) * curSize;
    public float BaseAttackSpeed => baseAtkSpeed;
    public float BonusAttackRange { get { return bonusAtkRange; } set { bonusAtkRange = Mathf.Max(value,0); } }
    public WeaponType WeaponType { get { return weaponType; } set { weaponType = value; } }

    //------------------ Navigation props --------------------
    [SerializeField] protected Image targetIndicatorImage;
    [SerializeField] Indicator indicatorPrefab;
    protected Indicator indicator;

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
            if(IsStatus(StatusType.Normal))
            {
                DetectNearestTarget();
            }
            UpdateAnimation();
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
        InitWeapon();
        InitIndicator();
        ChangeColor(ColorType);
    }

    private void InitIndependentFactors()
    {
        InitStatus();
        ClearTargets();
        ChangeAnim(Const.ANIM_NAME_IDLE);
    }

    protected virtual void InitIndicator()
    {
        indicator = Instantiate(indicatorPrefab, UIManager.Instance.GetUI<CanvasGamePlay>().transform);
        indicator.OnInit(this);
    }

    protected virtual void InitWeapon()
    {
        curWeapon.OnInit(this);
        ToggleWeapon(true);
        SetAttackRangeTF(baseAtkRange + bonusAtkRange);
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

    protected void SetAttackRangeTF(float atkRange)
    {
        atkRangeTF.localScale = new Vector3(atkRange * 2f, 0.1f, atkRange * 2f);
        atkRangeTF.gameObject.SetActive(true);
    }

    public virtual void OnDespawn()
    {
        if(gameObject)
        {
            Destroy(gameObject);
        }
    }

    protected void ChangeColor(ColorType type)
    {
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
            if (enemy.IsStatus(StatusType.Dead))
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
        if (curWeapon.HasBullet && HasTargetInRange && !IsStatus(StatusType.Attacking))
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
            curWeapon.OnShoot(targetPos);
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

    private void ProcessDie()
    {
        Invoke(nameof(OnDespawn), 2f);
        ChangeStatus(StatusType.Dead);
        ChangeColorDeath(ColorType);
        ToggleTargetIndicator(false);
        StopMoving();
        bulletPartical.Play();
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
        if(curWeapon)
        {
            curWeapon.gameObject.SetActive(value);
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
        if(bullet && this != bullet.Weapon.Owner && !bullet.IsDropped && !IsStatus(StatusType.Dead))
        {
            ProcessDie();
            bullet.Weapon.Owner.RemoveTargetInRange(this);
            bullet.Weapon.Owner.CheckToScaleSizeUp(CombatPoint);
            EnemyManager.Instance.SetRecordHighestPoint(bullet.Weapon.Owner.CombatPoint);
        }

    }

    public void OnTriggerExit(Collider other)
    {
        Character character = Cache.GetChar(other);
        if (character)
        {
            targetsInRange.Remove(character);
            character.ToggleTargetIndicator(false);
        }
    }

    public void CheckToScaleSizeUp(int opponentCombatPoint)
    {
        bool onPointToScale = false;
        int oldCombatPoint = CombatPoint;
        int gainnedPoint = GetCombatPointInReturn(opponentCombatPoint);
        ShowCombatPointGainned(gainnedPoint);
        CombatPoint += gainnedPoint;
        indicator.UpdateCombatPoint(CombatPoint);

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
            Invoke(nameof(TriggerScaleUpVFX), 0.1f);
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
