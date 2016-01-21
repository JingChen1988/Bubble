using UnityEngine;
/// <summary>
/// 数学工具类
/// </summary>
public static class MathD
{
    public const float Deviation = .01f;//小数差值

    /// <summary>
    /// 两个小数是否相近
    /// </summary>
    public static bool isEqual(float a, float b)
    {
        return (a - b > 0 ? a - b : b - a) <= Deviation;
    }

    /// <summary>
    /// 两个小数是否相近
    /// </summary>
    public static bool isEqual(float a, float b, float devi)
    {
        return (a - b > 0 ? a - b : b - a) <= devi;
    }

    /// <summary>
    /// 求x的n次方
    /// </summary>
    public static int Pow(int x, int n)
    {
        int num = n == 0 ? 1 : x;
        while (--n > 0) { num *= x; }
        return num;
    }
}
