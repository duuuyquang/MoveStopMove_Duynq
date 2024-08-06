using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TabType { Head, Pants, Shield, Sets };

public class CanvasSkinShop : UICanvas
{
    private const float SCREEN_DEFAULT_WIDTH  = 720f;
    private const float ITEM_SELECT_PADDING = 10f;
    private const int   ITEM_NUM_EACH_COL = 2;

    [SerializeField] TextMeshProUGUI    coinText;
    [SerializeField] TextMeshProUGUI    stats;
    [SerializeField] Transform[]        buttons;
    [SerializeField] TextMeshProUGUI[]  priceTexts;

    [SerializeField] Image[] tabBGs;

    [SerializeField] RectTransform  listContent;
    [SerializeField] ItemShop       itemShopPrefab;
    [SerializeField] Transform      itemSelectBG;

    private List<ItemShop> curItemShopList = new();

    private TabType curTab;
    private ItemShop choosingItem;

    public ItemDataSO itemDataSO;

    public void Update()
    {
        if(UIManager.Instance.IsOpened<CanvasSkinShop>())
        {
            LevelManager.Instance.Player.RotateAround();
        }    
    }

    public void OnOpen()
    {
        SetCoinText(GameManager.Instance.TotalCoin);
        OnSelectingTab(0);
        LevelManager.Instance.Player.ChangeAnim(Const.ANIM_NAME_SHOP);
        CameraFollower.Instance.SetupSkinShopMode();
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        SoundManager.Instance.PlayBtnClick();
    }

    public void Purchase()
    {
        if(GameManager.Instance.TotalCoin >= choosingItem.Item.Price)
        {
            UpdateItemBought(choosingItem.Item.Type);
            GameManager.Instance.ReduceTotalCoin(choosingItem.Item.Price);
            SetCoinText(GameManager.Instance.TotalCoin);
            PrintBuyButtons();
            SoundManager.Instance.PlayBtnClick();
        } 
        else
        {
            SoundManager.Instance.PlayBtnClickError();
        }
    }

    public void Select()
    {
        ChangeItem(choosingItem.Item.Type, true);
        PrintBuyButtons();
        UpdateAllItemShopState();
        SoundManager.Instance.PlayBtnClick();
    }

    public void Unequip()
    {
        ChangeItem(ItemType.None, true);
        LevelManager.Instance.Player.ChangeColor(PlayerData.Instance.colorType);
        PrintBuyButtons();
        choosingItem.ToggleEquippedTag(false);
        SoundManager.Instance.PlayBtnClick();
    }

    public void SetCoinText(float coin) => coinText.text = coin.ToString();

    private bool IsItemBought(ItemType type) => PlayerData.Instance.itemsState[type] == ItemState.Bought;

    private void UpdateItemBought(ItemType type)
    {
        PlayerData.Instance.itemsState[type] = ItemState.Bought;
        PlayerData.SaveData();
    }

    public void DisplayData()
    {
        float totalItems = GetTotalPurchasableItems();
        float totalCol   = Mathf.Floor( totalItems / ITEM_NUM_EACH_COL );

        float initalItemSelectWidth = SCREEN_DEFAULT_WIDTH / tabBGs.Length + ITEM_SELECT_PADDING * 2f;
        float initalItemSelectHeight = initalItemSelectWidth * 0.75f;

        float eachItemTotalWidth  = initalItemSelectWidth + ITEM_SELECT_PADDING * 2f;
        float eachItemTotalHeight = initalItemSelectHeight + ITEM_SELECT_PADDING * 2f;

        float totalWidth  = Mathf.Max( SCREEN_DEFAULT_WIDTH, eachItemTotalWidth * totalCol + ITEM_SELECT_PADDING * 2f );
        float offsetRight = Mathf.Max( 0f, totalWidth - SCREEN_DEFAULT_WIDTH);
        float posX = -totalWidth / 2f + initalItemSelectWidth / 2f + ITEM_SELECT_PADDING * 2f;
        float posY = 0f;

        Ultilities.SetRectTFLeft(listContent, 0);
        Ultilities.SetRectTFRight(listContent, -offsetRight);

        ClearItemList();

        for (int i = 1; i < totalItems; i++)
        {
            ItemShop itemShop = SimplePool.Spawn<ItemShop>(PoolType.ItemShop, Vector3.zero, Quaternion.identity);
            itemShop.TF.SetParent(listContent);
            itemShop.SetPropsByItemType(curTab,(ItemType)i);
            UpdateItemShopState(itemShop);

            posY = i % 2 == 0 ? posY - eachItemTotalHeight : 0f;
            posX = i % 2 == 1 && i != 1 ? posX + eachItemTotalWidth : posX;
            itemShop.RectTF.anchoredPosition = Cache.GetVector(posX, posY, 0f);

            curItemShopList.Add(itemShop);
        }
        OnChosingItem(curItemShopList[0]);
    }

    private void UpdateItemShopState(ItemShop itemShop)
    {
        itemShop.ToggleLockTag(true);
        itemShop.ToggleEquippedTag(false);
        if (IsItemBought(itemShop.Item.Type))
        {
            itemShop.ToggleLockTag(false);
            if (CheckSameItem(itemShop.Item.Type))
            {
                itemShop.ToggleEquippedTag(true);
            }
        }
    }

    private void UpdateAllItemShopState()
    {
        for (int i = 0; i < curItemShopList.Count; i++)
        {
            UpdateItemShopState(curItemShopList[i]);
        }
    }

    private int GetTotalPurchasableItems()
    {
        switch (curTab)
        {
            case TabType.Head:
                return itemDataSO.TotalHats;
            case TabType.Pants:
                return itemDataSO.TotalPants;
            case TabType.Shield:
                return itemDataSO.TotalShields;
            case TabType.Sets:
                return itemDataSO.TotalSets;
            default:
                return 0;
        }
    }

    private bool CheckSameItem(ItemType type)
    {
        switch (curTab)
        {
            case TabType.Head:
                return PlayerData.Instance.headType == type;
            case TabType.Pants:
                return PlayerData.Instance.pantsType == type;
            case TabType.Shield:
                return PlayerData.Instance.shieldType == type;
            case TabType.Sets:
                return PlayerData.Instance.setType == type;
            default:
                return false;
        }
    }

    private void ChangeItem(ItemType type, bool saveData = false)
    {
        switch (curTab)
        {
            case TabType.Head:
                LevelManager.Instance.Player.ChangeSet(ItemType.None);
                LevelManager.Instance.Player.ChangeHead(type);
                if(saveData)
                {
                    PlayerData.Instance.setType = ItemType.None;
                    PlayerData.Instance.headType = type;
                    PlayerData.SaveData();
                }
                break;
            case TabType.Pants:
                LevelManager.Instance.Player.ChangeSet(ItemType.None);
                LevelManager.Instance.Player.ChangePants(type);
                if (saveData)
                {
                    PlayerData.Instance.setType = ItemType.None;
                    PlayerData.Instance.pantsType = type;
                    PlayerData.SaveData();
                }
                break;
            case TabType.Shield:
                LevelManager.Instance.Player.ChangeSet(ItemType.None);
                LevelManager.Instance.Player.ChangeShield(type);
                if (saveData)
                {
                    PlayerData.Instance.setType = ItemType.None;
                    PlayerData.Instance.shieldType = type;
                    PlayerData.SaveData();
                }
                break;
            case TabType.Sets:
                LevelManager.Instance.Player.ChangeSet(type);
                if (saveData)
                {
                    PlayerData.Instance.headType = ItemType.None;
                    PlayerData.Instance.pantsType = ItemType.None;
                    PlayerData.Instance.shieldType = ItemType.None;
                    PlayerData.Instance.setType = type;
                    PlayerData.SaveData();
                }
                break;
        }
    }

    private void PrintBuyButtons()
    {
        ResetButtonsState();

        if (CheckSameItem(choosingItem.Item.Type))
        {
            buttons[3].gameObject.SetActive(true);
            choosingItem.ToggleLockTag(false);
        }
        else if (IsItemBought(choosingItem.Item.Type))
        {
            buttons[2].gameObject.SetActive(true);
            choosingItem.ToggleLockTag(false);
        }
        else if (GameManager.Instance.TotalCoin >= choosingItem.Item.Price)
        {
            buttons[1].gameObject.SetActive(true);
            priceTexts[1].text = choosingItem.Item.Price.ToString();
        }
        else
        {
            buttons[0].gameObject.SetActive(true);
            priceTexts[0].text = choosingItem.Item.Price.ToString();
        }
    }

    private void ResetButtonsState()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < priceTexts.Length; i++)
        {
            priceTexts[i].text = "";
        }
    }

    private void DisplayStatsText()
    {
        stats.text = "";
        if (choosingItem.Item.BonusMoveSpeed > 0)
        {
            stats.text += $"+{choosingItem.Item.BonusMoveSpeed.ToString()}% move speed \n";
        }
        if (choosingItem.Item.BonusAttackRange > 0)
        {
            stats.text += $"+{choosingItem.Item.BonusAttackRange.ToString()}% attack range\n";
        }
        if (choosingItem.Item.BonusGold > 0)
        {
            stats.text += $"+{choosingItem.Item.BonusGold.ToString()}% bonus gold";
        }
    }

    private void SetChosingItemBG(ItemShop chosenItem)
    {
        itemSelectBG.position = chosenItem.TF.position;
    }

    public void OnChosingItem(ItemShop itemShop)
    {
        choosingItem = itemShop;
        SetChosingItemBG(itemShop);
        ChangeItem(itemShop.Item.Type);
        DisplayStatsText();
        PrintBuyButtons();
        SoundManager.Instance.PlayBtnClick();
    }

    public void OnSelectingTab(int tab)
    {
        curTab = (TabType)tab;
        for (int i = 0; i < tabBGs.Length; i++)
        {
            tabBGs[i].enabled = true;
        }
        tabBGs[tab].enabled = false;
        LevelManager.Instance.Player.ChangeToSavedItems();
        DisplayData();
        SoundManager.Instance.PlayBtnClick();
    }

    private void ClearItemList()
    {
        while (curItemShopList.Count > 0)
        {
            ItemShop item = curItemShopList[0];
            curItemShopList.Remove(item);
            SimplePool.Despawn(item);
        }
        curItemShopList.Clear();
    }
}