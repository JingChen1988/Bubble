using UnityEngine;
/// <summary>
/// 子项改变事件
/// </summary>
public delegate void OnItemChange(GameObject sender, int index);
/// <summary>
/// 滑动缩放组件
/// </summary>
[RequireComponent(typeof(UICenterOnChild))]
public class ScrollScale : MonoBehaviour
{
    public OnItemChange OnEndItemChange;//子项改变事件
    public OnItemChange OnStartItemChange;//子项改变事件

    Transform panelTrans;//滑动组件
    Vector3 panelCenter;//中心位置
    GameObject centerObj;//中心对象

    Transform[] Items;//子项集合
    UIWidget[][] SpriteList;//精灵集合
    Transform FirstItem;//第一个子项

    int ItemCount;//子项数量
    int SpriteCount;//子项精灵数量
    public int ItemIndex;//选中索引
    float Distance;//距离记录
    SpriteScale[] Origin;//原始缩放参数
    float Factor;//缩放因子
    UICenterOnClick[] ItemsClick;//子项定位组件
    public bool IsScale = true;//是否缩放

    const float MaxFactor = 1;//最大缩放
    const float MinFactor = .6f;//最小缩放
    const int MaxDis = 464;//最大缩放距离
    const float WidthFactor = .2f;//宽间距因子
    const float HeightFactor = .5f;//高间距因子
    const int Rate = 1;//缩放频率

    public void Init()
    {
        Transform mTran = transform;
        UIScrollView mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);
        panelTrans = mScrollView.panel.cachedTransform;
        Factor = MaxFactor - MinFactor;

        //获取中心点
        Vector3[] corners = mScrollView.panel.worldCorners;
        panelCenter = (corners[2] + corners[0]) * 0.5f;

        //初始化子项集合
        ItemCount = mTran.childCount;
        Items = new Transform[ItemCount];
        SpriteList = new UIWidget[ItemCount][];
        ItemsClick = new UICenterOnClick[ItemCount];

        for (int i = 0; i < ItemCount; i++)
        {
            Transform tr = mTran.GetChild(i);
            Items[i] = tr;
            ItemsClick[i] = tr.GetComponent<UICenterOnClick>();
            SpriteCount = tr.childCount;
            SpriteList[i] = new UIWidget[SpriteCount];
            for (int j = 0; j < SpriteCount; j++)
                SpriteList[i][j] = tr.GetChild(j).GetComponent<UIWidget>();
        }
        FirstItem = Items[0];

        //获取宽度和高度偏移
        Origin = new SpriteScale[SpriteCount];
        for (int i = 0; i < SpriteCount; i++)
        {
            UIWidget sprite = SpriteList[0][i];
            if (sprite == null) continue;
            float w = sprite.width;
            float h = sprite.height;
            int t = sprite.topAnchor.absolute;
            int b = sprite.bottomAnchor.absolute;
            int l = sprite.leftAnchor.absolute;
            int r = sprite.rightAnchor.absolute;
            Origin[i] = new SpriteScale(w, h, t, b, l, r);
        }

        //开启自动居中功能
        UICenterOnChild UICenter = GetComponent<UICenterOnChild>();
        UICenter.enabled = false;
        UICenter.enabled = true;

        //设置回调
        UICenter.onCenter = onCenter;
        UICenter.onFinished = onFinished;

        //缩放
        if (IsScale) Scale();
    }

    void Update()
    {
        //根据距离，改变所有子项的缩放
        if (IsScale && IsMove()) Scale();
    }

    //缩放子项
    void Scale()
    {
        for (int i = 0; i < ItemCount; i++)
        {
            Transform target = Items[i];

            //计算距离和缩放因子
            Vector3 cp = panelTrans.InverseTransformPoint(target.position);
            Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
            Vector3 localOffset = cp - cc;
            float distance = Mathf.Abs(localOffset.x);
            float scale = distance >= MaxDis ? MinFactor : (MaxDis - distance) / MaxDis * Factor + MinFactor;

            //缩放子项
            for (int j = 0; j < SpriteCount; j++)
            {
                UIWidget sprite = SpriteList[i][j];
                SpriteScale origin = Origin[j];

                float WidthOffset = (origin.Width - origin.Width * scale) * WidthFactor;
                float HeightOffset = (origin.Height - origin.Height * scale) * HeightFactor;

                float Left = origin.Left + WidthOffset;
                float Right = origin.Right - WidthOffset;
                float Top = origin.Top - HeightOffset;
                float Bottom = origin.Bottom + HeightOffset;

                sprite.rightAnchor.absolute = (int)Right;
                sprite.leftAnchor.absolute = (int)Left;
                sprite.topAnchor.absolute = (int)Top;
                sprite.bottomAnchor.absolute = (int)Bottom;
            }
        }
    }

    //是否移动滑动组件
    bool IsMove()
    {
        Vector3 cp = panelTrans.InverseTransformPoint(FirstItem.position);
        Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
        Vector3 localOffset = cp - cc;
        float distance = Mathf.Abs(localOffset.x);

        //位置改变
        bool isMove = false;
        float offset = Mathf.Abs(Distance - distance);
        if (offset > 0.01f)
        {
            isMove = true;
            Distance = distance;
        }
        return isMove;
    }

    //定位回调
    void onCenter(GameObject obj)
    {
        if (obj != centerObj)
        {
            centerObj = obj;
            for (int i = 0; i < ItemCount; i++)
                if (Items[i].gameObject == centerObj)
                {
                    ItemIndex = i;
                    if (OnStartItemChange != null) OnStartItemChange(ItemsClick[ItemIndex].gameObject, ItemIndex);
                    break;
                }
        }
    }

    //完成回调
    void onFinished()
    {
        if (OnEndItemChange != null) OnEndItemChange(centerObj, ItemIndex);
    }

    //选择子项
    public void ChoiseItem(int index)
    {
        if (index < 0 || index >= ItemCount) return;
        ItemsClick[index].OnClick();
        ItemIndex = index;
    }

    //选择子项，递增
    public void ChoiseItem(bool right)
    {
        int index = ItemIndex;
        if (right) index++;
        else index--;
        ChoiseItem(index);
    }

    //精灵缩放
    public struct SpriteScale
    {
        public float Width;
        public float Height;
        public int Top;
        public int Bottom;
        public int Left;
        public int Right;

        public SpriteScale(float w, float h, int t, int b, int l, int r)
        {
            Width = w;
            Height = h;
            Top = t;
            Bottom = b;
            Left = l;
            Right = r;
        }
    }
}
