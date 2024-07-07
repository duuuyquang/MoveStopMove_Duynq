using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private float time;
    private float count;

    public void OnEnter(Enemy enemy)
    {
        enemy.StopMoving();
        time = Random.Range(0f, 2f);
        count = 0f;
    }

    public void OnExecute(Enemy enemy)
    {
        count += Time.deltaTime;

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

        if (count >= time )
        {
            enemy.SetState(new PatrolState());
        }
    }

    public bool RollToAttack()
    {
        return Random.Range(0f, 10f) < 5f;
    }

    public void OnExit(Enemy enemy)
    {

    }
}
