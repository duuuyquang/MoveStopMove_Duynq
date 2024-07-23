using UnityEngine;

public enum ColorType
{
    None    = 0,
    White   = 1,
    Blue    = 2,
    Green   = 3,
    Red     = 4,
    DarkRed = 5,
    Yellow  = 6,
    Purpil  = 7,
    Pink    = 8,
    Orange  = 9,
    Grey    = 10,
    Skin    = 11,
}

[CreateAssetMenu(menuName = "ColorDataSO")]

public class ColorDataSO : ScriptableObject
{
    [SerializeField] Material untouchable;
    [SerializeField] Material[] materials;
    [SerializeField] Material[] materialGUIs;
    [SerializeField] Material[] materialDeaths;
    [SerializeField] Color[] colors;

    public Material GetMat(ColorType index)
    {
        return materials[(int)index];
    }

    public Material GetMatGUI(ColorType index)
    {
        return materialGUIs[(int)index];
    }

    public Material GetMatDeath(ColorType index)
    {
        return materialDeaths[(int)index];
    }

    public Material GetMatUntouchable()
    {
        return untouchable;
    }

    public Color GetColor(ColorType index)
    {
        return colors[(int)index];
    }
}
