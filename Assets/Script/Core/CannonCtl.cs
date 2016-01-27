using System.Collections;
using UnityEngine;
/// <summary>
/// 大炮控制器
/// </summary>
public class CannonCtl : MonoBehaviour
{
    public static CannonCtl Instance;

    #region 系统组件
    Transform mTran;
    Transform Muzzle;//炮口
    Vector2 Position;//大炮位置
    RayPath Path;//投射路径
    #endregion

    #region 通用
    Vector2 FireDirect;//开炮方向
    int FireSpeed = 110;//开炮速度
    bool Rotating;//是否在旋转

    Transform NowBubble;//当前泡泡
    Transform NextBubble;//备用泡泡
    bool isSwap;//是否切换中
    #endregion

    #region 常量
    Vector2 NowPos = new Vector2(0, -28.6f);//当前泡泡位置
    Vector2 NextPos = new Vector2(8, -32);//备用泡泡位置
    Vector2 One = new Vector2(-1, 0);//夹角标量
    const int AngleMin = 20;//炮口最小角度
    const int AngleMax = 160;//炮口最大角度
    const float Radius = 1.8f;//默认碰撞范围
    AnimationCurve scaleAnim = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(.3f, 1) });//缩放动画

    float SwapSpeed = 40;//切换速度
    float SwapDis = .1f;//切换距离
    #endregion

    void Start()
    {
        Instance = this;
        mTran = transform;
        Muzzle = mTran.Find("Muzzle");
        Position = Camera.main.WorldToScreenPoint(mTran.position);
        Path = new RayPath();
    }

    void Update()
    {
        //旋转炮口
        RotateMuzzle();
        //投射路径
        RaycastPath();
    }

    #region 控制
    //初始化炮弹
    public IEnumerator InitShell()
    {
        //生成新泡泡
        NextBubble = GetBubble();
        NextBubble.position = NextPos;

        float scale = 0, time = 0;
        while (scale < 1)
        {
            time += Time.deltaTime;
            scale = scaleAnim.Evaluate(time);
            NextBubble.localScale = new Vector2(scale, scale);
            yield return 0;
        }
        StartCoroutine(FillBubble());
    }

    //控制炮口
    public void ControlMuzzle(bool isControl)
    {
        if (!Rotating && isControl)
        {
            Rotating = true;
            Path.SetVisible(true);
        }
        if (Rotating && !isControl)
        {
            Rotating = false;
            Path.SetVisible(false);
            if (NowBubble != null && !isSwap)
            {
                FireDirect = Muzzle.right;
                Fire(NowBubble);
                NowBubble = null;
            }
        }
    }

    //填充泡泡
    public void FillAction() { StartCoroutine(FillBubble()); }

    //泡泡反弹
    public void InverseDirect(Transform shell)
    {
        FireDirect = new Vector2(-FireDirect.x, FireDirect.y);
        Fire(shell);
    }

    //切换泡泡
    public void SwapBubble()
    {
        if (NowBubble != null && !isSwap)
        {
            StartCoroutine(SwapAction(NowBubble, NextPos));
            StartCoroutine(SwapAction(NextBubble, NowPos));

            Transform temp = NowBubble;
            NowBubble = NextBubble;
            NextBubble = temp;
        }
    }
    #endregion

    #region 行为
    //旋转炮口
    void RotateMuzzle()
    {
        if (Rotating && !isSwap)
        {
            Vector2 pos = Input.mousePosition;
            pos -= Position;
            if (pos.y >= 0)
            {
                float angle = Vector2.Angle(One, pos);
                if (angle >= AngleMin && angle <= AngleMax)
                    Muzzle.localRotation = Quaternion.Euler(0, -180, angle);
            }
        }
    }

    //投射路径
    void RaycastPath()
    {
        if (Rotating && Time.frameCount % 2 == 0)
        {
            Vector2 dir = Muzzle.right;
            Vector2 start = (Vector2)Muzzle.position + (dir * 8);
            RaycastHit2D hit = Physics2D.Raycast(start, dir);

            if (hit.collider.CompareTag(Tag.Wall))
            {
                Vector2 dir2 = new Vector2(-dir.x, dir.y);
                Vector2 end = hit.point + dir2 * 50;
                Path.SetPath(start, hit.point, end);
            }
            else
                Path.SetPath(start, hit.point);
            Path.ShowPath();
        }
    }

    //开炮
    void Fire(Transform shell)
    {
        shell.parent = null;
        shell.GetComponent<Collider2D>().enabled = true;
        Rigidbody2D body = shell.GetComponent<Rigidbody2D>();
        body.isKinematic = false;
        //泡泡发射，并限制速度
        body.AddForce(FireDirect * FireSpeed, ForceMode2D.Impulse);
        if (body.velocity.magnitude > FireSpeed)
            body.velocity = Vector2.ClampMagnitude(body.velocity, FireSpeed);
    }

    //获取新泡泡
    Transform GetBubble(char id = '-')
    {
        GameObject obj = PoolManager.GetObject(PrefabKey.Bubble);
        obj.SetActive(true);
        //从优先级中获取
        if (id == '-') id = SlotTable.Instance.RandomBubbleID;//随机样式
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.InitComponent();
        bubble.InitBubble(id);
        bubble.InitShell(mTran);
        return obj.transform;
    }
    #endregion

    #region 动画
    //切换动画
    IEnumerator SwapAction(Transform tran, Vector2 targetPos)
    {
        isSwap = true;
        Vector2 center = new Vector2((tran.position.x + targetPos.x) / 2, -26);

        tran.GetComponent<SpriteRenderer>().sortingOrder = SpriteManager.Top;//图层置顶
        //移动到中转点
        while (true)
        {
            tran.position = Vector2.MoveTowards(tran.position, center, Time.deltaTime * SwapSpeed);
            float dis = Vector2.Distance(tran.position, center);
            if (dis <= SwapDis) break;
            yield return 0;
        }
        //移动到目标点
        while (true)
        {
            tran.position = Vector2.MoveTowards(tran.position, targetPos, Time.deltaTime * SwapSpeed);
            float dis = Vector2.Distance(tran.position, targetPos);
            if (dis <= SwapDis) break;
            yield return 0;
        }
        tran.position = targetPos;
        tran.GetComponent<SpriteRenderer>().sortingOrder = SpriteManager.Bubble;//图层恢复
        isSwap = false;
    }

    //填充炮弹
    IEnumerator FillBubble(char id = '-')
    {
        //炮弹填充
        if (NextBubble != null)
        {
            NowBubble = NextBubble;
            StartCoroutine(SwapAction(NowBubble, NowPos));
        }

        //生成新炮弹
        NextBubble = GetBubble(id);
        NextBubble.position = NextPos;

        float scale = 0, time = 0;
        while (scale < 1)
        {
            time += Time.deltaTime;
            scale = scaleAnim.Evaluate(time);
            NextBubble.localScale = new Vector3(scale, scale, 1);
            yield return 0;
        }
    }
    #endregion
}