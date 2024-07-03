using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    public const int SIZE_SCALE_UP_THRESHOLD_1 = 2;
    public const int SIZE_SCALE_UP_THRESHOLD_2 = 6;
    public const int SIZE_SCALE_UP_THRESHOLD_3 = 12;
    public const int SIZE_SCALE_UP_THRESHOLD_4 = 24;
    public const int SIZE_SCALE_UP_THRESHOLD_5 = 48;

    //------------------ Movement props ---------------------
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float speed;
    private string curAnim = Const.ANIM_NAME_IDLE;
    public Transform TF;

    public virtual bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;

    //------------------ Basic props -------------------------
    private string charName;
    public string Name { get { return charName; } set { if (value.Length < 15) { charName = value; } } }

    [SerializeField] protected Renderer charRenderer;
    private ColorType colorType;
    public ColorType ColorType { get { return colorType; } set { colorType = value; } }

    //------------------ Combat props ------------------------
    protected int[] upSizeThresholds = { 1, 6, 16, 31, 51, 76 }; // kill 5 enemies same scale to up scale

    private int combatPoint;
    public int CombatPoint { get { return combatPoint; } set { combatPoint = Mathf.Max(0, value); } }

    private IEnumerator attackCoroutine;

    [SerializeField] protected Weapon curWeapon;
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected Transform atkRangeTF;
    [SerializeField] protected float baseAtkRange;
    [SerializeField] protected float baseAtkSpeed;
    protected float curSize;

    public float CurSize { get { return curSize; } }
    public float CurAttackRange { get { return baseAtkRange * curSize; } }
    public float BaseAttackSpeed { get { return baseAtkSpeed; } }
    public WeaponType WeaponType { get { return weaponType; } set { weaponType = value; } }

    //------------------ Navigation props --------------------
    [SerializeField] protected Canvas targetIndicator;
    [SerializeField] Indicator indicatorPrefab;
    protected Indicator indicator;

    protected List<Character> targetsInRange = new List<Character>();
    protected Character curTargetChar = null;
    public bool HasTargetInRange => curTargetChar != null;

    //------------------ Status props ------------------------
    protected bool isAttacking = false;
    public bool IsAttacking { get { return isAttacking; } }

    protected bool isDead = false;
    public bool IsDead { get { return isDead; } }

    //------------------ Data props --------------------------
    public ItemDataSO itemDataSO;
    public ColorDataSO colorDataSO;

    [SerializeField] ParticleSystem bulletPartical;
    [SerializeField] ParticleSystem sizeUpPartical;

    protected virtual void Start()
    {
        OnInit();
    }

    protected virtual void Update()
    {
        if (GameManager.IsState(GameState.GamePlay) && !isDead)
        {
            DetectNearestTarget();
            UpdateMovementAnim();
        }
    }

    public virtual void OnInit()
    {
        InitBasicStats();
        InitDependentFactors();
        InitInDependentFactors();
    }

    protected virtual void InitBasicStats()
    {
        InitAttackRangeTF();
    }

    private void InitDependentFactors()
    {
        InitWeapon();
        InitIndicator();
        ChangeColor(ColorType);
        SetupSizeByInitCombatPoint(CombatPoint);
    }

    private void InitInDependentFactors()
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
    }

    private void InitSize()
    {
        curSize = 1f;
        TF.localScale = Vector3.one;
    }

    protected void SetupSizeByInitCombatPoint(int initPoint)
    {
        InitSize();
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
        isDead = false;
        isAttacking = false;
    }

    protected void InitAttackRangeTF()
    {
        atkRangeTF.localScale = new Vector3(baseAtkRange, 0.1f, baseAtkRange);
        atkRangeTF.gameObject.SetActive(true);
    }

    protected void ToggleAtkRange(bool value)
    {
        atkRangeTF.gameObject.SetActive(value);
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

    public void ChangeAnim(string animName)
    {
        if (curAnim != animName)
        {
            animator.ResetTrigger(curAnim);
            curAnim = animName;
            animator.SetTrigger(animName);
        }
    }

    protected virtual void DetectNearestTarget()
    {
        float nearestDist = 0;
        curTargetChar = null;
        foreach (Character enemy in targetsInRange) {
            if (enemy.IsDead)
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
        if (curWeapon.HasBullet && HasTargetInRange && !IsAttacking)
        {
            ProcessAttack();
        }
    }

    private void ProcessAttack()
    {
        isAttacking = true;
        attackCoroutine = IEAttack();
        StartCoroutine(attackCoroutine);
    }

    protected void StopAttack()
    {
        isAttacking = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }
    private void ProcessDie()
    {
        Invoke(nameof(OnDespawn), 2f);
        isDead = true;
        bulletPartical.Play();
        ToggleTargetIndicator(false);
        StopMoving();
        ChangeAnim(Const.ANIM_NAME_DIE);
        GameManager.Instance.UpdateAliveNumText();
    }

    IEnumerator IEAttack()
    {
        Vector3 targetPos = TF.position + (curTargetChar.TF.position - TF.position).normalized * CurAttackRange * 0.5f;
        LookAtTarget(targetPos);
        ChangeAnim(Const.ANIM_NAME_ATTACK);
        yield return new WaitForSeconds(0.3f);
        if(!IsDead)
        {
            LookAtTarget(targetPos);
            ToggleWeapon(false);
            curWeapon.Throw(targetPos);
            curWeapon.BulletCharge--;
        }
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }

    protected void LookAtTarget(Vector3 targetPos)
    {
        // we don't want character looks down when its size scales bigger
        Vector3 targetDir = new Vector3(targetPos.x, TF.position.y, targetPos.z);
        TF.LookAt(targetDir);
    }

    protected virtual void UpdateMovementAnim()
    {
        if (IsStanding && !IsAttacking)
        {
            ChangeAnim(Const.ANIM_NAME_IDLE);
        }
        if (!IsStanding && !IsAttacking)
        {
            ChangeAnim(Const.ANIM_NAME_RUN);
        }
    }

    public void ToggleWeapon(bool value)
    {
        if(curWeapon)
        {
            curWeapon.gameObject.SetActive(value);
        }
    }

    public virtual void ToggleTargetIndicator(bool value)
    {
    }

    public virtual void StopMoving()
    {
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        bool isCharacter = other.CompareTag(Const.TAG_NAME_ENEMY) || other.CompareTag(Const.TAG_NAME_PLAYER);
        if (isCharacter)
        {
            Character character = Cache.GetChar(other);
            if(!character.isDead)
            {
                targetsInRange.Add(character);
            }
        }
        
        if (other.CompareTag(Const.TAG_NAME_BULLET) && !isDead)
        {
            Bullet bullet = Cache.GetBullet(other);
            if(bullet && this != bullet.Weapon.Owner && !bullet.Weapon.Owner.isDead)
            {
                bullet.Weapon.Owner.RemoveTargetInRange(this);
                bullet.Weapon.Owner.CheckToScaleSizeUp(CombatPoint);
                ProcessDie();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        bool isCharacter = other.CompareTag(Const.TAG_NAME_ENEMY) || other.CompareTag(Const.TAG_NAME_PLAYER);
        if (isCharacter)
        {
            Character character = Cache.GetChar(other);
            targetsInRange.Remove(character);
            character.ToggleTargetIndicator(false);
        }
    }

    public void CheckToScaleSizeUp(int opponentCombatPoint)
    {
        bool onPointToScale = false;
        int oldCombatPoint = CombatPoint;
        CombatPoint += GetCombatPointInReturn(opponentCombatPoint);
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
            StartCoroutine(IEScaleUp());
            //Invoke(nameof(TriggerScaleUpVFX), 0.1f);
        }
    }

    IEnumerator IEScaleUp()
    {
        if (!IsDead)
        {
            sizeUpPartical.Play();
            yield return new WaitForSeconds(0.3f);
            sizeUpPartical.Stop();
        }
    }

    private void TriggerScaleUpVFX()
    {
        if (!IsDead)
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

}
