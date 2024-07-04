using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Character;
using static UnityEngine.GraphicsBuffer;

public class Indicator : MonoBehaviour
{
    [SerializeField] Transform indicator;
    private Character targetChar;
    private Camera cam;
    private float offset = 60f;

    [SerializeField] TextMeshProUGUI nameTextMesh;
    [SerializeField] TextMeshProUGUI pointTextMesh;
    [SerializeField] Image backgroundImage;

    public ColorDataSO colorDataSO;

    private void Start()
    {
        cam = CameraFollower.Instance.GetCameraComponent();
    }
    private void LateUpdate()
    {
        if (targetChar)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(targetChar.TF.position);
            if(IsInScreen(screenPos))
            {
                nameTextMesh.gameObject.SetActive(true);
            } 
            else
            {
                nameTextMesh.gameObject.SetActive(false);
            }
            float restrictedX = Mathf.Max(Mathf.Min(Screen.width - offset, screenPos.x), 0 + offset);
            float restrictedY = Mathf.Max(Mathf.Min(Screen.height - offset * 3.5f, screenPos.y), 0 - offset * 1.5f);
            screenPos = new Vector3(restrictedX, restrictedY, 0);
            indicator.transform.position = screenPos + Vector3.up * offset * 3f;
        }

        if(targetChar && targetChar.IsStatus(StatusType.Dead) || GameManager.IsState(GameState.Finish))
        {
            Destroy(gameObject);
        }
    }

    public void OnInit(Character character)
    {
        targetChar = character;
        nameTextMesh.text = character.Name;
        pointTextMesh.text = character.CombatPoint.ToString();
        backgroundImage.material = colorDataSO.GetMatGUI(character.ColorType);
    }

    public void UpdateName(string name)
    {
        nameTextMesh.text = name;
    }

    public void UpdateCombatPoint(int point)
    {
        pointTextMesh.text = point.ToString();
    }

    public bool IsInScreen(Vector3 screenPos)
    {
        if(screenPos.x  > Screen.width || screenPos.x < 0 || screenPos.y > Screen.height || screenPos.y < 0)
        {
            return false;
        } 
        return true;
    }
}
