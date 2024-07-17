using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

public enum ColorType
{
    None    = 0,
    White   = 1,
    Blue    = 2,
    Green   = 3,
    Red     = 4,
    Yellow  = 5,
    Purpil  = 6,
    Pink    = 7,
    Orange  = 8,
    Grey    = 9
}

[CreateAssetMenu(menuName = "ColorDataSO")]

public class ColorDataSO : ScriptableObject
{
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

    public Color GetColor(ColorType index)
    {
        return colors[(int)index];
    }
}
