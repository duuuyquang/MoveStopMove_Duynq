using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using UnityEngine;

public enum ColorType
{
    White = 0,
    Blue = 1,
    Green = 2,
    Red = 3,
    Yellow = 4,
    Purpil = 5,
    Pink = 6,
    Orange = 7,
    Grey = 8
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
