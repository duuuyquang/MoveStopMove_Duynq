using System;
using UnityEngine;

public class Player : Character
{
    [SerializeField] CombatPointText combatPointTextPrefab; 
    protected override void Update()
    {
        base.Update();
        if (!IsStatus(StatusType.Dead))
        {
            if (GameManager.IsState(GameState.GamePlay))
            {
                ListenControllerInput();
                UpdateAttackStatus();
            }

            if (GameManager.IsState(GameState.Finish))
            {
                OnWinning();
            }
        }
    }

    void FixedUpdate()
    {
        if (!IsStatus(StatusType.Dead))
        {
            ProcessMoving();
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        InitTransform();
        PlayerController.Instance.OnInit();
    }

    private void OnWinning()
    {
        TF.eulerAngles = new Vector3(0, 200, 0);
        ChangeAnim(Const.ANIM_NAME_DANCE);
        StopMoving();
        ToggleAtkRangeTF(false);
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

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        if (IsStatus(StatusType.Normal))
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

    protected override void ShowCombatPointGainned(int point)
    {
        CombatPointText prefab = Instantiate(combatPointTextPrefab, UIManager.Instance.GetUI<CanvasGamePlay>().transform);
        prefab.transform.position = CameraFollower.Instance.Camera.WorldToScreenPoint(TF.position) + Vector3.up * 200f;
        prefab.SetPoint(point, Const.COMBAT_POINT_DEFAULT_SIZE * CurSize);
    }
}
