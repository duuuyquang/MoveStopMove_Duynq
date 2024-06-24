using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasLose : UICanvas
{
    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>();
    }

    public void RetryButton()
    {
        Close(0);
    }
}
