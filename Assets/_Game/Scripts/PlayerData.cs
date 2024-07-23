using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerData
{
    public static PlayerData Instance { get; private set; }

    private const string FILE_PATH = "/_Game/Data/";
    private const string FILE_NAME = "PlayerProfile";
    public static string FileDirectory => Application.dataPath + FILE_PATH + $"{FILE_NAME}.json";

    #region JSON properties
    public string name = "YourName";
    public float  totalCoin = 0f;
    public int    curLevel = 0;

    public WeaponType weaponType = WeaponType.Axe;

    public ColorType colorType = ColorType.Yellow;

    public ItemType setType = ItemType.None;
    public ItemType headType = ItemType.None;
    public ItemType pantsType = ItemType.None;
    public ItemType shieldType = ItemType.None;

    public Dictionary<WeaponType, ItemState> weaponsState = new();
    public Dictionary<ItemType, ItemState>   itemsState = new();
    #endregion

    private static StreamReader streamReader;
    private static StreamWriter streamWrite;

    public static void LoadData()
    {
        try
        {
            Read();
        } 
        catch(Exception)
        {
            try
            {
                InitData();
            }
            catch(Exception)
            {
                Debug.LogError("FAILED TO INIT NEW DATA...");
            }
        }
    }

    private static void InitData()
    {
        Instance = new PlayerData();
        SaveData();
    }

    private static void SetData(PlayerData data)
    {
        Instance = data;
    }

    public static void SaveData()
    {
        Write(Instance);
    }

    private static void Read()
    {
        streamReader = new StreamReader(FileDirectory);
        string data = streamReader.ReadToEnd();
        streamReader.Close();
        SetData(JsonConvert.DeserializeObject<PlayerData>(data));
    }

    private static void Write(PlayerData data)
    {
        string json = JsonConvert.SerializeObject(data);
        streamWrite = new StreamWriter(FileDirectory);
        streamWrite.WriteLine(json);
        streamWrite.Close();
        SetData(data);
    }

    private static string GetFileDirectory()
    {
        return Application.dataPath + FILE_PATH + $"{FILE_NAME}.json";
    }
}