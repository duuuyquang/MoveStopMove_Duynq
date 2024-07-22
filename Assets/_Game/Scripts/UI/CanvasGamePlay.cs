using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasGamePlay : UICanvas
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI aliveNumText;

    public void OnOpen()
    {
        UpdateLevelText(LevelManager.Instance.CurLevel.Index);
        UpdateAliveCountText(GameManager.Instance.AliveCount);
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = "Level " + level;
    }

    public void UpdateAliveCountText(int num)
    {
        aliveNumText.text = "Alive: " + num;
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
        GameManager.Instance.OnSetting();
    }
}
