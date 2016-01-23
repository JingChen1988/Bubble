using UnityEngine;
using System.Collections;
/// <summary>
/// 大炮控制器
/// </summary>
public class CannonControl : MonoBehaviour
{
    public static CannonControl Instance;

    Transform mTran;
    Transform Muzzle;//炮口
    Vector2 Position;//大炮位置

    bool Rotating;//是否在旋转
    Vector2 FireDirect;//开炮方向
    int FireSpeed = 100;//开炮速度

    Transform NowBubble;//当前泡泡
    Transform NextBubble;//备用泡泡

    RayPath Path;//投射路径

    Vector2 NowPos = new Vector2(3.4f, 0);//当前泡泡位置
    Vector2 NextPos = new Vector2(8, 0);//备用泡泡位置
    Vector2 One = new Vector2(-1, 0);//夹角标量
    const int AngleMin = 20;//炮口最小角度
    const int AngleMax = 160;//炮口最大角度
    const float Radius = 1.8f;//碰撞范围

    void Start()
    {
        Instance = this;
        mTran = transform;
        Muzzle = mTran.Find("Muzzle");
        Position = Camera.main.WorldToScreenPoint(mTran.position);

        SetNowBubble(GetBubble('B'));
        SetNextBubble(GetBubble('A'));

        Path = new RayPath();
    }

    void Update()
    {
        if (!Rotating && Input.GetMouseButtonDown(0))
        {
            Rotating = true;
            Path.SetVisible(true);
        }
        if (Rotating && Input.GetMouseButtonUp(0))
        {
            Rotating = false;
            Path.SetVisible(false);
            if (NowBubble != null)
            {
                FireDirect = NowBubble.up;
                Fire(NowBubble);
                NowBubble = null;
            }
        }
        //旋转炮口
        RotateMuzzle();
        //投射路径
        RaycastPath();
        //控制切换泡泡
        if (Input.GetKeyDown(KeyCode.W)) SwapBubble();
    }

    //投射路径
    void RaycastPath()
    {
        if (Rotating && Time.frameCount % 2 == 0)
        {
            Vector2 dir = Muzzle.right;
            Vector2 start = (Vector2)Muzzle.position + (dir * 10);
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

    //旋转炮口
    void RotateMuzzle()
    {
        if (Rotating)
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

    //开炮
    void Fire(Transform shell, bool isFill = true)
    {
        shell.parent = null;
        Rigidbody2D body = shell.GetComponent<Rigidbody2D>();
        body.isKinematic = false;
        //泡泡发射，并限制速度
        body.AddForce(FireDirect * FireSpeed, ForceMode2D.Impulse);
        if (body.velocity.magnitude > FireSpeed)
            body.velocity = Vector2.ClampMagnitude(body.velocity, FireSpeed);

        //填充泡泡
        if (isFill) StartCoroutine(FillBubble());
    }

    //泡泡反弹
    public void InverseDirect(Transform shell)
    {
        FireDirect = new Vector2(-FireDirect.x, FireDirect.y);
        Fire(shell, false);
    }

    //切换泡泡
    void SwapBubble()
    {
        Transform temp = NowBubble;
        SetNowBubble(NextBubble);
        SetNextBubble(temp);
    }

    //设置当前泡泡
    void SetNowBubble(Transform tran)
    {
        NowBubble = tran;
        NowBubble.parent = Muzzle;
        NowBubble.localPosition = NowPos;
        NowBubble.localRotation = Quaternion.Euler(0, -180, 90);
    }

    //设置备用泡泡
    void SetNextBubble(Transform tran)
    {
        NextBubble = tran;
        NextBubble.parent = mTran;
        NextBubble.localPosition = NextPos;
        NextBubble.localRotation = Quaternion.identity;
    }

    //获取新泡泡
    Transform GetBubble(char id = '-')
    {
        GameObject obj = PoolManager.GetObject(PrefabKey.Bubble);
        obj.SetActive(true);
        if (id == '-') id = (char)Random.Range('A', 'H');//随机样式
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.InitComponent();
        bubble.InitBubble(id);
        bubble.Collider.radius = Radius;
        return obj.transform;
    }

    //填充泡泡
    IEnumerator FillBubble()
    {
        yield return new WaitForSeconds(1);
        SetNowBubble(NextBubble);
        SetNextBubble(GetBubble());
    }
}