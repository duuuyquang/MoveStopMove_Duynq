using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    //------------------ Movement props ---------------------
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float speed;
    private string curAnim = Const.ANIM_NAME_IDLE;
    public Transform TF;
    public virtual bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;

    //------------------ Combat props ------------------------
    private IEnumerator attackCoroutine;
    [SerializeField] protected Weapon curWeapon;
    [SerializeField] protected float initAtkRange;
    [SerializeField] protected WeaponType weaponType;
    [SerializeField] protected Transform atkRangeTF;
    protected float curSize;
    public float CurSize { get { return curSize; } }
    public float CurAttackRange { get { return initAtkRange * curSize; } }
    public WeaponType WeaponType { get { return weaponType; } }

    //------------------ Navigation props --------------------
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

    [SerializeField] ParticleSystem BulletPartical;

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
        InitSize();
        InitWeapon();
        InitAttackRangeTF();
        ChangeAnim(Const.ANIM_NAME_IDLE);
        ToggleWeapon(true);
        isDead = false;
        isAttacking = false;
        targetsInRange.Clear();
        curTargetChar = null;
    }


    protected void InitWeapon()
    {
        curWeapon.OnInit(this, 1);
    }

    protected void InitSize()
    {
        curSize = 1f;
        TF.localScale = Vector3.one;
    }
    protected void InitAttackRangeTF()
    {
        atkRangeTF.localScale = new Vector3(initAtkRange, 0.1f, initAtkRange);
    }

    public virtual void OnDespawn()
    {
        if(gameObject)
        {
            Destroy(gameObject);
        }
    }

    public void ScaleSizeUp()
    {
        curSize += Const.CHARACTER_UPSCALE_UNIT;
        TF.localScale = Vector3.one * curSize;
        TF.position += Vector3.up * Const.CHARACTER_UPSCALE_UNIT;
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
        character.ToggleIndicator(false);
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
        BulletPartical.Play();
        ToggleIndicator(false);
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
        isAttacking = false;
    }

    protected void LookAtTarget(Vector3 targetPos)
    {
        //if (curTargetChar != null)
        //{
            // we don't want player looks down when his size scales bigger
            Vector3 targetDir = new Vector3(targetPos.x, TF.position.y, targetPos.z);
            TF.LookAt(targetDir);
        //}
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

    public virtual void ToggleIndicator(bool value)
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
            if(bullet && this != bullet.Weapon.Owner)
            {
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
            character.ToggleIndicator(false);
        }
    }

}
