using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] public Transform TF;
    [SerializeField] protected Transform avatarTf;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Weapon curWeapon;
    [SerializeField] protected float atkRange;
    [SerializeField] protected float speed;

    private string curAnim;
    protected Character curTargetChar = null;
    protected List<Character> targetsInRange = new List<Character>();

    protected int bulletCharge;
    public bool HasBullet => bulletCharge > 0;
    public bool HasTargetInRange => curTargetChar != null;

    public bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;

    public void Start()
    {
        OnInit();
    }

    protected virtual void Update()
    {
    }

    protected virtual void OnInit()
    {
        bulletCharge = 1;
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

    protected void ProcessAttack()
    {
        if(HasBullet && HasTargetInRange)
        {
            curWeapon.Throw(TF.position + (curTargetChar.TF.position - TF.position).normalized * atkRange/2);
            bulletCharge--;
        }
    }

    public void ReloadBullet(int chargeNum)
    {
        bulletCharge += chargeNum;
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
