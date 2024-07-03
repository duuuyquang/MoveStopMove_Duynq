using System;
using UnityEngine;

public class Player : Character
{
    protected override void Update()
    {
        base.Update();
        if (GameManager.IsState(GameState.GamePlay) && !isDead)
        {
            ListenControllerInput();
            UpdateAttackStatus();
        }
        if (GameManager.IsState(GameState.Finish) && !isDead)
        {
            SetWinningStatus();
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            ProcessMoving();
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        InitTransform();
        InitIndicator();
        PlayerController.Instance.OnInit();
    }

    private void InitTransform()
    {
        TF.position = Vector3.zero;
        TF.eulerAngles = new Vector3(0, 210, 0);
    }

    protected override void InitBasicStats()
    {
        base.InitBasicStats();
        Name = "AbcXz";
        CombatPoint = 0;
        ColorType = (ColorType) UnityEngine.Random.Range(1, Enum.GetNames(typeof(ColorType)).Length);
        WeaponType = (WeaponType)UnityEngine.Random.Range(1, Enum.GetNames(typeof(WeaponType)).Length);
    }

    public override void OnDespawn()
    {
        GameManager.ChangeState(GameState.Finish);
        UIManager.Instance.OpenUI<CanvasLose>();
    }

    protected override void DetectNearestTarget()
    {
        base.DetectNearestTarget();
        foreach (Character enemy in targetsInRange)
        {
            if(enemy)
            {
                enemy.ToggleTargetIndicator(false);
            }
        }
        
        if (curTargetChar != null)
        {
            curTargetChar.ToggleTargetIndicator(true);
        }
    }

    protected override void UpdateMovementAnim()
    {
        base.UpdateMovementAnim();
        if (!IsAttacking)
        {
            LookAtCurDirection();
        }
    }

    private void LookAtCurDirection()
    {
        if (!IsStanding)
        {
            TF.LookAt(TF.position + PlayerController.Instance.CurDir);
        }
    }

    private void ProcessMoving()
    {
        rb.velocity = PlayerController.Instance.CurDir * speed * Time.fixedDeltaTime;
    }

    private void ListenControllerInput()
    {
        PlayerController.Instance.SetCurDirection();
    }

    private void UpdateAttackStatus()
    {
        if (IsStanding)
        {
            CheckAndProcessAttack();
        }
        else
        {
            StopAttack();
        }
    }

    public override void StopMoving()
    {
        base.StopMoving();
        rb.velocity = Vector3.zero;
        PlayerController.Instance.CurDir = Vector3.zero;
    }

    public void SetWinningStatus()
    {
        TF.eulerAngles = new Vector3(0, 200, 0);
        ChangeAnim(Const.ANIM_NAME_DANCE);
        StopMoving();
        ToggleAtkRange(false);
    }
}
