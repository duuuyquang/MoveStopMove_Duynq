using UnityEngine;

public class Player : Character
{
    [SerializeField] private Transform atkRangeTF;
    protected override void OnInit()
    {
        base.OnInit();
        InitAttackRangeUI();
    }

    public void InitAttackRangeUI()
    {
        atkRangeTF.localScale = new Vector3(initAtkRange, 0.1f, initAtkRange);
    }

    protected override void Update()
    {
        base.Update();
        if (GameManager.IsState(GameState.GamePlay))
        {
            ListenControllerInput();
            UpdateAttackStatus();
            UpdateAnim();
        }
    }

    void FixedUpdate()
    {
        ProcessMoving();
    }

    private void ProcessMoving()
    {
        rb.velocity = PlayerController.Instance.CurDir * speed * Time.fixedDeltaTime;
    }

    private void LookAtCurDirection()
    {
        if (Vector3.Distance(rb.velocity, Vector3.zero) >= 0.1f)
        {
            TF.LookAt(TF.position + PlayerController.Instance.CurDir);
        }
    }

    private void LookAtTarget()
    {
        if (curTargetChar != null)
        {
            // we don't want player look down when his size scales bigger
            Vector3 targetDir = new Vector3(curTargetChar.TF.position.x, TF.position.y, curTargetChar.TF.position.z);
            TF.LookAt(targetDir);
        }
    }

    private void ListenControllerInput()
    {
        PlayerController.Instance.SetCurDirection();
        LookAtCurDirection();
    }

    private void UpdateAnim()
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

    private void UpdateAttackStatus()
    {
        if (IsStanding)
        {
            LookAtTarget();
            ProcessAttack();
        }
        else
        {
            StopAttack();
        }
    }
}
