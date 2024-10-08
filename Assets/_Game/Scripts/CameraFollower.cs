using System.Collections;
using UnityEngine;

public enum CameraState { Normal, Transition }
public class CameraFollower : Singleton<CameraFollower>
{
    private CameraState curState = CameraState.Normal;
    private Vector3 targetPos;

    private Vector3 OFFSET_GAME_MENU    = new Vector3(-5f, 1.5f, -7f);
    private Vector3 OFFSET_GAME_PLAY    = new Vector3(0f, 10f, -9f);
    private Vector3 OFFSET_WEAPON_SHOP  = new Vector3(-0.75f, 1.1f, 1.5f);
    private Vector3 OFFSET_SKIN_SHOP    = new Vector3(0f, 1.5f, -7f);

    [SerializeField] private float speed = 1f;
    [field:SerializeField] public Camera Camera { get; private set; }

    public Transform TF;

    public bool IsState(CameraState state) => curState == state;

    private Vector3 PlayerInitalPos => Player.InitPosition + Vector3.down * 3f;
    //private Vector3 PlayerWeaponPos => LevelManager.Instance.Player.WeaponHolder.TF.position + Vector3.left * 0.3f + Vector3.down * 0.3f;
    private Vector3 PlayerWeaponPos => Player.InitPosition + Vector3.left * 0.6f + Vector3.down * 0.8f;
    private Vector3 SkinShopPos => Player.InitPosition + Vector3.down * 1.5f;
    private Vector3 GamePlayOffset => new Vector3(0, 1+ LevelManager.Instance.Player.CurAttackRange * 3f, -1 -LevelManager.Instance.Player.CurAttackRange * 2.5f);

    private RaycastHit viewBlockerHit;

    private Transform curViewBlocker;
    private Material cachedViewBlockerMat;
    [SerializeField] ColorDataSO colorDataSO;

    void LateUpdate()
    {
        if (GameManager.IsState(GameState.GamePlay) && IsState(CameraState.Normal))
        {
            targetPos = LevelManager.Instance.Player.TF.position + GamePlayOffset;
            TF.position = Vector3.MoveTowards(TF.position, targetPos, speed * Time.fixedDeltaTime);
            DetectViewBlocker();
        }
        
    }

    void DetectViewBlocker()
    {
        Ray ray = Camera.ScreenPointToRay(Camera.WorldToScreenPoint(LevelManager.Instance.Player.TF.position));
        if (Physics.Raycast(ray, out viewBlockerHit))
        {
            Transform objectHit = viewBlockerHit.transform;
            if (objectHit.CompareTag(Const.TAG_NAME_VIEW_BLOCKER))
            {
                if (curViewBlocker == null)
                {
                    curViewBlocker = objectHit;
                    cachedViewBlockerMat = curViewBlocker.GetComponent<Renderer>().material;
                    curViewBlocker.GetComponent<Renderer>().material = colorDataSO.GetMatUntouchable();
                }
            }
            else if (objectHit.CompareTag(Const.TAG_NAME_PLAYER)) 
            {
                if (curViewBlocker != null)
                {
                    curViewBlocker.GetComponent<Renderer>().material = cachedViewBlockerMat;
                    curViewBlocker = null;
                }
            }
        }
    }

    void ChangeState(CameraState state)
    {
        if(curState != state)
        {
            curState = state;
        }
    }

    public void OnInit()
    {
        SetupMenuMode();
    }

    public void InitPosition()
    {
        TF.position = OFFSET_GAME_MENU;
        TF.LookAt(PlayerWeaponPos);
    }

    public void SetupMenuMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_GAME_MENU, PlayerWeaponPos, 15f));
    }

    public void SetupWeaponShopMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_WEAPON_SHOP, PlayerWeaponPos, 15f));
    }

    public void SetupGamePlayMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_GAME_PLAY, PlayerInitalPos, 20f));
    }

    public void SetupSkinShopMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_SKIN_SHOP, SkinShopPos, 10f));
    }

    IEnumerator IEGenericTransitionAndLookAt(Vector3 finalPos, Vector3 lookAtPos, float speed)
    {
        ChangeState(CameraState.Transition);
        while (Vector3.Distance(TF.position, finalPos) > 0f)
        {
            TF.position = Vector3.MoveTowards(TF.position, finalPos, speed * Time.deltaTime);
            TF.LookAt(lookAtPos);
            yield return new WaitForEndOfFrame();
        }
        ChangeState(CameraState.Normal);
    }
}
