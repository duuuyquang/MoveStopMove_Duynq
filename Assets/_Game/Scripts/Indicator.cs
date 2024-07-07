using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Character;

public class Indicator : MonoBehaviour
{
    [SerializeField] Transform indicator;
    private Character targetChar;
    private float offsetScreen = 70f;
    private float offsetChar = 120f;
    private float offsetPointer = 50f;

    [SerializeField] TextMeshProUGUI nameTextMesh;
    [SerializeField] TextMeshProUGUI pointTextMesh;
    [SerializeField] Image backgroundImage;
    [SerializeField] Image pointerImage;

    public ColorDataSO colorDataSO;

    private void LateUpdate()
    {
        if(targetChar)
        {
            Vector3 charPosOnScreen = CameraFollower.Instance.Camera.WorldToScreenPoint(targetChar.TF.position);
            if (IsInScreen(charPosOnScreen))
            {
                pointerImage.gameObject.SetActive(false);
                nameTextMesh.gameObject.SetActive(true);
                indicator.position = charPosOnScreen + Vector3.up * offsetChar;
            }
            else
            {
                pointerImage.gameObject.SetActive(true);
                nameTextMesh.gameObject.SetActive(false);
                pointerImage.transform.LookAt(indicator.position);
                pointerImage.transform.Rotate(0, 90, 0);

                Vector3 playerPos = CameraFollower.Instance.Camera.WorldToScreenPoint(LevelManager.Instance.Player.TF.position);
                Vector3 charPosOutScreen = playerPos + (CameraFollower.Instance.Camera.WorldToScreenPoint(targetChar.TF.position) - playerPos);
                float restrictedX = Mathf.Max(0 + offsetScreen, Mathf.Min(Screen.width - offsetScreen, charPosOutScreen.x));
                float restrictedY = Mathf.Max(0 + offsetScreen * 1.1f, Mathf.Min(Screen.height - offsetScreen * 1.1f, charPosOutScreen.y));
                charPosOutScreen = new Vector3(restrictedX, restrictedY, 0f);
                indicator.position = charPosOutScreen;

                Vector3 pointerPos = charPosOutScreen + (charPosOutScreen - playerPos).normalized * offsetPointer;
                restrictedX = Mathf.Max(0, Mathf.Min(Screen.width, pointerPos.x));
                restrictedY = Mathf.Max(0 + offsetScreen * 0.5f, Mathf.Min(Screen.height - offsetScreen * 0.5f, pointerPos.y));
                pointerImage.transform.position = new Vector3(restrictedX, restrictedY, 0f);
            }
        }

        if (targetChar && targetChar.IsStatus(StatusType.Dead) || GameManager.IsState(GameState.Finish))
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
        pointerImage.material = colorDataSO.GetMatGUI(character.ColorType);
        nameTextMesh.color = colorDataSO.GetColor(character.ColorType);
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
        if(screenPos.x  > Screen.width || screenPos.x < 0 || screenPos.y > Screen.height - offsetScreen * 0.5f || screenPos.y < 0)
        {
            return false;
        } 
        return true;
    }
}
