using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private Vector3 targetPos;
    [SerializeField] Transform avatar;
    private Player player;

    private bool IsDestination => Vector3.Distance(transform.position, targetPos) < 0.1f; 

    void Update()
    {
        MovingToTarget();
    }

    private void MovingToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        avatar.Rotate(new Vector3(0, 1, 0), 15);
        if (IsDestination)
        {
            OnDespawn();
        }
    }

    public void OnInit(Player player, float speed, Vector3 pos)
    {
        this.speed = speed;
        this.player = player;
        targetPos = pos;

    }

    public void OnDespawn()
    {
        player.ReloadBullet(1);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            OnDespawn();
        }
    }
}
