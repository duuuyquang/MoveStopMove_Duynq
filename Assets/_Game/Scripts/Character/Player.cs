using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    //[SerializeField] CombatPointText combatPointTextPrefab;

    [SerializeField] ParticleSystem winPartical;
    [SerializeField] ParticleSystem losePartical;

    private Dictionary<WeaponType, int> ownedWeaponTypes = new Dictionary<WeaponType, int>();

    private Dictionary<ItemType, int> ownedItemTypes = new Dictionary<ItemType, int>();

    private ParticleSystem curFinishPartical = null;

    protected override void Update()
    {
        base.Update();
        if (GameManager.IsState(GameState.GamePlay) && !IsStatus(StatusType.Dead))
        {
            ListenControllerInput();
            UpdateAttackStatus();
        }

        if (GameManager.IsState(GameState.Finish) && IsStatus(StatusType.Normal))
        {
            ChangeStatus(StatusType.Win);
            OnWin();
        }

        if(GameManager.IsState(GameState.SkinShop))
        {
            SetSkinShopPose();
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
        UpdateOwnedWeapon(WeaponType.Axe);
        weaponType = WeaponType.Axe;
        base.OnInit();
        ChangeWeapon(weaponType);
        ChangeHead(ItemType.None);
        ChangePants(ItemType.None);
        ChangeShield(ItemType.None);
        ChangeSet(ItemType.None);
        ChangeColor(ColorType.Yellow);
        PlayerController.Instance.OnInit();
        InitTransform();
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

    protected override void InitBasicStats()
    {
        base.InitBasicStats();
        Name = "AbcXz";
        CombatPoint = 0;
    }

    public override void OnDespawn()
    {
        GameManager.ChangeState(GameState.Finish);
        UIManager.Instance.OpenUI<CanvasLose>();
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

    protected override void OnDead()
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
        Debug.Log(MoveSpeed);
        rb.velocity = PlayerController.Instance.CurDir * MoveSpeed * Time.fixedDeltaTime;
    }

    private void ListenControllerInput()
    {
        PlayerController.Instance.SetCurDirection();
    }

    private void UpdateAttackStatus()
    {
        if (IsStanding)
        {
            CheckAndProcessAttack();
        }
        else
        {
            StopAttack();
        }
    }

    public override void StopMoving()
    {
        base.StopMoving();
        rb.velocity = Vector3.zero;
        PlayerController.Instance.CurDir = Vector3.zero;
    }

    protected override void ShowCombatPointGainned(int point)
    {
        //CombatPointText prefab = Instantiate(combatPointTextPrefab, UIManager.Instance.GetUI<CanvasGamePlay>().transform);
        CombatPointText pointText = SimplePool.Spawn<CombatPointText>(PoolType.PointText, Vector3.zero, Quaternion.identity);
        pointText.TF.position = CameraFollower.Instance.Camera.WorldToScreenPoint(TF.position) + Vector3.up * 200f;
        pointText.SetPoint(point, Const.COMBAT_POINT_DEFAULT_SIZE * CurSize);
    }

    public override void ProcessOnTargetKilled(Character opponent)
    {
        base.ProcessOnTargetKilled(opponent);
        float gainnedCoin = 1 + (float)opponent.CombatPoint / 3;
        //Debug.Log(gainnedCoin);
        //Debug.Log("Bonus from shield: " + gainnedCoin * curShield.BonusGold * 0.01f);
        gainnedCoin += gainnedCoin * curShield.BonusGold * 0.01f;
        gainnedCoin = Mathf.Ceil(gainnedCoin - 0.5f); // round up if greater or equal x.5f
        GameManager.Instance.UpdateTotalCoin(gainnedCoin);
    }

    public void ChangeWeaponHolderMesh(WeaponType type)
    {
        WeaponHolder.ChangeWeapon(type);
    }

    public bool IsOwnedWeapon(WeaponType type) => ownedWeaponTypes.ContainsKey(type);
    public bool IsOwnedItem(ItemType type) => ownedItemTypes.ContainsKey(type);

    public void UpdateOwnedWeapon(WeaponType type)
    {
        if (!IsOwnedWeapon(type))
        {
            // TODO: add owned skin later
            ownedWeaponTypes[type] = 1;
        }
    }

    public void AddOwnedItem(ItemType type)
    {
        if (!IsOwnedItem(type))
        {
            // TODO: add owned skin later
            ownedItemTypes[type] = 1;
        }
    }

    private void RotateAround()
    {
        TF.Rotate(Vector3.up, 60f * Time.deltaTime);
    }

    public void SetSkinShopPose()
    {
        RotateAround();
        LevelManager.Instance.Player.ChangeAnim(Const.ANIM_NAME_SHOP);
    }
}
