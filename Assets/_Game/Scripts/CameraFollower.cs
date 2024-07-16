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
    private Vector3 OFFSET_SKIN_SHOP    = new Vector3(0f, 2f, -6f);

    [SerializeField] private float speed = 1f;
    [SerializeField] private Player player;
    [SerializeField] private Camera cam;

    public Player Player => player;
    public Camera Camera => cam;

    public Transform TF;

    public bool IsState(CameraState state) => curState == state;

    private Vector3 initalPos => Vector3.zero + Vector3.back * 3f;
    private Vector3 playerWeaponPos => LevelManager.Instance.Player.WeaponHolder.TF.position + Vector3.left * 0.3f + Vector3.down * 0.3f;

    void LateUpdate()
    {
        if (!player)
        {
            player = LevelManager.Instance.Player;
        }
        else if (GameManager.IsState(GameState.GamePlay) && IsState(CameraState.Normal))
        {
            targetPos = player.TF.position + new Vector3(0, player.CurAttackRange * 2.7f, -player.CurAttackRange * 1.9f);
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
        //InitPosition();
        SetupMenuMode();
    }

    public void InitPosition()
    {
        TF.position = OFFSET_GAME_MENU;
        TF.LookAt(playerWeaponPos);
    }

    public void SetupMenuMode()
    {
        //TF.eulerAngles = new Vector3(EULER_X_GAME_MENU, 0, 0);
        //TF.position = OFFSET_GAME_MENU;
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_GAME_MENU, playerWeaponPos, 15f));
    }

    public void SetupWeaponShopMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_WEAPON_SHOP, playerWeaponPos, 15f));
    }

    public void SetupGamePlayMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_GAME_PLAY, initalPos, 20f));
        //StartCoroutine(IECameraTransition());
    }

    public void SetupSkinShopMode()
    {
        StartCoroutine(IEGenericTransitionAndLookAt(OFFSET_SKIN_SHOP, initalPos, 15f));
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
