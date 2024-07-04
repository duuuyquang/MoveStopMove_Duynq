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
        SetState(new IdleState());
        ToggleTargetIndicator(false);
    }

    protected override void InitBasicStats()
    {
        base.InitBasicStats();
        Name = GetRandomName();
        CombatPoint = Random.Range(0, GameManager.Instance.RecordHighestPoint + 1);
        ColorType = (ColorType)Random.Range(1, Enum.GetNames(typeof(ColorType)).Length);
        WeaponType = (WeaponType)Random.Range(1, Enum.GetNames(typeof(WeaponType)).Length);
        //WeaponType = WeaponType.Axe;
    }

    private string GetRandomName()
    {
        return nameList[Random.Range(0, nameList.Count)];
    }

    public override void OnDespawn()
    {
        EnemyManager.Instance.Despawn(this);
        base.OnDespawn();
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

    public override void SetState(IState state)
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
        targetIndicator.enabled = value;
    }
}
