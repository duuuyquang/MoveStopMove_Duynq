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
            if(enemy.CurWeapon.IsGrab)
            {
                if (enemy.CurWeapon.CurBullet && enemy.CurWeapon.CurBullet.IsDropped && enemy.IsStatus(Character.StatusType.Normal))
                {
                    Vector3 targetPos = new Vector3(enemy.CurWeapon.CurBullet.TF.position.x, enemy.TF.position.y, enemy.CurWeapon.CurBullet.TF.position.z);
                    enemy.SetState(new RunToDestination(targetPos));
                }
                return;
            }

            if (enemy.IsStatus(Character.StatusType.Normal)) {
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
