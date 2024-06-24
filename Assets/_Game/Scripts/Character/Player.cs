using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Transform atkRangeTf;

    public void InitAttackRange()
    {
        atkRangeTf.localScale = new Vector3(atkRange, 0.1f, atkRange);
    }

    protected override void OnInit()
    {
        base.OnInit();
        InitAttackRange();
    }

    protected override void Update()
    {
        DetectNearestTarget();
        ListenControllerInput();
    }

    void FixedUpdate()
    {
        ProcessMoving();
    }

    private void OnMouseUp()
    {
        if(GameManager.IsState(GameState.GamePlay))
        {
            ProcessAttack();
        }
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

    private void ListenControllerInput()
    {
        PlayerController.Instance.SetCurDirection();
        LookAtCurDirection();
    }
}
