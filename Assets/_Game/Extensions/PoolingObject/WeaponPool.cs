using System.Collections.Generic;
using UnityEngine;

public static class WeaponPool
{
    private static Dictionary<WeaponType, EachWeaponPool> poolInstance = new Dictionary<WeaponType, EachWeaponPool>();

    //Init pool
    public static void Preload(WeaponUnit prefab, int amount, Transform parent)
    {
        if (prefab == null)
        {
            Debug.LogError("PREFAB IS EMPTY !!! ");
            return;
        }

        if (!poolInstance.ContainsKey(prefab.WeaponType) || poolInstance[prefab.WeaponType] == null)
        {
            EachWeaponPool p = new EachWeaponPool();
            p.Preload(prefab, amount, parent);
            poolInstance[prefab.WeaponType] = p;
        }
    }

    //Take element out
    public static T Spawn<T>(WeaponType poolType, Vector3 pos, Quaternion rot) where T : WeaponUnit
    {
        if (!poolInstance.ContainsKey(poolType))
        {
            Debug.LogError(poolType + " IS NOT PRELOAD !!!");
            return null;
        }

        return poolInstance[poolType].Spawn(pos, rot) as T;
    }

    //Return element in
    public static void Despawn(WeaponUnit unit)
    {
        if (!poolInstance.ContainsKey(unit.WeaponType))
        {
            Debug.LogError(unit.WeaponType + "IS NOT PRELOAD !!!");
        }
        poolInstance[unit.WeaponType].Despawn(unit);
    }

    //Collect element
    public static void Collect(WeaponType poolType)
    {
        if (!poolInstance.ContainsKey(poolType))
        {
            Debug.LogError(poolType + "IS NOT PRELOAD !!!");
        }
        poolInstance[poolType].Collect();
    }

    public static void CollectAll()
    {
        foreach (var item in poolInstance.Values)
        {
            item.Collect();
        }
    }

    //Destroy 1 pool
    public static void Release(WeaponType poolType)
    {
        if (!poolInstance.ContainsKey(poolType))
        {
            Debug.LogError(poolType + "IS NOT PRELOAD !!!");
        }
        poolInstance[poolType].Release();
    }

    public static void ReleaseAll()
    {
        foreach (var item in poolInstance.Values)
        {
            item.Release();
        }
    }
}

public class EachWeaponPool
{
    Transform parent;
    WeaponUnit prefab;

    //list contains units in pool
    Queue<WeaponUnit> inactives = new Queue<WeaponUnit>();

    //list contains units being used 
    List<WeaponUnit> actives = new List<WeaponUnit>();

    //Init pool
    public void Preload(WeaponUnit prefab, int amount, Transform parent)
    {
        this.parent = parent;
        this.prefab = prefab;

        for (int i = 0; i < amount; i++)
        {
            inactives.Enqueue(GameObject.Instantiate(prefab, parent));
        }
    }

    //Take element out from pool
    public WeaponUnit Spawn(Vector3 position, Quaternion rot)
    {
        WeaponUnit unit;
        if (inactives.Count <= 0)
        {
            unit = GameObject.Instantiate(prefab, parent);
        }
        else
        {
            unit = inactives.Dequeue();
        }

        unit.TF.SetPositionAndRotation(position, rot);
        actives.Add(unit);
        unit.gameObject.SetActive(true);

        return unit;
    }

    //Return element into pool
    public void Despawn(WeaponUnit unit)
    {
        if (unit != null && unit.gameObject.activeSelf)
        {
            actives.Remove(unit);
            inactives.Enqueue(unit);
            unit.gameObject.SetActive(false);
        }
    }

    //Collect all elements being used into pool
    public void Collect()
    {
        while (actives.Count > 0)
        {
            Despawn(actives[0]);
        }
    }

    //Destroy all elements
    public void Release()
    {
        Collect();
        while (inactives.Count > 0)
        {
            GameObject.Destroy(inactives.Dequeue().gameObject);
        }
        inactives.Clear();
    }
}