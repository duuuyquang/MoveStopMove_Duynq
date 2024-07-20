using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasWin : UICanvas
{
    [SerializeField] TextMeshProUGUI scoreText;

    public void SetBestScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().SetCoinText(GameManager.Instance.TotalCoin);
        LevelManager.Instance.OnInit(0);
    }

    public void NextButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().SetCoinText(GameManager.Instance.TotalCoin);
        LevelManager.Instance.OnInitNextLevel();
    }
}
