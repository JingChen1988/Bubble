using UnityEngine;

public class RangeAtt : PropertyAttribute
{
    public int min;
    public int max;

    public RangeAtt(int mi, int ma)
    {
        min = mi;
        max = ma;
    }
}
