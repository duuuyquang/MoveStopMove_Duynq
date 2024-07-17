using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Camera cam;
    public Camera Camera => cam;

    public Transform TF;

    public bool IsState(CameraState state) => curState == state;

    private Vector3 gamePlayInitalPos => Vector3.zero + Vector3.down * 3f;
    private Vector3 playerWeaponPos => LevelManager.Instance.Player.WeaponHolder.TF.position + Vector3.left * 0.3f + Vector3.down * 0.3f;
    private Vector3 skinShopPos => Vector3.zero + Vector3.down * 1.5f;
    private Vector3 gamePlayOffset => new Vector3(0, LevelManager.Instance.Player.CurAttackRange * 3.1f, -LevelManager.Instance.Player.CurAttackRange * 2.5f);

    void LateUpdate()
    {
        if (GameManager.IsState(GameState.GamePlay) && IsState(CameraState.Normal))
        {
            targetPos = LevelManager.Instance.Player.TF.position + gamePlayOffset;
            TF.position = Vector3.MoveTowards(TF.position, targetPos, speed * Time.fixedDeltaTime);
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
        TF.LookAt(playerWeaponPos);
    }

    public void SetupMenuMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_GAME_MENU, playerWeaponPos, 15f));
    }

    public void SetupWeaponShopMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_WEAPON_SHOP, playerWeaponPos, 15f));
    }

    public void SetupGamePlayMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_GAME_PLAY, gamePlayInitalPos, 20f));
        //StartCoroutine(IECameraTransition());
    }

    public void SetupSkinShopMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_SKIN_SHOP, skinShopPos, 10f));
    }

    //IEnumerator IECameraTransition()
    //{
    //    int desiredFPS = 45;
    //    float count = 0f;
    //    float amountToRotate = EULER_X_GAME_PLAY - TF.eulerAngles.x;
    //    float rotateUnit = amountToRotate / desiredFPS;
    //    while (count < amountToRotate)
    //    {
    //        TF.Rotate(rotateUnit, 0, 0);
    //        count += rotateUnit;
    //        yield return new WaitForEndOfFrame();
    //    }
    //}

    IEnumerator IEGenericTransitionAndLookAt(Vector3 finalPos, Vector3 lookAtPos, float speed)
    {
        ChangeState(CameraState.Transition);
        while (Vector3.Distance(TF.position, finalPos) > 0.05f)
        {
            TF.position = Vector3.MoveTowards(TF.position, finalPos, speed * Time.deltaTime);
            TF.LookAt(lookAtPos);
            yield return new WaitForEndOfFrame();
        }
        ChangeState(CameraState.Normal);
    }
}
