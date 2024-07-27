using TMPro;
using UnityEngine;

public class CanvasWin : UICanvas
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Animator animator;

    public void OnOpen()
    {
        animator.SetTrigger(Const.ANIM_NAME_CANVAS_WIN);
    }

    public void SetBestScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void MainMenuButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().SetCoinText(GameManager.Instance.TotalCoin);
        LevelManager.Instance.OnInit(PlayerData.Instance.curLevel);
        SoundManager.Instance.PlayBtnClick();
    }

    public void NextButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasMainMenu>().SetCoinText(GameManager.Instance.TotalCoin);
        LevelManager.Instance.OnInitNextLevel();
        SoundManager.Instance.PlayBtnClick();
    }
}
