using TMPro;
using UnityEngine;

public class CanvasRevive : UICanvas
{
    [SerializeField] TextMeshProUGUI counterText;
    public void ReviveButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasGamePlay>().OnOpen();
        GameManager.Instance.OnPlayRevive();
    }

    public void CancelButton()
    {
        UIManager.Instance.CloseAll();
        UIManager.Instance.OpenUI<CanvasLose>().OnOpen();
        GameManager.Instance.OnLoseRevive();
    }

    public void SetCounterText(int count)
    {
        counterText.text = count.ToString();
    }
}
