using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] ItemType type;

    [Header("Stats")]
    [SerializeField] float bonusAttackRange = 0;
    [SerializeField] float bonusMoveSpeed = 0f;
    [SerializeField] float bonusGold = 0f;

    [Header("Shop")]
    [SerializeField] int price = 500;

    public float BonusAttackRange => bonusAttackRange;
    public float BonusMoveSpeed => bonusMoveSpeed;
    public float BonusGold => bonusGold;
    public int Price => price;

    public ItemType Type => type;

    public void OnInit(ItemType type, float attackSpeed = 0f, float moveSpeed = 0f, float gold = 0f)
    {
        this.type = type;
        bonusAttackRange = attackSpeed;
        bonusMoveSpeed = moveSpeed;
        bonusGold = gold;
        price = 500;
    }
}
