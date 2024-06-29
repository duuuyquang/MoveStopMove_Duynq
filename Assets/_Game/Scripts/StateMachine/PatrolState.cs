using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private float distance = Random.Range(5f, 20f);

    private float delayAttackTimer = Random.Range(0f, 1.5f);
    private float count = 0f;

    public void OnEnter(Enemy enemy)
    {
        enemy.SetRandomDestination(distance);
    }

    public void OnExecute(Enemy enemy)
    {
        if (enemy.HasTargetInRange)
        {
            count += Time.deltaTime;
            if (count >= delayAttackTimer)
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
