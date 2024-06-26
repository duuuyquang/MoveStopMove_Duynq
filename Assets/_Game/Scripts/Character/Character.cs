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
    public bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;

    //------------------ Combat props ------------------------
    private IEnumerator attackCoroutine;
    [SerializeField] protected Weapon curWeapon;
    [SerializeField] protected float initAtkRange;
    [SerializeField] protected WeaponType weaponType;
    protected float curSize;
    public float CurSize { get { return curSize; } }
    public float CurAttackRange { get { return initAtkRange * curSize; } }
    public WeaponType WeaponType { get { return weaponType; } }

    //------------------ Navigation props --------------------
    protected List<Character> targetsInRange = new List<Character>();
    protected Character curTargetChar = null;
    public bool HasTargetInRange => curTargetChar != null;

    //------------------ Status props ------------------------
    protected bool IsAttacking { get; set; } = false;
    protected bool IsDie { get; set; } = false;

    //------------------ Data props --------------------------
    public ItemDataSO itemDataSO;

    public void Start()
    {
        OnInit();
    }

    protected virtual void Update()
    {
        if (GameManager.IsState(GameState.GamePlay))
        {
            DetectNearestTarget();
        }
    }

    protected virtual void OnInit()
    {
        InitSize();
        InitWeapon();
        ChangeAnim(Const.ANIM_NAME_IDLE);
    }

    public virtual void OnDespawn()
    {
        Destroy(gameObject);
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

    protected void InitWeapon()
    {
        curWeapon.OnInit(this, 1);
    }

    protected void InitSize()
    {
        curSize = 1f;
        TF.localScale = Vector3.one;
    }

    public void SetSizeBigger()
    {
        curSize += Const.CHARACTER_UPSCALE_UNIT;
        TF.localScale = Vector3.one * curSize;
        TF.position += Vector3.up * Const.CHARACTER_UPSCALE_UNIT;
    }

    private void DetectNearestTarget()
    {
        float nearestDist = 0;
        curTargetChar = null;
        foreach (Character enemy in targetsInRange) {
            enemy.ToggleIndicator(false);
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

        if (curTargetChar != null)
        {
            curTargetChar.ToggleIndicator(true);
        }
    }

    public void RemoveTargetInRange(Character character)
    {
        character.ToggleIndicator(false);
        targetsInRange.Remove(character);
    }

    protected void ProcessAttack()
    {
        if (curWeapon.HasBullet && HasTargetInRange && !IsAttacking)
        {
            IsAttacking = true;
            attackCoroutine = IEAttack();
            StartCoroutine(attackCoroutine);
        }
    }

    protected void StopAttack()
    {
        IsAttacking = false;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
    }

    IEnumerator IEAttack()
    {
        ChangeAnim(Const.ANIM_NAME_ATTACK);

        yield return new WaitForSeconds(0.7f);

        ToggleWeapon(false);
        curWeapon.Throw(TF.position + (curTargetChar.TF.position - TF.position).normalized * CurAttackRange / 2);
        curWeapon.BulletCharge--;
        IsAttacking = false;
    }

    public void ToggleWeapon(bool value)
    {
        curWeapon.gameObject.SetActive(value);
    }

    public virtual void ToggleIndicator(bool value)
    {
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        bool isCharacter = other.CompareTag(Const.TAG_NAME_ENEMY) || other.CompareTag(Const.TAG_NAME_PLAYER);
        if (isCharacter)
        {
            Character character = Cache.GetChars(other);
            if(!character.IsDie)
            {
                character.ToggleIndicator(true);
                targetsInRange.Add(character);
            }
        }
        
        if (other.CompareTag(Const.TAG_NAME_BULLET) && !IsDie)
        {
            IsDie = true;
            ChangeAnim(Const.ANIM_NAME_DIE);
            Invoke(nameof(OnDespawn), 2f);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        bool isCharacter = other.CompareTag(Const.TAG_NAME_ENEMY) || other.CompareTag(Const.TAG_NAME_PLAYER);
        if (isCharacter)
        {
            Character character = Cache.GetChars(other);
            character.ToggleIndicator(false);
            targetsInRange.Remove(character);
        }
    }

}
