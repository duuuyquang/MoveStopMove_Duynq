using UnityEngine;

public class AttackState : IState
{
    public void OnEnter(Enemy enemy)
    {
        enemy.StopMoving();
        enemy.StateDelayTime = Random.Range(0f, 1f);
        enemy.StateCounter = 0f;
        enemy.StateIsAttacked = false;
    }

    public void OnExecute(Enemy enemy)
    {
       enemy.ChangeAnimByCurStatus();
        enemy.StateCounter += Time.deltaTime;
        if (enemy.StateCounter >= enemy.StateDelayTime)
        {
            enemy.CheckToProcessAttack();
            if (!enemy.StateIsAttacked)
            {
                enemy.StateIsAttacked = enemy.CheckToProcessAttack();
            }

            if(enemy.WeaponHolder.CurWeapon.IsGrab)
            {
                if (enemy.WeaponHolder.CurBullet && enemy.WeaponHolder.CurBullet.IsDropped && enemy.IsStatus(StatusType.Normal))
                {
                    Vector3 targetPos = new Vector3(enemy.WeaponHolder.CurBullet.TF.position.x, enemy.TF.position.y, enemy.WeaponHolder.CurBullet.TF.position.z);
                    enemy.StateTargetPos = targetPos;
                    enemy.SetState(new RunToDestination());
                }
                return;
            }

            if (enemy.IsStatus(StatusType.Normal)) 
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
    }

    public void OnExit(Enemy enemy)
    {
    }
}
