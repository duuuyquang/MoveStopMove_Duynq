using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    [SerializeField] NavMeshAgent agent;

    private List<string> nameList = new List<string>() {"Dazzle","Crystal","Lina","Clink","Axe","Phantom","Sniper","TrollWarlord","BrewMaster","Hoodwink","Winranger","Traxex","Enchantress","Luna"};

    private IState curState;

    private Vector3 curDestination;

    private NavMeshHit hit;
    public override bool IsStanding => Vector3.Distance(agent.velocity, Vector3.zero) < 0.1f;

    //public bool IsDestination => Vector3.Distance(curDestination, TF.position + (TF.position.y - curDestination.y) * Vector3.up ) < 0.3f;
    public bool IsDestination => agent.remainingDistance <= 0.1f;

    protected override void Update()
    {
        base.Update();
        if (GameManager.IsState(GameState.GamePlay))
        {
            if (curState != null && !IsStatus(StatusType.Dead))
            {
                curState.OnExecute(this);
            }
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        ChangeWeapon((WeaponType)Random.Range(1, Enum.GetNames(typeof(WeaponType)).Length));
        ChangeHead((ItemType)Random.Range(0, 11));
        ChangePants((ItemType)Random.Range(100, 110));
        ChangeShield((ItemType)Random.Range(200, 203));
        ChangeColor((ColorType)Random.Range(1, Enum.GetNames(typeof(ColorType)).Length));
        SetState(new IdleState());
        ChangeTargetIndicatorColor(ColorType);
        ToggleTargetIndicator(false);
    }

    public override void ChangePants(ItemType type)
    {
        base.ChangePants(type);
        agent.speed += agent.speed * curPants.BonusMoveSpeed * 0.01f;
    }

    private void ChangeTargetIndicatorColor(ColorType type)
    {
        //targetIndicatorImage.color = colorDataSO.GetColor(type);
        targetIndicatorImage.color = Color.black;
    }

    protected override void InitBasicStats()
    {
        base.InitBasicStats();
        CombatPoint = Random.Range(0, EnemyManager.Instance.RecordHighestPoint + 1);
        Name = GetRandomName();
    }

    protected override void SetAttackRangeTF(float atkRange)
    {
        base.SetAttackRangeTF(atkRange);
        atkRangeTF.gameObject.SetActive(true);
    }

    private string GetRandomName()
    {
        return nameList[Random.Range(0, nameList.Count)];
    }

    public override void OnDespawn()
    {
        Name = "";
        curTargetChar = null;
        EnemyManager.Instance.Despawn(this);
        //base.OnDespawn();
    }

    public void SetRandomDestination(float distance)
    {
        float randomX = Random.Range(-distance, distance);
        float randomZ = Random.Range(-distance, distance);
        SetDestination(TF.position + new Vector3(randomX, 0, randomZ));
    }

    public void SetDestination(Vector3 destination)
    {
        curDestination = destination;
        if(agent.isOnNavMesh)
        {
            agent.SetDestination(destination);
        }
    }

    public void SetState(IState state)
    {
        if(curState != state)
        {
            if (curState != null) {
                curState.OnExit(this);
            }
            curState = state;
            curState.OnEnter(this);
        }
    }

    public override void StopMoving()
    {
        base.StopMoving();
        if(agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
    }

    public override void ToggleTargetIndicator(bool value)
    {
        targetIndicatorImage.enabled = value;
    }
}
