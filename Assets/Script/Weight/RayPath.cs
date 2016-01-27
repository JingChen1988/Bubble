using UnityEngine;
/// <summary>
/// 射线路径
/// </summary>
public class RayPath
{
    public Vector2 Start;//起点
    public Vector2 Center;//中转
    public Vector2 End;//终点

    GameObject[] Points;//点集合
    float[] OffsetPos;//点位置
    float OffsetLen;//偏移长度
    const int PointCount = 10;//点数量
    const int PointDis = 3;//点间距
    const int MoveSpeed = 10;//移动速度
    const float Scale = .2f;//点缩放尺寸
    const float Size = .1f;//点尺寸

    public RayPath()
    {
        //生成圆点
        Points = new GameObject[PointCount];
        for (int i = 0; i < PointCount; i++)
        {
            Points[i] = GameObject.Instantiate<GameObject>(PrefabList.Circle);
            Points[i].SetActive(false);
        }
        //初始化位置
        OffsetPos = new float[PointCount];
        for (int i = 0; i < PointCount; i++)
            OffsetPos[i] = i * PointDis;
        OffsetLen = PointDis * PointCount;
    }

    //设置节点
    public void SetPath(Vector2 start, Vector2 center) { SetPath(start, center, Vector2.zero); }
    public void SetPath(Vector2 start, Vector2 center, Vector2 end)
    {
        Start = start;
        Center = center;
        End = end;
    }

    //开关路径
    public void SetVisible(bool isVisible)
    {
        for (int i = 0; i < PointCount; i++)
            Points[i].SetActive(isVisible);
    }

    //显示路径
    public void ShowPath()
    {
        //计算所有点的偏移量
        float move = Time.deltaTime * MoveSpeed;
        for (int i = 0; i < PointCount; i++)
        {
            float pos = OffsetPos[i] + move;
            if (pos > OffsetLen) pos -= OffsetLen;
            float scale = Scale * (1 - pos / OffsetLen) + Size;
            Points[i].transform.localScale = new Vector2(scale, scale);
            OffsetPos[i] = pos;
        }

        //显示第一条射线上的点
        float Distance = Vector2.Distance(Start, Center);
        Vector2 dir = (Center - Start).normalized;
        for (int i = 0; i < PointCount; i++)
        {
            float dis = OffsetPos[i];
            if (dis < Distance)
                Points[i].transform.position = Start + dir * dis;
        }

        //显示第二条射线上的点
        if (End != Vector2.zero)
        {
            Vector2 dir2 = (End - Center).normalized;
            for (int i = 0; i < PointCount; i++)
            {
                float dis = OffsetPos[i];
                if (dis > Distance)
                    Points[i].transform.position = Center + dir2 * (dis - Distance);
            }
        }
    }
}

