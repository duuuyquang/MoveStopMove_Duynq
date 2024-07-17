using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TabType { Head, Pants, Shield, Sets };

public enum ItemState { IsNotPurchasable, IsPurchasable, IsSelect, IsEquipped };

public class CanvasSkinShop : UICanvas
{
    private const float SCREEN_DEFAULT_WIDTH  = 720;

    private const float ITEM_SELECT_WIDTH   = SCREEN_DEFAULT_WIDTH/4 + ITEM_SELECT_PADDING*2f;
    private const float ITEM_SELECT_HEIGHT  = ITEM_SELECT_WIDTH * 0.75f;
    private const float ITEM_SELECT_PADDING = 10f;

    private const int ITEM_NUM_EACH_COL = 2;

    [SerializeField] private TextMeshProUGUI coinText;

    [SerializeField] TextMeshProUGUI    stats;
    [SerializeField] Transform[]        buttons;
    [SerializeField] TextMeshProUGUI[] priceTexts;

    [SerializeField] Image[] tabBGs;

    [SerializeField] RectTransform  listContent;
    [SerializeField] ItemShop       itemShopPrefab;
    [SerializeField] Transform      itemSelectBG;

    private List<ItemShop> curItemShopList = new List<ItemShop>();

    private TabType curTab = TabType.Head;
    private ItemShop    choosingItem;

    public ItemDataSO itemDataSO;

    private Item cachedHead;
    private Item cachedPants;
    private Item cachedShield;
    private Item cachedSet;

    private List<Sprite> headSprites = new List<Sprite>();
    private List<Sprite> pantsSprites = new List<Sprite>();
    private List<Sprite> shieldPrites = new List<Sprite>();

    public void Start()
    {
        OnSelectingTab((int)curTab);
    }

    public void SetCoin(float coin)
    {
        coinText.text = coin.ToString();
    }

    public void CachedPlayerItems()
    {
        cachedHead = LevelManager.Instance.Player.CurHead;
        cachedPants = LevelManager.Instance.Player.CurPants;
        cachedShield = LevelManager.Instance.Player.CurShield;
        cachedSet = LevelManager.Instance.Player.CurSet;
    }

    private void RevertPlayerItems()
    {
        if(cachedSet != null)
        {
            LevelManager.Instance.Player.ChangeSet(cachedSet.Type);
        } 
        else
        {
            LevelManager.Instance.Player.ChangeHead(cachedHead.Type);
            LevelManager.Instance.Player.ChangePants(cachedPants.Type);
            LevelManager.Instance.Player.ChangeShield(cachedShield.Type);
        }
    }

    public void MainMenuButton()
    {
        RevertPlayerItems();
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>();
        GameManager.ChangeState(GameState.MainMenu);
        LevelManager.Instance.Player.InitTransform();
        LevelManager.Instance.Player.ChangeAnim(Const.ANIM_NAME_IDLE);
        CameraFollower.Instance.SetupMenuMode();
    }

    public void Purchase()
    {
        if(GameManager.Instance.TotalCoin >= choosingItem.Item.Price)
        {
            GameManager.Instance.ReduceTotalCoin(choosingItem.Item.Price);
            LevelManager.Instance.Player.AddOwnedItem(choosingItem.Item.Type);
            UIManager.Instance.GetUI<CanvasMainMenu>().SetCoin(GameManager.Instance.TotalCoin);
            SetCoin(GameManager.Instance.TotalCoin);
            PrintBuyButtons();
        }
    }

    public void Select()
    {
        ChangePlayerItem(choosingItem.Item.Type);
        CachedPlayerItems();
        PrintBuyButtons();
        choosingItem.ToggleEquippedTag(true);
    }

    public void Unequip()
    {
        ChangePlayerItem(ItemType.None);
        CachedPlayerItems();
        PrintBuyButtons();
        choosingItem.ToggleEquippedTag(false);
    }

    public void DisplayData()
    {
        float totalItems = GetTotalPurchasableItems();
        float totalCol   = Mathf.Floor( totalItems / ITEM_NUM_EACH_COL );

        float eachItemTotalWidth  = ITEM_SELECT_WIDTH + ITEM_SELECT_PADDING * 2f;
        float eachItemTotalHeight = ITEM_SELECT_HEIGHT + ITEM_SELECT_PADDING * 2f;

        float totalWidth  = Mathf.Max( SCREEN_DEFAULT_WIDTH, eachItemTotalWidth * totalCol + ITEM_SELECT_PADDING * 2f );
        float offsetRight = Mathf.Max( 0f, totalWidth - SCREEN_DEFAULT_WIDTH);
        float posX = -totalWidth / 2f + ITEM_SELECT_WIDTH / 2f + ITEM_SELECT_PADDING * 2f;
        float posY = 0f;

        Ultilities.SetRectTFLeft(listContent, 0);
        Ultilities.SetRectTFRight(listContent, -offsetRight);

        ClearItemList();

        for (int i = 1; i < totalItems; i++)
        {
            ItemShop itemShop = Instantiate(itemShopPrefab, listContent);
            itemShop.SetPropsByItemType(curTab,(ItemType)i);
            UpdateItemShopState(itemShop);

            posY = i % 2 == 0 ? posY - eachItemTotalHeight : 0f;
            posX = i % 2 == 1 && i != 1 ? posX + eachItemTotalWidth : posX;
            itemShop.RectTF.anchoredPosition = new Vector3(posX, posY);

            curItemShopList.Add(itemShop);
        }
        OnChosingItem(curItemShopList.First());
    }

    private void UpdateItemShopState(ItemShop itemShop)
    {
        itemShop.ToggleLockTag(true);
        itemShop.ToggleEquippedTag(false);
        if (LevelManager.Instance.Player.IsOwnedItem(itemShop.Item.Type))
        {
            itemShop.ToggleLockTag(false);
            if (CheckSameItem(itemShop.Item.Type))
            {
                itemShop.ToggleEquippedTag(true);
            }
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
                return cachedHead.Type == type;
            case TabType.Pants:
                return cachedPants.Type == type;
            case TabType.Shield:
                return cachedShield.Type == type;
            case TabType.Sets:
                return cachedSet.Type == type;
            default:
                return false;
        }
    }

    private void ChangePlayerItem(ItemType type)
    {
        switch (curTab)
        {
            case TabType.Head:
                LevelManager.Instance.Player.ChangeHead(type);
                break;
            case TabType.Pants:
                LevelManager.Instance.Player.ChangePants(type);
                break;
            case TabType.Shield:
                LevelManager.Instance.Player.ChangeShield(type);
                break;
            case TabType.Sets:
                LevelManager.Instance.Player.ChangeSet(type);
                break;
        }
    }

    private void PrintBuyButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < priceTexts.Length; i++)
        {
            priceTexts[i].text = "";
        }

        if (CheckSameItem(choosingItem.Item.Type))
        {
            buttons[(int)ItemState.IsEquipped].gameObject.SetActive(true);
            choosingItem.ToggleLockTag(false);
        }
        else if (LevelManager.Instance.Player.IsOwnedItem(choosingItem.Item.Type))
        {
            buttons[(int)ItemState.IsSelect].gameObject.SetActive(true);
            choosingItem.ToggleLockTag(false);
        }
        else if (GameManager.Instance.TotalCoin >= choosingItem.Item.Price)
        {
            buttons[(int)ItemState.IsPurchasable].gameObject.SetActive(true);
            priceTexts[(int)ItemState.IsPurchasable].text = choosingItem.Item.Price.ToString();
        }
        else
        {
            buttons[(int)ItemState.IsNotPurchasable].gameObject.SetActive(true);
            priceTexts[(int)ItemState.IsNotPurchasable].text = choosingItem.Item.Price.ToString();
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
        itemSelectBG.SetParent(chosenItem.TF, false);
    }

    public void OnChosingItem(ItemShop itemShop)
    {
        choosingItem = itemShop;
        ChangePlayerItem(itemShop.Item.Type);
        DisplayStatsText();
        PrintBuyButtons();
        SetChosingItemBG(itemShop);
    }

    private void OnSelectingTab(int tab)
    {
        for (int i = 0; i < tabBGs.Length; i++)
        {
            tabBGs[i].enabled = true;
        }
        tabBGs[tab].enabled = false;
        curTab = (TabType) tab;
        RevertPlayerItems();
        DisplayData();
    }

    private void ClearItemList()
    {
        itemSelectBG.SetParent(listContent, false);
        while (curItemShopList.Count > 0)
        {
            ItemShop item = curItemShopList[0];
            curItemShopList.Remove(item);
            Destroy(item.gameObject);
        }
        curItemShopList.Clear();
    }
}