using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public void Awake()
    {
        OnInit();
    }
    public void OnInit()
    {
        DataWeapon.Init();
    }
}
