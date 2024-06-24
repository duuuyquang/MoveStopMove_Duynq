using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Cache
{
    private static Dictionary<Collider, Character> bridgeChars = new Dictionary<Collider, Character>();

    public static Character GetChars(Collider collider)
    {
        if(!bridgeChars.ContainsKey(collider))
        {
            bridgeChars.Add(collider, collider.GetComponent<Character>());
        }

        return bridgeChars[collider];
    }
}
