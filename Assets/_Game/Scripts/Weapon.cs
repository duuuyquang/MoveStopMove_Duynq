using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    public Player player;
    public Bullet bulletPrefab;
    public float speed;

    public void Throw(Vector3 targetPos)
    {
        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.OnInit(player, speed, targetPos);
    }
}
