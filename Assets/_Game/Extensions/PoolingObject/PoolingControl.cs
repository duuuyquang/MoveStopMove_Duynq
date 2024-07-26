using System;
using UnityEngine;

public class PoolControl : Singleton<PoolControl>
{
    [SerializeField] PoolAmount[] poolAmounts;

    public ItemDataSO itemDataSO;
    public int eachWeaponPoolingNum = 10;
    [SerializeField] Transform weaponPoolTF;
    public Transform WeaponPoolTF => weaponPoolTF;

    private void Awake()
    {
        //// Load from resources
        //GameUnit[] gameUnits = Resources.LoadAll<GameUnit>("Pool/");
        //for (int i = 0; i < gameUnits.Length; i++)
        //{
        //    SimplePool.Preload(gameUnits[i], 0, new GameObject(gameUnits[i].name).transform);
        //}

        // Load from list
        for (int i = 0; i < poolAmounts.Length; i++)
        {
            SimplePool.Preload(poolAmounts[i].prefab, poolAmounts[i].amount, poolAmounts[i].parent);
        }

        for (int i = 1; i < Enum.GetNames(typeof(WeaponType)).Length; i++)
        {
            WeaponPool.Preload(itemDataSO.GetWeapon((WeaponType) i), eachWeaponPoolingNum, WeaponPoolTF);
        }

    }
}

public enum PoolType
{
    Bullet      = 0,
    Indicator   = 1,
    PointText   = 2,
    Enemy       = 3,
    ItemShop    = 4,
    Booster     = 5,
}

[System.Serializable]
public class PoolAmount
{
    public GameUnit prefab;
    public Transform parent;
    public int amount;
}