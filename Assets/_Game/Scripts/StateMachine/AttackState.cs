using UnityEngine;

public class AttackState : IState
{
    float delayTime = Random.Range(0f, 1.5f);
    float count = 0f;

    public void OnEnter(Enemy enemy)
    {
        enemy.StopMoving();
    }

    public void OnExecute(Enemy enemy)
    {
        count += Time.deltaTime;
        if (count >= delayTime)
        {
            enemy.CheckAndProcessAttack();
            if(Random.Range(0,10) < 5)
            {
                enemy.SetState(new PatrolState());
            } else
            {
                enemy.SetState(new IdleState());
            }
        }
    }

    public void OnExit(Enemy enemy)
    {
    }
}
