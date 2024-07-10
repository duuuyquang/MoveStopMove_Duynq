using UnityEngine;

public class WeaponUnit : MonoBehaviour
{
    public WeaponType WeaponType;
    private Transform tf;

    public Transform TF
    {
        get
        {
            if (tf == null)
            {
                tf = transform;
            }

            return tf;
        }
    }
}
