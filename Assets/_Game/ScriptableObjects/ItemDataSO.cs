using UnityEngine;

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

//public enum HeadType
//{
//    None        = 0,
//    Arrow       = 1,
//    Beared      = 2,
//    Crown       = 3,
//    Ear         = 4,
//    NormalHat   = 5,
//    Cap         = 6,
//    Yellow      = 7,
//    Headphone   = 8,
//    Horn        = 9
//}

//public enum PantsType
//{
//    None = 0,
//    Batman = 1,
//    ChamBi = 2,
//    Comy = 3,
//    Dabao = 4,
//    Onion = 5,
//    Pokemon = 6,
//    Rainbow = 7,
//    Skull = 9,
//    Vantim = 10
//}

//public enum ShieldType
//{
//    None = 0,
//    CaptainAmerican = 1,
//    Normal = 2,
//}

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
    Pants_Batman            = 101,
    Pants_ChamBi            = 102,
    Pants_Comy              = 103,
    Pants_Dabao             = 104,
    Pants_Onion             = 105,
    Pants_Pokemon           = 106,
    Pants_Rainbow           = 107,
    Pants_Skull             = 108,
    Pants_Vantim            = 109,
    Shield_CaptainAmerican  = 201,
    Shield_Normal           = 202,
}

[CreateAssetMenu(menuName = "ItemDataSO")]

public class ItemDataSO : ScriptableObject
{
    [SerializeField] Weapon[] weapons;
    [SerializeField] Item[] hats;
    [SerializeField] Item[] pants;
    [SerializeField] Item[] shields;
    [SerializeField] Material[] pantsMaterials;

    public Weapon GetWeapon(WeaponType index)
    {
        return weapons[(int)index];
    }

    public Item GetHead(ItemType index)
    {
        return hats[(int)index % 100];
    }

    public Material GetPantsMat(ItemType index)
    {
        return pantsMaterials[(int)index % 100];
    }

    public Item GetPants(ItemType index)
    {
        return pants[(int)index % 100];
    }

    public Item GetShield(ItemType index)
    {
        return shields[(int)index % 100];
    }

    public int TotalWeapons => weapons.Length;
    public int TotalHats => hats.Length;
    public int TotalPants => pantsMaterials.Length;
    public int TotalShields => shields.Length;
}
