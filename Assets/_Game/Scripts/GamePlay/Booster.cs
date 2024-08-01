using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BoosterType { None, Attack, Speed }
public class Booster : GameUnit
{
    public const float ATTACK_RANGE_INDEX = 3f;
    public const float ATTACK_SPD_INDEX = 2f;
    public const float ATTACK_BULLET_SIZE_INDEX = 1f;

    public const float SPEED_ACTIVE_SECS = 10f;
    public const float SPEED_INDEX = 0.75f;

    [SerializeField] Renderer boxRenderer;
    [SerializeField] ColorDataSO colorDataSO;
    [SerializeField] Transform boxTF;
    public BoosterType Type { get; private set; } = BoosterType.None;
    public float MoveSpdMultiplier { get; private set; } = 1.0f;
    public float AtkRange { get; private set; } = 0f;

    private Vector3 ceilPos;
    private Vector3 floorPos;
    private Vector3 direction;

    public void OnInit()
    {
        Type = (BoosterType) Random.Range(1, Enum.GetNames(typeof(BoosterType)).Length);
        ChangeColor(Type);
        UpdateStatsByType();
        ceilPos = new Vector3(TF.position.x, 0f, TF.position.z);
        floorPos = new Vector3(TF.position.x, -0.5f, TF.position.z);
        direction = floorPos;
    }

    public void Start()
    {
    }

    public void Update()
    {
        Spin();
        MoveUpDown();
    }

    private void Spin()
    {
        boxTF.Rotate(Vector3.up, 100f * Time.deltaTime);
    }

    private void MoveUpDown()
    {
        if (Vector3.Distance(boxTF.position, ceilPos) <= 0.1f)
        {
            direction = floorPos;
        }
        else if (Vector3.Distance(boxTF.position, floorPos) <= 0.1f)
        {
            direction = ceilPos;
        }

        boxTF.position = Vector3.MoveTowards(boxTF.position, direction, Time.deltaTime * 0.5f);
    }

    private void ChangeColor(BoosterType type)
    {
        switch(type)
        {
            case BoosterType.None:
                break;
            case BoosterType.Attack:
                boxRenderer.material = colorDataSO.GetMat(ColorType.Red);
                break;
            case BoosterType.Speed:
                boxRenderer.material = colorDataSO.GetMat(ColorType.Green);
                break;
        }
    }

    public void OnDespawn()
    {
        BoosterManager.Instance.SpawnedBoostersList.Remove(this);
        SimplePool.Despawn(this);
    }    

    public void UpdateStatsByType()
    {
        switch(Type)
        {
            case BoosterType.None:
                break;
            case BoosterType.Attack:
                AtkRange = ATTACK_RANGE_INDEX;
                break;
            case BoosterType.Speed:
                MoveSpdMultiplier = SPEED_INDEX;
                break;
        }
    }
}
