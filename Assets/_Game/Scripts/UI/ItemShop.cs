using UnityEngine;
using UnityEngine.UI;

public class ItemShop : GameUnit
{
    [SerializeField] Transform equippedTag;
    [SerializeField] Transform lockTag;
    [SerializeField] Image image;

    public ItemDataSO itemDataSO;

    private Item item;
    public Item Item => item;

    public RectTransform RectTF;

    public void OnInit(Item item)
    {
        this.item = item;
        image.sprite = item.Sprite;
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
        switch (tab)
        {
            case TabType.Head:
                OnInit(itemDataSO.GetHead(type));
                break;
            case TabType.Pants:
                OnInit(itemDataSO.GetPants(type));
                break;
            case TabType.Shield:
                OnInit(itemDataSO.GetShield(type));
                break;
            case TabType.Sets:
                OnInit(itemDataSO.GetSet(type));
                break;
        }
    }
}
