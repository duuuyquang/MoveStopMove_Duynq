using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;
    private Vector3 movingDir;
    [SerializeField] Transform avatar;
    private Player player;

    void Update()
    {
        transform.Translate(movingDir * speed * Time.deltaTime);
        //transform.position = Vector3.MoveTowards(transform.position, movingDir, speed * Time.deltaTime);
        avatar.Rotate(new Vector3(0, 1, 0), 15);
    }

    public void OnInit(Player player, float speed, Vector3 direction)
    {
        this.speed = speed;
        this.player = player;
        movingDir = direction;

    }

    public void OnDespawn()
    {
        player.ReloadBullet(1);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if(other.CompareTag("Enemy"))
        {
            OnDespawn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("AttackRange"))
        {
            OnDespawn();
        }
    }
}
