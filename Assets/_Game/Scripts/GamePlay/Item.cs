using System;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] ItemType type;

    [Header("Stats")]
    [SerializeField] float bonusAttackRange = 0;
    [SerializeField] float bonusMoveSpeed = 0f;
    [SerializeField] float bonusGold = 0f;

    [Header("Shop")]
    [SerializeField] Sprite sprite;
    [SerializeField] int price = 500;

    [Header("Set Item Related")]
    [SerializeField] bool isItemInSet = false;
    [SerializeField] bool isSetItem = false;

    [ConditionalProperty("isSetItem")]
    [SerializeField] protected Item headItem;
    [ConditionalProperty("isSetItem")]
    [SerializeField] protected Item shieldItem;
    [ConditionalProperty("isSetItem")]
    [SerializeField] protected Item pantsItem;
    [ConditionalProperty("isSetItem")]
    [SerializeField] protected Item wingItem;
    [ConditionalProperty("isSetItem")]
    [SerializeField] protected Item tailItem;
    [ConditionalProperty("isSetItem")]
    [SerializeField] protected ColorType charColorType;

    public float BonusAttackRange => bonusAttackRange;
    public float BonusMoveSpeed => bonusMoveSpeed;
    public float BonusGold => bonusGold;
    public int Price => price;
    public bool ItemInSet => isItemInSet;
    public ItemType Type => type;
    public Sprite Sprite => sprite;

    public Item HeadItem => headItem;
    public Item ShieldItem => shieldItem;
    public Item PantsItem => pantsItem;
    public Item WingItem => wingItem;
    public Item TailItem => tailItem;
    public ColorType CharColorType => charColorType;

    public void Start()
    {
        if(isItemInSet)
        {
            bonusAttackRange = 0;
            bonusMoveSpeed = 0;
            bonusGold = 0;
            price = 0;
            isSetItem = false;
        }
    }

    public void OnInit(ItemType type, float attackSpeed = 0f, float moveSpeed = 0f, float gold = 0f)
    {
        this.type = type;
        bonusAttackRange = attackSpeed;
        bonusMoveSpeed = moveSpeed;
        bonusGold = gold;
        price = 500;
    }
}