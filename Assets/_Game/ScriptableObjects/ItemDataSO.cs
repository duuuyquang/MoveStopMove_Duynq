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
    Pants_Batman            = 101,
    Pants_ChamBi            = 102,
    Pants_Comy              = 103,
    Pants_Dabao             = 104,
    Pants_Onion             = 105,
    Pants_Pokemon           = 106,
    Pants_Rainbow           = 107,
    Pants_Skull             = 108,
    Pants_Vantim            = 109,
    Pants_Devil             = 110,
    Pants_Angel             = 111,
    Pants_Deadpool          = 112,
    Pants_Thor              = 113,
    Shield_CaptainAmerican  = 201,
    Shield_Normal           = 202,
    Shield_BowAngel         = 203,
    Shield_BookWitch        = 204,
    Wing_Devil              = 301,
    Wing_Angel              = 302,
    Wing_BladeDeadpool      = 303,
    Tail_Devil              = 401,
    Set_1                   = 1001,
    Set_2                   = 1002,
    Set_3                   = 1003,
    Set_4                   = 1004,
    Set_5                   = 1005,
    Set_6                   = 1006
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
        return hats[(int)index % 100];
    }

    public Item GetPants(ItemType index)
    {
        return pants[(int)index % 100];
    }

    public Item GetShield(ItemType index)
    {
        return shields[(int)index % 100];
    }

    public Item GetTail(ItemType index)
    {
        return tails[(int)index % 100];
    }

    public Item GetWing(ItemType index)
    {
        return wings[(int)index % 100];
    }

    public Item GetSet(ItemType index)
    {
        return sets[(int)index % 1000];
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
