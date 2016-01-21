using UnityEngine;
/// <summary>
/// 颜色工具
/// </summary>
public static class ColorTool
{
    const float Max = 255;
    public static Color Black = ColorTool.Convert(0, 0, 0);
    public static Color White = ColorTool.Convert(255, 255, 255);
    public static Color WhiteTran = ColorTool.Convert(255, 255, 255, 80);
    public static Color Red = ColorTool.Convert(255, 0, 0);
    public static Color Green = ColorTool.Convert(0, 255, 0);
    public static Color Blue = ColorTool.Convert(0, 0, 255);
    public static Color Gray = ColorTool.Convert(122, 122, 122);
    public static Color Orange = ColorTool.Convert(255, 111, 0);

    public static Color Convert(float value) { return new Color(value / Max, value / Max, value / Max); }
    public static Color Convert(int r, int b, int g, int a = 255) { return new Color(r / Max, b / Max, g / Max, a / Max); }
}
