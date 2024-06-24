using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Transform atkRangeTf;

    public void SetupAttackRange()
    {
        atkRangeTf.transform.localScale = new Vector3(atkRange, 0.1f, atkRange);
    }

    protected override void OnInit()
    {
        base.OnInit();
        SetupAttackRange();
    }

    void Update()
    {
        ListenControllerInput();
        if (IsStanding) {
            Invoke(nameof(ThrowWeapon), 0.5f);
        }
    }

    void FixedUpdate()
    {
        ProcessMoving();
    }

    private void ProcessMoving()
    {
        Vector3 curDir = PlayerController.Instance.CurDir;
        rb.velocity = curDir * speed * Time.fixedDeltaTime;
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

    public void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            curTargetObject = other.gameObject;
            curTargetObject.GetComponent<Enemy>().ToggleIndicator(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            curTargetObject = null;
            other.gameObject.GetComponent<Enemy>().ToggleIndicator(false);
        }
    }
}
