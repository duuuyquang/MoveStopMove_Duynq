using UnityEngine;

public class RunToDestination : IState
{
    public void OnEnter(Enemy enemy)
    {
        enemy.SetDestination(enemy.StateTargetPos);
    }

    public void OnExecute(Enemy enemy)
    {
        enemy.ChangeAnimByCurStatus();
        if (enemy.IsDestination)
        {
            if (Random.Range(0, 10) < 5)
            {
                enemy.SetState(new PatrolState());
            }
            else
            {
                enemy.SetState(new IdleState());
            }
        }
    }

    public void OnExit(Enemy enemy)
    {
    }
}
