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
    protected GameObject curTargetObject = null;

    protected int bulletCharge;
    public bool HasBullet => bulletCharge > 0;
    public bool HasTargetInRange => curTargetObject != null;

    public bool IsStanding => Vector3.Distance(rb.velocity, Vector3.zero) < 0.1f;

    public void Start()
    {
        OnInit();
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

    protected void ThrowWeapon()
    {
        if (HasBullet && HasTargetInRange)
        {
            curWeapon.Throw(curTargetObject.transform.position - TF.position);
            bulletCharge--;
        }
    }

    public void ReloadBullet(int chargeNum)
    {
        bulletCharge += chargeNum;
    }

}
