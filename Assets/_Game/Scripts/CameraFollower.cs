using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : Singleton<CameraFollower>
{

    private const float EULER_X_GAME_MENU = 20f;
    private const float EULER_X_GAME_PLAY = 60f;

    private Vector3 OFFSET_GAME_MENU = new Vector3(0f, 3f, -6f);
    private Vector3 OFFSET_GAME_PLAY = new Vector3(0f, 10f, -9f);

    public Vector3 offset = Vector3.zero;
    [SerializeField] private float speed = 1f;

    private Vector3 targetPos;

    [SerializeField] private Player player;
    public Player Player => player;

    [SerializeField] private Camera cam;
    public Camera Camera => cam;

    public Transform TF;


    void LateUpdate()
    {
        if (!player)
        {
            player = LevelManager.Instance.Player;
        }
        else if (GameManager.IsState(GameState.GamePlay))
        {
            targetPos = player.TF.position + new Vector3(0, player.CurAttackRange * 2.7f, -player.CurAttackRange * 1.9f);
            TF.position = Vector3.MoveTowards(TF.position, targetPos, speed * Time.fixedDeltaTime);
        }
    }

    public void OnInit()
    {
        SetupMenuMode();
    }

    public void SetupMenuMode()
    {
        TF.eulerAngles = new Vector3(EULER_X_GAME_MENU, 0, 0);
        TF.position = OFFSET_GAME_MENU;
    }

    public void SetupGamePlayMode()
    {
        StartCoroutine(IECameraTransition());
    }

    IEnumerator IECameraTransition()
    {
        int desiredFPS = 45;
        float count = 0f;
        float amountToRotate = EULER_X_GAME_PLAY - TF.eulerAngles.x;
        float rotateUnit = amountToRotate / desiredFPS;
        while (count < amountToRotate)
        {
            TF.Rotate(rotateUnit, 0, 0);
            count += rotateUnit;
            yield return new WaitForEndOfFrame();
        }
    }

    public Player GetPlayer()
    {
        return player;
    }
}
