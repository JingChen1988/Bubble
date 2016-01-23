using System.Collections.Generic;
using System.Text;
using UnityEngine;

//泡泡槽列表
public class SlotTable
{
    public static SlotTable Instance;

    Slot[][] Tables;//列表
    Transform SlotContain;//容器

    #region 常量
    Direct[] Directs = { Direct.Left, Direct.Right, Direct.LeftDown, Direct.RightDown, Direct.LeftUp, Direct.RightUp };//方向集合
    const int ChainCount = 3;//连锁最低数量
    #endregion

    public SlotTable()
    {
        Instance = this;
        SlotContain = GameObject.Find("SlotTable").transform;
        LoadTable();
    }

    void LoadTable()
    {
        //加载关卡配置
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

    //打印列表
    public void PrintTable()
    {
        StringBuilder text = new StringBuilder();
        for (int i = 0, len = Tables.Length; i < len; i++)
        {
            Slot[] solts = Tables[i];
            for (int j = 0, len2 = solts.Length; j < len2; j++)
            {
                Slot slot = solts[j];
                text.AppendLine(slot.Position.ToString());
            }
            text.AppendLine("\n");
        }
        Debug.Log(text);
    }

    //泡泡吸附顶部
    public void AbsorbTop(Bubble bubble)
    {
        Vector2 pos = SlotContain.InverseTransformPoint(bubble.mTran.position);
        float DistanceMin = 1000;
        Slot targetSlot = null;
        Slot[] tops = Tables[0];
        //添加顶部泡泡
        List<Slot> topBubbles = new List<Slot>();
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
        if (targetSlot != null)
        {
            bubble.mTran.parent = SlotContain;
            bubble.AbsorbAction(targetSlot);
        }
    }

    //泡泡吸附
    public void Absorb(Slot slot, Bubble bubble)
    {
        Vector2 pos = SlotContain.InverseTransformPoint(bubble.mTran.position);
        float DistanceMin = 1000;
        Vector2 location = slot.Location;
        bool even = slot.Row % 2 == 0;
        Slot targetSlot = null;

        for (int i = 0, len = Directs.Length; i < len; i++)
        {
            Direct dir = Directs[i];
            Slot temp = FindSideBubble(location, dir, even);
            if (temp != null)
            {
                float dis = Vector2.Distance(pos, temp.Position);
                //空槽&距离最短
                if (temp.Bubble == null && dis < DistanceMin)
                {
                    DistanceMin = dis;
                    targetSlot = temp;
                }
            }
        }

        //泡泡嵌入槽内
        if (targetSlot != null)
        {
            bubble.mTran.parent = SlotContain;
            bubble.AbsorbAction(targetSlot);
        }
    }

    //震动效果
    void Shake()
    {

    }

    //搜索附近的泡泡（x=行号，y=列号）
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

    //连锁泡泡
    public void ChainBubble(Slot slot)
    {
        List<Slot> slots = new List<Slot>();
        Chain(slot, slots);
        //连锁爆炸
        if (slots.Count >= ChainCount)
        {
            for (int i = 0, len = slots.Count; i < len; i++)
            {
                Slot tempSlot = slots[i];
                tempSlot.ToChain();
            }
            //坠落
            FallDown();
        }
    }

    //连锁递归
    void Chain(Slot center, List<Slot> slots)
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
                Chain(temp, slots);
        }
    }

    void FallDown()
    {
        Slot[] tops = Tables[0];

        //添加顶部泡泡
        List<Slot> topBubbles = new List<Slot>();
        for (int i = 0, len = tops.Length; i < len; i++)
        {
            Slot slot = tops[i];
            if (slot.Bubble != null) topBubbles.Add(slot);
        }

        //搜索所有连接顶部的泡泡
        List<Slot> slots = new List<Slot>();
        slots.AddRange(topBubbles);
        for (int i = 0, len = topBubbles.Count; i < len; i++)
        {
            Slot slot = topBubbles[i];
            //向左下边搜索
            Direct dir = Direct.LeftDown;
            Slot temp = FindSideBubble(slot.Location, dir, true);
            if (temp != null && temp.Bubble != null && !slots.Contains(temp)) FindTopBubble(temp, slots);
            //向右下边搜索
            dir = Direct.RightDown;
            temp = FindSideBubble(slot.Location, dir, true);
            if (temp != null && temp.Bubble != null && !slots.Contains(temp)) FindTopBubble(temp, slots);
        }

        //遍历所有泡泡，排除连接顶部的
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
        for (int i = 0, len = fallBubbles.Count; i < len; i++) fallBubbles[i].ToFall();
    }

    void FindTopBubble(Slot center, List<Slot> slots)
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
                FindTopBubble(temp, slots);
        }
    }
}
