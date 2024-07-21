using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    [SerializeField] NavMeshAgent agent;

    private IState curState;

    private Vector3 curDestination;

    private NavMeshHit hit;
    public override bool IsStanding => Vector3.Distance(agent.velocity, Vector3.zero) < 0.1f;

    //public bool IsDestination => Vector3.Distance(curDestination, TF.position + (TF.position.y - curDestination.y) * Vector3.up ) < 0.1f;
    public bool IsDestination => agent.remainingDistance <= 0.1f;

    protected override void Update()
    {
        base.Update();
        if (GameManager.IsState(GameState.GamePlay))
        {
            if (curState != null)
            {
                curState.OnExecute(this);
            }
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        GenerateRandomData();

        SetState(new IdleState());
        ToggleTargetIndicator(false);
    }

    private void GenerateRandomData()
    {
        Name = Ultilities.GetRandomName(Const.ENEMY_NAME_LIST);
        CombatPoint = Random.Range(0, EnemyManager.Instance.RecordHighestPoint + 1);
        SetupSizeByInitCombatPoint(CombatPoint);

        ChangeWeapon((WeaponType)Random.Range(1, Enum.GetNames(typeof(WeaponType)).Length));
        ChangeHead((ItemType)Random.Range(0, 14));
        ChangePants((ItemType)Random.Range(100, 114));
        ChangeShield((ItemType)Random.Range(200, 205));
        ChangeColor((ColorType)Random.Range(1, Enum.GetNames(typeof(ColorType)).Length));

        //ChangeSet(ItemType.None);

        TF.eulerAngles = new Vector3(0, Random.Range(-180f, 181f), 0);
    }

    protected override void SetAttackRangeTF(float atkRange)
    {
        base.SetAttackRangeTF(atkRange);
        atkRangeTF.gameObject.SetActive(true);
    }

    public override void OnDead()
    {
        base.OnDead();
        curState = null;
        curTargetChar = null;
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        EnemyManager.Instance.Despawn(this);
    }

    public void SetRandomDestination(float distance)
    {
        SetDestination(TF.position + new Vector3(Random.Range(-distance, distance), 0, Random.Range(-distance, distance)));
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
        //if(curState != state)
        //{
        //    if (curState != null) {
        //        curState.OnExit(this);
        //    }
        //    curState = state;
        //    curState.OnEnter(this);
        //}
        curState?.OnExit(this);
        curState = state;
        curState?.OnEnter(this);
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

    protected override void UpdateBonusStatsFromItem(Item item)
    {
        base.UpdateBonusStatsFromItem(item);
        agent.speed += agent.speed * item.BonusMoveSpeed * 0.01f;
    }
}
