using UnityEngine;

public class IdleState : IState
{
    public void OnEnter(Enemy enemy)
    {
        enemy.StopMoving();
        enemy.StateCounter = 0f;
        enemy.StateDelayTime = Random.Range(0f, 1.5f);
    }

    public void OnExecute(Enemy enemy)
    {
        enemy.ChangeAnimByCurStatus();
        enemy.StateCounter += Time.deltaTime;

        if (enemy.HasTargetInRange)
        {
            if (RollToAttack())
            {
                enemy.SetState(new AttackState());
            }
            else
            {
                enemy.SetState(new PatrolState());
            }
        }

        if (enemy.StateCounter >= enemy.StateDelayTime)
        {
            enemy.SetState(new PatrolState());
        }
    }

    public bool RollToAttack()
    {
        return Random.Range(0, 10) < 8;
    }

    public void OnExit(Enemy enemy)
    {
    }
}
