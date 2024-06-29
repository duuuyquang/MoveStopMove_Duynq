using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasGamePlay : UICanvas
{
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI aliveNumText;

    public override void Setup()
    {
        base.Setup();
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = "Level " + level;
    }

    public void UpdateAliveNumText(int num)
    {
        aliveNumText.text = "Alive: " + num;
    }

    public void SettingsButton()
    {
        UIManager.Instance.OpenUI<CanvasSettings>().SetState(this);
        GameManager.ChangeState(GameState.Setting);
    }
}
