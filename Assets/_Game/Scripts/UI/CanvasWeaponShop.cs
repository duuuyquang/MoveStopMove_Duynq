using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasWeaponShop : UICanvas
{
    public void Start()
    {
        DataWeaponEach data = DataWeapon.FetchByID(2);
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }
}
