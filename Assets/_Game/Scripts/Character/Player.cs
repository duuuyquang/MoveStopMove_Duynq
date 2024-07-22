using UnityEngine;

public class Player : Character
{
    [SerializeField] ParticleSystem winPartical;
    [SerializeField] ParticleSystem losePartical;

    private ParticleSystem curFinishPartical;

    public static Vector3 InitPosition = Vector3.zero;

    public int reviveTimes;

    protected override void Update()
    {
        base.Update();
        if (GameManager.IsState(GameState.GamePlay))
        {
            ListenControllerInput();
            CheckToProcessAttack();
        }
    }

    void FixedUpdate()
    {
        if (GameManager.IsState(GameState.GamePlay))
        {
            ProcessMoving();
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        PlayerController.Instance.OnInit();
        InitTransform(InitPosition);
        reviveTimes = 1;
        CombatPoint = 0; 
        SetupSizeByInitCombatPoint(CombatPoint);
        InitSavedData();
        LoadSavedData();
    }

    private void InitSavedData()
    {
        PlayerData.GetData();

        if(PlayerData.Instance.weaponsState.Count == 0)
        {
            PlayerData.Instance.weaponsState = itemDataSO.InitAllWeaponsState();
            PlayerData.Instance.weaponsState[WeaponType.Axe] = ItemState.Bought;
            PlayerData.SaveData();
        }

        if (PlayerData.Instance.itemsState.Count == 0)
        {
            PlayerData.Instance.itemsState = itemDataSO.InitAllItemsState();
            PlayerData.SaveData();
        }
    }

    private void LoadSavedData()
    {
        Name = PlayerData.Instance.name;
        ChangeColor(PlayerData.Instance.colorType);
        ChangeWeapon(PlayerData.Instance.weaponType);
        ChangeToSavedItems();
    }

    public void InitTransform(Vector3 pos)
    {
        TF.position = pos;
        TF.eulerAngles = Cache.GetVector(0f, 210f, 0f);
    }

    private void SetFinishPartical(ParticleSystem partical, float delay)
    {
        StopFinishPartical();
        curFinishPartical = partical;
        Invoke(nameof(PlayFinishPartical), delay);
    }

    private void PlayFinishPartical()
    {
        if (curFinishPartical)
        {
            curFinishPartical.gameObject.SetActive(true);
            curFinishPartical.Play();
        }
    }

    private void StopFinishPartical()
    {
        if (curFinishPartical)
        {
            curFinishPartical.Stop();
            curFinishPartical.gameObject.SetActive(false);
        }
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        if(reviveTimes <= 0)
        {
            GameManager.Instance.OnLose();
        }
        else
        {
            reviveTimes--;
            GameManager.Instance.OnRevive();
        }
    }

    public void OnWin()
    {
        TF.eulerAngles = Cache.GetVector(0f, 200f, 0f);
        ChangeAnim(Const.ANIM_NAME_DANCE);
        StopMoving();
        ToggleAtkRangeTF(false);
        SetFinishPartical(winPartical, 0.5f);
    }

    public override void OnDead()
    {
        base.OnDead();
        SetFinishPartical(losePartical, 0.5f);
    }

    public void OnRevive()
    {
        base.OnInit();
        PlayerController.Instance.OnInit();
        InitTransform(TF.position);
        StopFinishPartical();
        SetupSizeByInitCombatPoint(CombatPoint);
        LoadSavedData();
        InitIndicator();
    }

    public override void ChangeAnimByCurStatus()
    {
        base.ChangeAnimByCurStatus();
        if (IsStatus(StatusType.Normal))
        {
            LookAtCurDirection();
        }
    }

    private void LookAtCurDirection()
    {
        if (!IsStanding)
        {
            TF.LookAt(TF.position + PlayerController.Instance.CurDir);
        }
    }

    private void ProcessMoving()
    {
        rb.velocity = PlayerController.Instance.CurDir * MoveSpeed * Time.fixedDeltaTime;
        ChangeAnimByCurStatus();
    }

    private bool CheckMovingConditions => !IsStatus(StatusType.Dead) && !IsStatus(StatusType.Win);

    private void ListenControllerInput()
    {
        if (CheckMovingConditions) PlayerController.Instance.SetCurDirection();
    }

    public override void StopMoving()
    {
        base.StopMoving();
        rb.velocity = Vector3.zero;
        PlayerController.Instance.CurDir = Vector3.zero;
    }

    private Vector3 OffsetAbovePlayerPos => CameraFollower.Instance.Camera.WorldToScreenPoint(TF.position) + Vector3.up * 200f;

    protected override void ShowCombatPointGainned(int point)
    {
        CombatPointText pointText = SimplePool.Spawn<CombatPointText>(PoolType.PointText, Vector3.zero, Quaternion.identity);
        pointText.TF.position = OffsetAbovePlayerPos;
        pointText.SetPoint(point, Const.COMBAT_TEXT_DEFAULT_SIZE * CurSize);
    }

    public override void OnTargetKilled(Character opponent)
    {
        base.OnTargetKilled(opponent);
        float gainnedCoin = (1 + (float)opponent.CombatPoint / 3) * BonusGoldMultiplier * 0.01f;
        gainnedCoin = Mathf.Ceil(gainnedCoin - 0.49f); // round up if greater or equal x.5f
        GameManager.Instance.UpdateTotalCoin(gainnedCoin);
    }

    public void RotateAround(float speed = 60f)
    {
        TF.Rotate(Vector3.up, speed * Time.deltaTime);
    }

    public void SetMainMenuPose()
    {
        InitTransform(InitPosition);
        ChangeAnim(Const.ANIM_NAME_IDLE);
    }

    public void ChangeToSavedItems()
    {
        ChangeSet(ItemType.None);
        if (PlayerData.Instance.setType != ItemType.None) ChangeSet(PlayerData.Instance.setType);
        else
        {
            ChangeHead(PlayerData.Instance.headType);
            ChangePants(PlayerData.Instance.pantsType);
            ChangeShield(PlayerData.Instance.shieldType);
            ChangeColor(PlayerData.Instance.colorType);
        }
    }

    public void ChangeToSavedWeapon()
    {
        ChangeWeapon(PlayerData.Instance.weaponType);
    }

    protected override void UpdateBonusStatsFromItem(Item item)
    {
        base.UpdateBonusStatsFromItem(item);
        bonusMoveSpeed = baseMoveSpeed * item.BonusMoveSpeed * 0.01f;
    }
}
