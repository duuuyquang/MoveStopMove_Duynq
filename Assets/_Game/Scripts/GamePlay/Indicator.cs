using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : GameUnit
{
    private Character targetChar;
    private float offsetScreen = 70f;
    private float offsetChar = 120f;
    private float offsetPointer = 50f;

    [SerializeField] TextMeshProUGUI nameTextMesh;
    [SerializeField] TextMeshProUGUI pointTextMesh;
    [SerializeField] Image backgroundImage;
    [SerializeField] Image pointerImage;
    [SerializeField] Transform pointerImageTF;
    [SerializeField] TextMeshProUGUI curStateText;

    public ColorDataSO colorDataSO;

    private float restrictedX;
    private float restrictedY;

    Enemy enemy;

    private void LateUpdate()
    {
        if(targetChar)
        {
#if UNITY_INCLUDE_TESTS
            curStateText.text = "";
#endif
            Vector3 charPosOnScreen = CameraFollower.Instance.Camera.WorldToScreenPoint(targetChar.TF.position);
            if (IsInScreen(charPosOnScreen))
            {
                pointerImageTF.gameObject.SetActive(false);
                nameTextMesh.gameObject.SetActive(true);
                TF.position = charPosOnScreen + Vector3.up * offsetChar;
                #if UNITY_INCLUDE_TESTS
                if (targetChar is Enemy && enemy && enemy.CurState != null)
                {
                    curStateText.text = $"( {enemy.CurState.ToString()} )";
                }
                #endif
            }
            else
            {
                pointerImageTF.gameObject.SetActive(true);
                nameTextMesh.gameObject.SetActive(false);
                pointerImageTF.LookAt(TF.position);
                pointerImageTF.Rotate(0, 90, 0);

                Vector3 playerPos = CameraFollower.Instance.Camera.WorldToScreenPoint(LevelManager.Instance.Player.TF.position);
                Vector3 charPosOutScreen = playerPos + (CameraFollower.Instance.Camera.WorldToScreenPoint(targetChar.TF.position) - playerPos);
                restrictedX = Mathf.Max(0 + offsetScreen, Mathf.Min(Screen.width - offsetScreen, charPosOutScreen.x));
                restrictedY = Mathf.Max(0 + offsetScreen * 1.1f, Mathf.Min(Screen.height - offsetScreen * 1.1f, charPosOutScreen.y));
                charPosOutScreen = new Vector3(restrictedX, restrictedY, 0f);
                TF.position = charPosOutScreen;

                Vector3 pointerPos = charPosOutScreen + (charPosOutScreen - playerPos).normalized * offsetPointer;
                restrictedX = Mathf.Max(0, Mathf.Min(Screen.width, pointerPos.x));
                restrictedY = Mathf.Max(0 + offsetScreen * 0.5f, Mathf.Min(Screen.height - offsetScreen * 0.5f, pointerPos.y));
                pointerImageTF.position = new Vector3(restrictedX, restrictedY, 0f);
            }
        }

        if (!targetChar || targetChar && targetChar.IsStatus(StatusType.Dead))
        {
            SimplePool.Despawn(this);
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

        #if UNITY_INCLUDE_TESTS
        if(character is Enemy)
        {
            enemy = character.GetComponent<Enemy>();
        }
        #endif
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
