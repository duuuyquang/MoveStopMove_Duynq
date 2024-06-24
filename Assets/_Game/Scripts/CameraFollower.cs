using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : Singleton<CameraFollower>
{

    private const float EULER_X_GAME_MENU = 20f;
    private const float EULER_X_GAME_PLAY = 50f;

    private Vector3 OFFSET_GAME_MENU = new Vector3(0f, 5f, -7f);
    private Vector3 OFFSET_GAME_PLAY = new Vector3(0f, 10f, -9f);

    public Vector3 offset = Vector3.zero;
    [SerializeField] private float speed = 1f;

    private Vector3 targetPos;

    public Player player;
    public Transform TF;

    void LateUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        else if (!GameManager.IsState(GameState.Finish))
        {
            targetPos = player.TF.position + offset;
            TF.position = Vector3.MoveTowards(TF.position, targetPos, speed * Time.fixedDeltaTime);
        }
    }

    public void OnInit()
    {
        TF.position = offset;
    }
}
