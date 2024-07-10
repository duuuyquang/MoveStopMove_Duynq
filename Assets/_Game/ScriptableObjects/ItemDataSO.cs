using UnityEngine;

public enum WeaponType
{
    None = 0,
    Axe = 1,
    Hammer = 2,
    Broom = 3,
    Boomerang = 4,
    Mace = 5,
    Umbrella = 6,
    Bat = 7,
    Lollipop = 8
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
    [SerializeField] Weapon[] weapons;
    [SerializeField] GameObject[] items;

    public Weapon GetWeapon(WeaponType index)
    {
        return weapons[(int)index];
    }

    public GameObject GetItem(ItemType index)
    {
        return items[(int)index];
    }
}
