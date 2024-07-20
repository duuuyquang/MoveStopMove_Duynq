using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField] ParticleSystem winPartical;
    [SerializeField] ParticleSystem losePartical;

    private ParticleSystem curFinishPartical;

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
        InitTransform();

        CombatPoint = 0;
        SetupSizeByInitCombatPoint(CombatPoint);

        InitSavedData();
        LoadSavedDataToPlayer();
    }

    private void InitSavedData()
    {
        PlayerData.GetData();

        if(PlayerData.Instance.weaponsState.Count == 0)
        {
            PlayerData.Instance.weaponsState = itemDataSO.InitAllWeaponsState();
            PlayerData.SaveData();
        }

        if (PlayerData.Instance.itemsState.Count == 0)
        {
            PlayerData.Instance.itemsState = itemDataSO.InitAllItemsState();
            PlayerData.SaveData();
        }
    }

    private void LoadSavedDataToPlayer()
    {
        Name = PlayerData.Instance.name;
        ChangeColor(PlayerData.Instance.colorType);
        ChangeWeapon(PlayerData.Instance.weaponType);
        ChangeToSavedItems();
    }

    public void InitTransform()
    {
        TF.position = Vector3.zero;
        TF.eulerAngles = new Vector3(0, 210, 0);
    }

    private void SetFinishPartical(ParticleSystem partical, float delay)
    {
        if (curFinishPartical != partical)
        {
            if (curFinishPartical) curFinishPartical.Stop();
            curFinishPartical = partical;
            Invoke(nameof(PlayFinishPartical), delay);
        }
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
        if(curFinishPartical)
        {
            curFinishPartical.Stop();
            curFinishPartical.gameObject.SetActive(false);
        }
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        GameManager.ChangeState(GameState.Finish);
        GameManager.Instance.OnLose();
    }

    public void OnPlay()
    {
        atkRangeTF.gameObject.SetActive(true);
    }

    private void OnWin()
    {
        TF.eulerAngles = new Vector3(0, 200, 0);
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

    protected override void DetectNearestTarget()
    {
        base.DetectNearestTarget();
        foreach (Character enemy in targetsInRange)
        {
            if (enemy)
            {
                enemy.ToggleTargetIndicator(false);
            }
        }

        if (curTargetChar != null)
        {
            curTargetChar.ToggleTargetIndicator(true);
        }
    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
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
    }

    private void ListenControllerInput()
    {
        PlayerController.Instance.SetCurDirection();
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
        pointText.SetPoint(point, Const.COMBAT_POINT_DEFAULT_SIZE * CurSize);
    }

    public override void ProcessOnTargetKilled(Character opponent)
    {
        base.ProcessOnTargetKilled(opponent);
        float gainnedCoin = 1 + (float)opponent.CombatPoint / 3;
        gainnedCoin += gainnedCoin * BonusGoldMultiplier * 0.01f;
        gainnedCoin = Mathf.Ceil(gainnedCoin - 0.49f); // round up if greater or equal x.5f
        GameManager.Instance.UpdateTotalCoin(gainnedCoin);
    }

    public void RotateAround()
    {
        TF.Rotate(Vector3.up, 60f * Time.deltaTime);
    }

    public void SetMainMenuPose()
    {
        InitTransform();
        ChangeAnim(Const.ANIM_NAME_IDLE);
    }

    public void ChangeToSavedItems()
    {
        ChangeSet(ItemType.None);
        if (PlayerData.Instance.setType != ItemType.None) {
            ChangeSet(PlayerData.Instance.setType);
        }
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
