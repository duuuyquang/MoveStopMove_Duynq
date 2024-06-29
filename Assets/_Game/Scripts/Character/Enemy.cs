using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Canvas indicator;

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
            if (curState != null && !IsDead)
            {
                curState.OnExecute(this);
            }
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        SetState(new IdleState());
        ToggleIndicator(false);
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

    private void SetDestination(Vector3 destination)
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
        agent.ResetPath();
    }

    public override void ToggleIndicator(bool value)
    {
        indicator.enabled = value;
    }
}
