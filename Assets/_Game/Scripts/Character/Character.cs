using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected const string ANIM_NAME_IDLE     = "idle";
    protected const string ANIM_NAME_RUN      = "run";
    protected const string ANIM_NAME_DANCE    = "dance";
    protected const string ANIM_NAME_ATTACK   = "attack";
    protected const string ANIM_NAME_DIE      = "die";

    public const float UPSCALE_UNIT = 0.1f;

    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Weapon curWeapon;
    [SerializeField] protected float initAtkRange;
    [SerializeField] protected float speed;

    private string curAnim = ANIM_NAME_IDLE;
    [SerializeField] protected WeaponType weaponType;

    protected float curSize;
    protected Character curTargetChar = null;
    protected List<Character> targetsInRange = new List<Character>();
    protected bool IsAttacking { get; set; } = false;
    private bool HasTargetInRange => curTargetChar != null;
    public bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;

    public float CurAttackRange { get { return initAtkRange * curSize; } }
    public float CurSize { get { return curSize; } }
    public WeaponType WeaponType { get { return weaponType; } }

    public ItemDataSO itemDataSO;

    public Transform TF;

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
        ChangeAnim(ANIM_NAME_IDLE);
    }

    public virtual void OnDespawn()
    {
        Destroy(TF.gameObject);
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
        curSize = 1;
        TF.localScale = new Vector3(1, 1, 1);
    }

    public void SetSizeBigger()
    {
        curSize += UPSCALE_UNIT;
        TF.localScale = Vector3.one * curSize;
        TF.position += Vector3.up * UPSCALE_UNIT;
    }

    private void DetectNearestTarget()
    {
        float curDis = 0;

        if (targetsInRange.Count > 0)
        {
            for (int i = 0; i < targetsInRange.Count; i++)
            {
                targetsInRange[i].ToggleIndicator(false);
                float dist = Vector3.Distance(targetsInRange[i].TF.position, TF.position);
                if (curDis == 0)
                {
                    curDis = dist;
                    curTargetChar = targetsInRange[i];
                }

                if (dist < curDis)
                {
                    curDis = dist;
                    curTargetChar = targetsInRange[i];
                }
            }
            curTargetChar.ToggleIndicator(true);
        }
        else
        {
            curTargetChar = null;
        }
    }

    protected void ProcessAttack()
    {
        if(curWeapon.HasBullet && HasTargetInRange && !IsAttacking)
        {
            IsAttacking = true;
            StartCoroutine(IEAttack());
        }
    }

    protected void StopAttack()
    {
        IsAttacking = false;
        StopAllCoroutines();
        //StopCoroutine(IEAttack());
    }

    IEnumerator IEAttack()
    {
        ChangeAnim(ANIM_NAME_ATTACK);

        yield return new WaitForSeconds(0.8f);

        if (IsAttacking)
        {
            ToggleWeapon(false);
            curWeapon.Throw(new Vector3(TF.position.x, 0, TF.position.z) + (curTargetChar.TF.position - TF.position).normalized * CurAttackRange / 2);
            curWeapon.BulletCharge--;
        }

        IsAttacking = false;
    }

    public void ToggleWeapon(bool value)
    {
        curWeapon.gameObject.SetActive(value);
    }

    public virtual void ToggleIndicator(bool value)
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Character character = Cache.GetChars(other);
            character.ToggleIndicator(true);
            targetsInRange.Add(character);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Character character = Cache.GetChars(other);
            character.ToggleIndicator(false);
            targetsInRange.Remove(character);
        }
    }

}
