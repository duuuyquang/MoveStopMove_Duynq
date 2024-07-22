using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState { Bought, InShop }

public enum WeaponType
{
    None        = 0,
    Axe         = 1,
    Lollipop    = 2,
    Plunger     = 3,
    Umbrella    = 4,
    Bat         = 5,
    Boomerang   = 6,
    Pan         = 7,
    Broom       = 8,
    Pitchfork   = 9,
    Bone        = 10,
    Mace        = 11,
    PoleAxe     = 12,
}

public enum ItemType
{
    None                    = 0,
    Head_Arrow              = 1,
    Head_Beared             = 2,
    Head_CowBoy             = 3,
    Head_Crown              = 4,
    Head_Ear                = 5,
    Head_NormalHat          = 6,
    Head_Cap                = 7,
    Head_Yellow             = 8,
    Head_Headphone          = 9,
    Head_Horn               = 10,
    Head_Angel              = 11,
    Head_Witch              = 12,
    Head_Thor               = 13,
    Pants_None              = 1000,
    Pants_Batman            = 1001,
    Pants_ChamBi            = 1002,
    Pants_Comy              = 1003,
    Pants_Dabao             = 1004,
    Pants_Onion             = 1005,
    Pants_Pokemon           = 1006,
    Pants_Rainbow           = 1007,
    Pants_Skull             = 1008,
    Pants_Vantim            = 1009,
    Pants_Devil             = 1010,
    Pants_Angel             = 1011,
    Pants_Deadpool          = 1012,
    Pants_Thor              = 1013,
    Shield_None             = 2000,
    Shield_CaptainAmerican  = 2001,
    Shield_Normal           = 2002,
    Shield_BowAngel         = 2003,
    Shield_BookWitch        = 2004,
    Wing_None               = 3000,
    Wing_Devil              = 3001,
    Wing_Angel              = 3002,
    Wing_BladeDeadpool      = 3003,
    Tail_None               = 4000,
    Tail_Devil              = 4001,
    Set_None                = 10000,
    Set_1                   = 10001,
    Set_2                   = 10002,
    Set_3                   = 10003,
    Set_4                   = 10004,
    Set_5                   = 10005,
}

[CreateAssetMenu(menuName = "ItemDataSO")]

public class ItemDataSO : ScriptableObject
{
    [SerializeField] Weapon[] weapons;
    [SerializeField] Item[] hats;
    [SerializeField] Item[] pants;
    [SerializeField] Item[] shields;
    [SerializeField] Item[] wings;
    [SerializeField] Item[] tails;
    [SerializeField] Item[] sets;

    public Weapon GetWeapon(WeaponType index)
    {
        return weapons[(int)index];
    }

    public Item GetHead(ItemType index)
    {
        return hats[(int)index];
    }

    public Item GetPants(ItemType index)
    {
        return pants[(int)index % (int)ItemType.Pants_None];
    }

    public Item GetShield(ItemType index)
    {
        return shields[(int)index % (int)ItemType.Shield_None];
    }

    public Item GetWing(ItemType index)
    {
        return wings[(int)index % (int)ItemType.Wing_None];
    }

    public Item GetTail(ItemType index)
    {
        return tails[(int)index % (int)ItemType.Tail_None];
    }

    public Item GetSet(ItemType index)
    {
        return sets[(int)index % (int)ItemType.Set_None];
    }

    public int TotalWeapons => weapons.Length;
    public int TotalHats => GetTotal(hats);
    public int TotalPants => GetTotal(pants);
    public int TotalShields => GetTotal(shields);
    public int TotalSets => GetTotal(sets);

    public int GetTotal(Item[] array)
    {
        int count = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].ItemInSet) continue;
            count++;
        }

        return count;
    }

    public Dictionary<WeaponType, ItemState> InitAllWeaponsState()
    {
        Dictionary<WeaponType, ItemState> weaponsState = new();

        foreach(WeaponType type in Enum.GetValues(typeof(WeaponType)))
        {
            weaponsState[type] = ItemState.InShop;
        }

        return weaponsState;
    }

    public Dictionary<ItemType, ItemState> InitAllItemsState()
    {
        Dictionary<ItemType, ItemState> itemsState = new();

        foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
        {
            itemsState[type] = ItemState.InShop;
        }

        return itemsState;
    }
}
