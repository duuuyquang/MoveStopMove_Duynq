using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] float bonusAttackRange = 0;
    [SerializeField] float bonusSpeed = 0;
    [SerializeField] float spinSpeed = 15f;
    [SerializeField] Vector3 rotateAxis = Vector3.right;
    [SerializeField] bool isReturn = false;
    [SerializeField] bool isGrab = false;

    public float BonusAttackRange => bonusAttackRange;
    public float BonusSpeed => bonusSpeed;
    public float SpinSpeed => spinSpeed;
    public Vector3 RotateAxis => rotateAxis;
    public bool IsReturn => isReturn;
    public bool IsGrab => isGrab;
}
