using UnityEngine;

public class Weapon : WeaponUnit
{
    [SerializeField] float bonusAttackRange = 0;
    [SerializeField] float bonusSpeed = 0;
    [SerializeField] float spinSpeed = 15f;

    [Header("Special Effects")]
    [SerializeField] bool isReturn = false;
    [SerializeField] bool isGrab = false;

    [Header("Inital Transform Stats")]
    [SerializeField] Vector3 initPosition;
    [SerializeField] Vector3 initEulerAngles;
    [SerializeField] Vector3 rotateAxis = Vector3.right;

    public Vector3 RotateAxis => rotateAxis;

    public float BonusAttackRange => bonusAttackRange;
    public float BonusSpeed => bonusSpeed;
    public float SpinSpeed => spinSpeed;
    public bool IsReturn => isReturn;
    public bool IsGrab => isGrab;

    public void OnInit()
    {
        TF.localPosition = initPosition;
        TF.localEulerAngles = initEulerAngles;
    }
}
