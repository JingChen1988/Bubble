using UnityEngine;
//泡泡槽
public class Slot
{
    //对应泡泡标识，字符"-"代表空
    public char ID { get { return Bubble ? Bubble.ID : NULL; } }
    public byte Row;//行号，0开始
    public byte Column;//列号，0开始
    public Vector2 Location { get { return new Vector2(Row, Column); } }//位置
    public Vector2 Position;//物理坐标
    public Bubble Bubble;//泡泡对象

    #region 常量
    const char NULL = '-';//空标识
    const float INITY = -2.3f;//初始y
    const float INITX_EVEN = -20.7f;//初始x-偶数
    const float INITX_ODD = -18.4f;//初始x-奇数
    const float WIDTH = 4.6f;//宽度间隔
    const float HEIGHT = 4f;//高度间隔
    #endregion

    public Slot(Transform contain, byte r, byte c, char id = NULL)
    {
        Row = r;
        Column = c;
        //计算物理坐标
        float x = (Row % 2 == 0 ? INITX_EVEN : INITX_ODD) + Column * WIDTH;
        float y = INITY - Row * HEIGHT;
        Position = new Vector2(x, y);
        //创建泡泡
        if (id != NULL) InstanceBubble(contain, id);
    }

    //创建泡泡
    void InstanceBubble(Transform contain, char id)
    {
        GameObject obj = PoolManager.GetObject(PrefabKey.Bubble);
        obj.SetActive(true);
        //设置层级位置
        Transform BubbleTran = obj.transform;
        BubbleTran.parent = contain;
        BubbleTran.localPosition = Position;
        //初始化
        Bubble = BubbleTran.GetComponent<Bubble>();
        Bubble.InitComponent();
        Bubble.InitBubble(id);
        Bubble.Slot = this;
        Bubble.isAbsorb = true;
    }

    //连锁消除
    public void ToChain()
    {
        Bubble.mObj.SetActive(false);
        Bubble.Slot = null;
        Bubble = null;
    }

    //断开坠落
    public void ToFall()
    {
        Bubble.Collider.enabled = false;
        Bubble.Ridid.isKinematic = false;
        Bubble.Ridid.gravityScale = 1;
        PoolManager.TimingCollect(2f, Bubble.mObj);
        Bubble.Slot = null;
        Bubble = null;
    }
}
