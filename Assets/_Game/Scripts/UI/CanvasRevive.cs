using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRevive : UICanvas
{
    [SerializeField] TextMeshProUGUI counterText;
    [SerializeField] Transform loadingCirle;

    private Vector3 rotateAxis = Vector3.back;

    private float rotateSpd = 360 / Const.CHARACTER_REVIVE_COUNTDOWN_SECS;

    public void OnOpen(int totalSecs)
    {
        loadingCirle.transform.eulerAngles = Vector3.zero;
        SetCounterText(totalSecs);
    }

    public void Update()
    {
        loadingCirle.Rotate(rotateAxis, rotateSpd * Time.deltaTime);
    }
    public void ReviveButton()
    {
        UIManager.Instance.CloseAll();
        SoundManager.Instance.PlayBtnClick();
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
