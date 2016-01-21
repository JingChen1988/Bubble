using UnityEngine;
/// <summary>
/// Transform方法拓展
/// </summary>
public static class TransformExt
{
    public static Transform SetPosition(this Transform tran, float x, float y, float z, bool local = false)
    {
        if (local) tran.localPosition = new Vector3(x, y, z);
        else tran.position = new Vector3(x, y, z);
        return tran;
    }
    public static Transform SetX(this Transform tran, float x, bool local = false) { Vector3 p = local ? tran.localPosition : tran.position; return SetPosition(tran, x, p.y, p.z, local); }
    public static Transform SetY(this Transform tran, float y, bool local = false) { Vector3 p = local ? tran.localPosition : tran.position; return SetPosition(tran, p.x, y, p.z, local); }
    public static Transform SetZ(this Transform tran, float z, bool local = false) { Vector3 p = local ? tran.localPosition : tran.position; return SetPosition(tran, p.x, p.y, z, local); }
    public static Transform SetXY(this Transform tran, float x, float y, bool local = false) { Vector3 p = local ? tran.localPosition : tran.position; return SetPosition(tran, x, y, p.z, local); }
    public static Transform SetXZ(this Transform tran, float x, float z, bool local = false) { Vector3 p = local ? tran.localPosition : tran.position; return SetPosition(tran, x, p.y, z, local); }
    public static Transform SetYZ(this Transform tran, float y, float z, bool local = false) { Vector3 p = local ? tran.localPosition : tran.position; return SetPosition(tran, p.x, y, z, local); }

    public static Transform SetScale(this Transform tran, float scale = 1)
    {
        tran.localScale = new Vector3(scale, scale, scale);
        return tran;
    }
}
