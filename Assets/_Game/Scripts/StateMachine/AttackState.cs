using UnityEngine;

public class AttackState : IState
{
    float delayTime = Random.Range(0f, 1.5f);
    float count = 0f;
    bool isAttacked = false;

    public void OnEnter(Enemy enemy)
    {
        enemy.StopMoving();
    }

    public void OnExecute(Enemy enemy)
    {
        count += Time.deltaTime;
        if (count >= delayTime)
        {
            if(!isAttacked)
            {
                enemy.CheckAndProcessAttack();
                isAttacked = true;
            }

            if(enemy.WeaponHolder.IsGrab)
            {
                if (enemy.WeaponHolder.CurBullet && enemy.WeaponHolder.CurBullet.IsDropped && enemy.IsStatus(StatusType.Normal))
                {
                    Vector3 targetPos = new Vector3(enemy.WeaponHolder.CurBullet.TF.position.x, enemy.TF.position.y, enemy.WeaponHolder.CurBullet.TF.position.z);
                    enemy.SetState(new RunToDestination(targetPos));
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
