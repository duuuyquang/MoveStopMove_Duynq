using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [SerializeField] Transform equippedTag;
    [SerializeField] Transform lockTag;

    public ItemDataSO itemDataSO;

    private Item item;
    public Item Item => item;


    public void OnInit(Item item)
    {
        this.item = item;
    }

    public void ToggleEquippedTag(bool value)
    {
        equippedTag.gameObject.SetActive(value);
    }

    public void ToggleLockTag(bool value)
    {
        lockTag.gameObject.SetActive(value);
    }

    public void SetPropsByItemType(TabType tab, ItemType type)
    {
        Item item;
        switch (tab)
        {
            case TabType.Head:
                item = itemDataSO.GetHead(type);
                OnInit(item);
                break;
            case TabType.Pants:
                item = itemDataSO.GetPants(type);
                OnInit(item);
                break;
            case TabType.Shield:
                item = itemDataSO.GetShield(type);
                OnInit(item);
                break;
            case TabType.Sets:
                //totalItems = itemDataSO.TotalSets;
                break;
        }
    }
}
