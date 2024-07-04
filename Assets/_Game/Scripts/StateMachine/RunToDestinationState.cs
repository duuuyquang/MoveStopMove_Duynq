using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunToDestination : IState
{
    private Vector3 targetPos;

    public RunToDestination(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }

    public void OnEnter(Enemy enemy)
    {
        enemy.SetDestination(targetPos);
    }

    public void OnExecute(Enemy enemy)
    {
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
