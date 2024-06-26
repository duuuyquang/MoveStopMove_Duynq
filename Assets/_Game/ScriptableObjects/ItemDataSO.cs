using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

public enum WeaponType
{
    Axe = 0,
    Hammer = 1,
    Bloom = 2,
    Boomerang = 3,
    Mace = 4,
    Umbrella = 5,
    Bat = 6,
    Lollipop = 7
}

public enum ItemType
{
    Axe = 0,
    Hammer = 1,
    Bloom = 2,
    Boomerang = 3,
    Mace = 4,
    Umbrella = 5,
    Bat = 6,
    Lollipop = 7
}

[CreateAssetMenu(menuName = "ItemDataSO")]

public class ItemDataSO : ScriptableObject
{
    [SerializeField] GameObject[] weapons;
    [SerializeField] GameObject[] items;

    public GameObject GetWeapon(WeaponType index)
    {
        return weapons[(int)index];
    }

    public GameObject GetItem(ItemType index)
    {
        return items[(int)index];
    }
}
