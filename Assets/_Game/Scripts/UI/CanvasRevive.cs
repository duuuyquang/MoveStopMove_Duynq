using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasRevive : UICanvas
{
    [SerializeField] TextMeshProUGUI counterText;
    public void ReviveButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasGamePlay>().OnOpen(this);
    }

    public void CancelButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().OnOpen();
        GameManager.Instance.StopReviveTimer();
        LevelManager.Instance.OnInitCurLevel();
    }

    public void SetCounterText(int count)
    {
        counterText.text = count.ToString();
    }
}
