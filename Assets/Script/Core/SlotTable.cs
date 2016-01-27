using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 泡泡槽列表
/// </summary>
public class SlotTable : MonoBehaviour
{
    public static SlotTable Instance;

    Slot[][] Tables;//列表
    Transform SlotContain;//容器
    List<char> Repertory = new List<char>();//泡泡仓库
    public char RandomBubbleID { get { return Repertory[Random.Range(0, Repertory.Count)]; } }

    #region 常量
    Direct[] Directs = { Direct.Left, Direct.Right, Direct.LeftDown, Direct.RightDown, Direct.LeftUp, Direct.RightUp };//方向集合
    const int ChainCount = 3;//连锁达到数量
    const float ChainDelay = .05f;//连锁延迟
    const int SpreadRange = 10;//震动传播范围
    AnimationCurve scaleAnim = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(.6f, 1) });//缩放动画
    const float ShowScale = .6f;//展示缩放
    #endregion

    #region 初始化
    void Start()
    {
        Instance = this;
        SlotContain = transform;
        LoadTable();
        LoadRepertory();
    }

    //加载关卡表
    void LoadTable()
    {
        TextAsset binAsset = Resources.Load<TextAsset>("Data/Level01");
        string[] lineArray = binAsset.text.Split('\n');
        //读取行数设置
        int RowCount = int.Parse(lineArray[0]);
        Tables = new Slot[RowCount][];
        //初始化列表
        for (byte i = 0; i < RowCount; i++)
        {
            //偶数行=10，奇数行=9
            int columnCount = (i % 2 == 0 ? 10 : 9);
            Tables[i] = new Slot[columnCount];
            //读取配置信息，不足空槽补齐
            if (i + 1 < lineArray.Length)
            {
                string columnArray = lineArray[i + 1].Trim();
                for (byte j = 0; j < columnCount; j++)
                    Tables[i][j] = new Slot(SlotContain, i, j, columnArray[j]);
            }
            else
            {
                for (byte j = 0; j < columnCount; j++)
                    Tables[i][j] = new Slot(SlotContain, i, j);
            }
        }
    }

    //加载仓库
    void LoadRepertory()
    {
        List<char> rep = new List<char>();
        for (int i = 0, len = Tables.Length; i < len; i++)
        {
            Slot[] solts = Tables[i];
            for (int j = 0, len2 = solts.Length; j < len2; j++)
            {
                Slot slot = solts[j];
                if (slot.Bubble != null)
                {
                    char id = slot.Bubble.ID;
                    if (id != Slot.NULL && !rep.Contains(id)) rep.Add(id);
                }
            }
        }
        Repertory = rep;
    }
    #endregion

    #region 行为逻辑
    //泡泡展示动画(递归)
    public IEnumerator ShowTable(int index = 0)
    {
        if (index < Tables.Length)
        {
            Slot[] solts = Tables[index];
            float scale = 0, time = 0;
            bool existBubble = false;//该行是否存在泡泡
            bool nextBubble = false;//下行泡泡缩放
            while (scale < 1)
            {
                time += Time.deltaTime;
                scale = scaleAnim.Evaluate(time);
                //批量缩放
                for (int i = 0, len = solts.Length; i < len; i++)
                {
                    Slot slot = solts[i];
                    if (slot.Bubble != null)
                    {
                        slot.Bubble.mTran.localScale = new Vector3(scale, scale, 1);
                        existBubble = true;
                    }
                }
                //不存在泡泡则退出缩放线程
                if (!existBubble)
                {
                    GSM.Instance.ShowTableFinish();
                    break;
                }
                //递归启动下行泡泡缩放
                if (!nextBubble && scale > ShowScale)
                {
                    nextBubble = true;
                    StartCoroutine(ShowTable(index + 1));
                }
                yield return 0;
            }
        }
    }

    //泡泡吸附顶部
    public void AbsorbTop(Bubble bubble)
    {
        Vector2 pos = SlotContain.InverseTransformPoint(bubble.mTran.position);
        float DistanceMin = 1000;
        Slot targetSlot = null;

        //搜索顶部泡泡
        Slot[] tops = Tables[0];
        for (int i = 0, len = tops.Length; i < len; i++)
        {
            Slot slot = tops[i];
            if (slot.Bubble == null)
            {
                float dis = Vector2.Distance(pos, slot.Position);
                if (dis < DistanceMin)
                {
                    DistanceMin = dis;
                    targetSlot = slot;
                }
            }
        }

        //泡泡嵌入槽内
        bubble.mTran.parent = SlotContain;
        bubble.AbsorbAction(targetSlot);
        //震动传播
        SpreadShake(bubble, targetSlot);
    }

    //泡泡吸附泡泡
    public void Absorb(Bubble bubble, Slot slot)
    {
        Vector2 pos = SlotContain.InverseTransformPoint(bubble.mTran.position);
        float DistanceMin = 1000;
        Vector2 location = slot.Location;
        bool even = slot.Row % 2 == 0;
        Slot targetSlot = null;

        //搜索邻近槽位
        for (int i = 0, len = Directs.Length; i < len; i++)
        {
            Direct dir = Directs[i];
            Slot temp = FindSideBubble(location, dir, even);
            if (temp != null && temp.Bubble == null)
            {
                float dis = Vector2.Distance(pos, temp.Position);
                //距离最短
                if (dis < DistanceMin)
                {
                    DistanceMin = dis;
                    targetSlot = temp;
                }
            }
        }

        //泡泡嵌入槽内
        bubble.mTran.parent = SlotContain;
        bubble.AbsorbAction(targetSlot);
        //震动传播
        SpreadShake(bubble, targetSlot);
    }

    //震动传播
    void SpreadShake(Bubble bubble, Slot slot)
    {
        Vector2 point = bubble.mTran.localPosition;
        Vector2 location = slot.Location;
        bool even = slot.Row % 2 == 0;

        for (int i = 0, len = Directs.Length; i < len; i++)
        {
            Direct dir = Directs[i];
            Slot temp = FindSideBubble(location, dir, even);
            if (temp != null && temp.Bubble != null && !temp.Bubble.isShake)
            {
                float dis = Vector2.Distance(point, temp.Position);
                //距离限制
                if (dis < SpreadRange)
                {
                    temp.Bubble.ShakeAction(bubble);
                    SpreadShake(bubble, temp);
                }
            }
        }
    }

    //连锁泡泡
    public void ChainBubble(Slot slot)
    {
        List<Slot> slots = new List<Slot>();
        SearchChain(slot, slots);
        StartCoroutine(ChainExplode(slot, slots));
    }

    //连锁搜索(递归)
    void SearchChain(Slot center, List<Slot> slots)
    {
        slots.Add(center);//添加中心槽
        Vector2 location = center.Location;
        bool even = center.Row % 2 == 0;
        //以中心为基点，向六个方向深度搜索相同的泡泡
        for (int i = 0, len = Directs.Length; i < len; i++)
        {
            Direct dir = Directs[i];
            Slot temp = FindSideBubble(location, dir, even);
            if (temp != null && temp.Bubble != null && temp.ID == center.ID && !slots.Contains(temp))
                SearchChain(temp, slots);
        }
    }

    //连锁爆炸
    IEnumerator ChainExplode(Slot center, List<Slot> slots)
    {
        if (slots.Count >= ChainCount)
        {
            //按距离排序
            List<SlotDistance> diss = new List<SlotDistance>();
            for (int i = 0, len = slots.Count; i < len; i++)
            {
                Slot tempSlot = slots[i];
                diss.Add(new SlotDistance(tempSlot, Vector2.Distance(tempSlot.Position, center.Position)));
            }
            System.Comparison<SlotDistance> comp = (a, b) => { return a.distance > b.distance ? 1 : -1; };
            diss.Sort(comp);

            //连锁爆炸（由近到远）
            for (int i = 0, len = diss.Count; i < len; i++)
            {
                Slot tempSlot = diss[i].slot;
                tempSlot.ToChain();
                yield return new WaitForSeconds(ChainDelay);
            }
            FallDown();//泡泡坠落
            LoadRepertory();//重置仓库
        }

        if (Repertory.Count > 0) CannonCtl.Instance.FillAction();
        else Application.LoadLevel(0);
    }

    //坠落
    void FallDown()
    {
        //获取顶部泡泡
        List<Slot> topBubbles = FindTopBubbles();

        //搜索所有连接顶部的泡泡
        List<Slot> slots = new List<Slot>();
        slots.AddRange(topBubbles);
        for (int i = 0, len = topBubbles.Count; i < len; i++)
        {
            Slot slot = topBubbles[i];
            //搜索-左下边
            Direct dir = Direct.LeftDown;
            Slot temp = FindSideBubble(slot.Location, dir, true);
            if (temp != null && temp.Bubble != null && !slots.Contains(temp)) FindLinkTop(temp, slots);
            //搜索-右下边
            dir = Direct.RightDown;
            temp = FindSideBubble(slot.Location, dir, true);
            if (temp != null && temp.Bubble != null && !slots.Contains(temp)) FindLinkTop(temp, slots);
        }

        //遍历所有泡泡，剔除连接顶部的
        List<Slot> fallBubbles = new List<Slot>();
        for (int i = 0, len = Tables.Length; i < len; i++)
        {
            Slot[] solts = Tables[i];
            for (int j = 0, len2 = solts.Length; j < len2; j++)
            {
                Slot slot = solts[j];
                if (slot.Bubble != null && !slots.Contains(slot))
                    fallBubbles.Add(slot);
            }
        }

        //坠落失去连接的泡泡
        for (int i = 0, len = fallBubbles.Count; i < len; i++)
            fallBubbles[i].ToFall();
    }

    //搜索连接顶部的泡泡(递归)
    void FindLinkTop(Slot center, List<Slot> slots)
    {
        slots.Add(center);//添加中心槽
        Vector2 location = center.Location;
        bool even = center.Row % 2 == 0;
        //以中心为基点，向六个方向深度搜索相同的泡泡
        for (int i = 0, len = Directs.Length; i < len; i++)
        {
            Direct dir = Directs[i];
            Slot temp = FindSideBubble(location, dir, even);
            if (temp != null && temp.Bubble != null && !slots.Contains(temp))
                FindLinkTop(temp, slots);
        }
    }
    #endregion

    #region 通用函数
    //搜索邻近的泡泡（x=行号，y=列号）
    Slot FindSideBubble(Vector2 seat, Direct dir, bool even)
    {
        Vector2 nextSeat = Vector2.zero;
        if (even)
        {//偶数行
            switch (dir)
            {
                case Direct.Left: nextSeat = new Vector2(seat.x, seat.y - 1); break;
                case Direct.Right: nextSeat = new Vector2(seat.x, seat.y + 1); break;
                case Direct.LeftUp: nextSeat = new Vector2(seat.x - 1, seat.y - 1); break;
                case Direct.LeftDown: nextSeat = new Vector2(seat.x + 1, seat.y - 1); break;
                case Direct.RightUp: nextSeat = new Vector2(seat.x - 1, seat.y); break;
                case Direct.RightDown: nextSeat = new Vector2(seat.x + 1, seat.y); break;
            }
        }
        else
        {//奇数行
            switch (dir)
            {
                case Direct.Left: nextSeat = new Vector2(seat.x, seat.y - 1); break;
                case Direct.Right: nextSeat = new Vector2(seat.x, seat.y + 1); break;
                case Direct.LeftUp: nextSeat = new Vector2(seat.x - 1, seat.y); break;
                case Direct.LeftDown: nextSeat = new Vector2(seat.x + 1, seat.y); break;
                case Direct.RightUp: nextSeat = new Vector2(seat.x - 1, seat.y + 1); break;
                case Direct.RightDown: nextSeat = new Vector2(seat.x + 1, seat.y + 1); break;
            }
        }
        return FindBubble(nextSeat);
    }

    //获取对应位置的槽
    Slot FindBubble(Vector2 seat)
    {
        Slot bubble = null;
        if (seat.x >= 0 && seat.x < Tables.Length)
        {
            Slot[] solts = Tables[(int)seat.x];
            if (seat.y >= 0 && seat.y < solts.Length)
                bubble = solts[(int)seat.y];
        }
        return bubble;
    }

    //获取位于顶部的泡泡
    List<Slot> FindTopBubbles()
    {
        Slot[] tops = Tables[0];
        List<Slot> topBubbles = new List<Slot>();
        for (int i = 0, len = tops.Length; i < len; i++)
        {
            Slot slot = tops[i];
            if (slot.Bubble != null) topBubbles.Add(slot);
        }
        return topBubbles;
    }
    #endregion
}

//槽-距离对
class SlotDistance
{
    public Slot slot;
    public float distance;
    public SlotDistance(Slot sl, float dis)
    {
        slot = sl;
        distance = dis;
    }
}