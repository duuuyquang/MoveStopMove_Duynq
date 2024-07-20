using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Cache
{
    private static Dictionary<Collider, Character> bridgeChars = new Dictionary<Collider, Character>();
    private static Dictionary<Collider, Bullet> bridgeBullets = new Dictionary<Collider, Bullet>();
    private static Dictionary<float, WaitForSeconds> wait = new Dictionary<float, WaitForSeconds>();

    public static Character GetChar(Collider collider)
    {
        if (!bridgeChars.ContainsKey(collider))
        {
            bridgeChars.Add(collider, collider.GetComponent<Character>());
        }

        return bridgeChars[collider];
    }

    public static Bullet GetBullet(Collider collider)
    {
        if (!bridgeBullets.ContainsKey(collider))
        {
            bridgeBullets.Add(collider, collider.GetComponent<Bullet>());
        }

        return bridgeBullets[collider];
    }
}
