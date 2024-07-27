using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    [SerializeField] protected Image targetIndicatorImage;
    [SerializeField] NavMeshAgent agent;

    private IState curState;
    public override bool IsStanding => Vector3.Distance(agent.velocity, Vector3.zero) < 0.1f;
    public bool IsDestination => agent.remainingDistance < 0.1f;

    private float baseAgentMoveSpeed = Const.CHARACTER_AGENT_MOVE_SPD_DEFAULT;
    private float bonusAgentMoveSpeed = 0f;
    public float AgentMoveSpeed => Mathf.Min(Const.CHARACTER_AGENT_MOVE_SPD_MAX, (baseAgentMoveSpeed + baseAgentMoveSpeed*BoosterSpdMultipler + bonusAgentMoveSpeed) * CurSize);

    //private Vector3 curDestination;
    //public bool IsDestination => Vector3.Distance(curDestination, TF.position + (TF.position.y - curDestination.y) * Vector3.up ) < 0.1f;

    #region StateMachine properties
    public float    StateDelayTime { get; set; }
    public float    StateCounter { get; set; }
    public bool     StateIsAttacked { get; set; }
    public Vector3  StateTargetPos { get; set; }
    #endregion

    protected override void Update()
    {
        base.Update();
        if (GameManager.IsState(GameState.GamePlay))
        {
            curState?.OnExecute(this);
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        InitSpeed();
        GenerateRandomData();
        SetState(new IdleState());
        ToggleTargetIndicator(false);
    }

    private bool RollSetItem => Random.Range(0, 10) < 3;

    private void GenerateRandomData()
    {
        TF.eulerAngles = new Vector3(0, Random.Range(-180f, 181f), 0);
        Name = Ultilities.GetRandomValue(EnemyManager.NAMES);
        CombatPoint = Random.Range(0, EnemyManager.Instance.RecordHighestPoint + 1);
        ScaleUp.ProcessByInitCombatPoint(this,CombatPoint);

        ChangeWeapon((WeaponType)Random.Range(1, Enum.GetNames(typeof(WeaponType)).Length));

        ChangeSet(ItemType.None);
        if (RollSetItem)
        {
            ChangeSet(Ultilities.GetRandomValue(EnemyManager.SET_ITEM_TYPES));
        }
        else
        {
            ChangeHead(Ultilities.GetRandomValue(EnemyManager.HEAD_ITEM_TYPES));
            ChangePants(Ultilities.GetRandomValue(EnemyManager.PANTS_ITEM_TYPES));
            ChangeShield(Ultilities.GetRandomValue(EnemyManager.SHIELD_ITEM_TYPES));
            ChangeColor((ColorType)Random.Range(1, Enum.GetNames(typeof(ColorType)).Length));
        }
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
        CurTargetChar = null;
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        EnemyManager.Instance.Despawn(this);
    }

    public void SetRandomDestination(float distance)
    {
        SetDestination(TF.position + new Vector3(Random.Range(-distance, distance), 0f, Random.Range(-distance, distance)));
    }

    public void SetDestination(Vector3 destination)
    {
        //curDestination = destination;
        agent.SetDestination(destination);
    }

    public void SetState(IState state)
    {
        if(curState != state)
        {
            curState?.OnExit(this);
            curState = state;
            curState?.OnEnter(this);
        }
    }

    public override void StopMoving()
    {
        base.StopMoving();
        agent.ResetPath();
    }

    public override void InitSpeed()
    {
        agent.speed = AgentMoveSpeed;
    }

    public override void ToggleTargetIndicator(bool value)
    {
        targetIndicatorImage.enabled = value;
    }

    protected override void UpdateBonusStatsFromItem(Item item)
    {
        base.UpdateBonusStatsFromItem(item);
        bonusAgentMoveSpeed = baseAgentMoveSpeed * item.BonusMoveSpeed * 0.01f;
        agent.speed = AgentMoveSpeed;
    }

    protected override void UpdateBoosterStats(Booster booster)
    {
        base.UpdateBoosterStats(booster);
        BoosterSpdMultipler = booster.MoveSpdMultiplier;
        agent.speed = AgentMoveSpeed;
    }

    protected override void DeactiveSpeedBooster()
    {
        base.DeactiveSpeedBooster();
        agent.speed = AgentMoveSpeed;
    }
}