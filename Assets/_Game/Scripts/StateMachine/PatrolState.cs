using UnityEngine;

public class PatrolState : IState
{
    public void OnEnter(Enemy enemy)
    {
        enemy.StateCounter = 0f;
        enemy.StateDelayTime = Random.Range(0f, 1f);
        enemy.SetRandomDestination(Random.Range(5f, 20f));
    }

    public void OnExecute(Enemy enemy)
    {
        enemy.ChangeAnimByCurStatus();
        if (enemy.HasTargetInRange)
        {
            enemy.StateCounter += Time.deltaTime;
            if (enemy.StateCounter >= enemy.StateDelayTime)
            {
                enemy.SetState(new AttackState());
            }
        }

        if (enemy.IsDestination)
        {
            enemy.SetState(new IdleState());
        }
    }

    public void OnExit(Enemy enemy)
    {
    }
}
