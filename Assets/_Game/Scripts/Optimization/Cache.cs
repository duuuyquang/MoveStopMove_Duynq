using System.Collections.Generic;
using UnityEngine;

public static class Cache
{
    private static Dictionary<Collider, Character> bridgeChars = new();
    private static Dictionary<Collider, Bullet> bridgeBullets = new();
    private static Dictionary<Collider, Booster> bridgeBoosters = new();
    private static Dictionary<float, WaitForSeconds> bridgeWaitSecs = new();
    private static Dictionary<string, Vector3> bridgeVectors = new();

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

    public static Booster GetBooster(Collider collider)
    {
        if (!bridgeBoosters.ContainsKey(collider))
        {
            bridgeBoosters.Add(collider, collider.GetComponent<Booster>());
        }

        return bridgeBoosters[collider];
    }

    public static WaitForSeconds GetWaitSecs(float secs)
    {
        if (!bridgeWaitSecs.ContainsKey(secs))
        {
            bridgeWaitSecs.Add(secs, new WaitForSeconds(secs));
        }

        return bridgeWaitSecs[secs];
    }

    public static Vector3 GetVector(float x, float y, float z)
    {

        if (!bridgeVectors.ContainsKey($"{x}_{y}_{z}"))
        {
            bridgeVectors.Add($"{x}_{y}_{z}", new Vector3(x,y,z));
        }

        return bridgeVectors[$"{x}_{y}_{z}"];
    }
}
