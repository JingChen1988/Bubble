using System.Collections;
using UnityEngine;
/// <summary>
/// 泡泡行为组件
/// </summary>
public class Bubble : MonoBehaviour
{
    #region 系统组件
    public Transform mTran;
    public GameObject mObj;
    public Rigidbody2D Ridid;
    public CircleCollider2D Collider;
    public SpriteRenderer Sprite;
    #endregion

    #region 通用
    public char ID;//对应泡泡标识，字符"-"代表空
    public Slot Slot;//泡泡所在槽位
    public bool isAbsorb;//是否吸附
    public bool isShake;//是否震动
    #endregion

    #region 常量
    const int AbsorbSpeed = 25;//吸附速度
    const float AbsorbMinDis = .2f;//吸附最小距离
    const float Radius = 2.3f;//碰撞范围

    const int ScopeBase = 4;//幅度基数
    const int ShakeMax = 5;//最大值
    const int ShakeSpeed = 12;//速度
    const float ShakeMinDis = .1f;//最小距离

    const float FALL_X = 6;//坠落X取值
    const float FALL_Y = 8;//坠落Y取值
    const int FallGravity = 5;//坠落重力
    #endregion

    #region 初始化
    public void InitComponent()
    {
        if (mObj) return;
        mTran = transform;
        mObj = gameObject;
        Ridid = mTran.GetComponent<Rigidbody2D>();
        Collider = mTran.GetComponent<CircleCollider2D>();
        Sprite = mTran.GetComponent<SpriteRenderer>();
    }

    public void InitBubble(char id)
    {
        //更新图片
        if (id != ID)
        {
            ID = id;
            Sprite.sprite = SpriteManager.GetInstance().GetSprite(ID.ToString());
        }

        //重置属性
        isAbsorb = false;
        Ridid.gravityScale = 0;
        Collider.enabled = true;
        mTran.rotation = Quaternion.identity;
    }

    public void InitShell(Transform cannon)
    {
        mObj.layer = Layer.Bubble;
        Collider.radius = Radius;
        Collider.enabled = false;
        Sprite.sortingOrder = SpriteManager.Bubble;
        Ridid.fixedAngle = true;
        mTran.parent = cannon;
        mTran.localScale = Vector2.zero;
    }
    #endregion

    #region 碰撞处理
    //泡泡碰撞处理
    void OnCollisionEnter2D(Collision2D coll)
    {
        if (isAbsorb) return;

        Transform other = coll.transform;
        if (other.CompareTag(Tag.Wall))
        {//1.墙壁反弹
            CannonCtl.Instance.InverseDirect(mTran);
        }
        else if (other.CompareTag(Tag.Top))
        {//2.吸附顶部
            isAbsorb = true;
            SlotTable.Instance.AbsorbTop(this);
        }
        else if (other.CompareTag(Tag.Bubble))
        {//3.吸附泡泡
            isAbsorb = true;
            SlotTable.Instance.Absorb(this, other.GetComponent<Bubble>().Slot);
        }
    }

    //地面碰撞处理
    void OnCollisionStay2D(Collision2D coll)
    {
        Transform other = coll.transform;
        if (other.CompareTag(Tag.Ground) && Ridid.velocity.y < 1)
        {//碰撞地面消失
            mObj.SetActive(false);
        }
    }
    #endregion

    #region 动画
    //泡泡吸附动画
    public void AbsorbAction(Slot slot) { StartCoroutine(AbsorbSlot(slot)); }
    IEnumerator AbsorbSlot(Slot slot)
    {   //吸附到指定槽位
        Slot = slot;
        Slot.Bubble = this;
        Ridid.isKinematic = true;

        //吸附动画
        Vector2 to = Slot.Position;
        while (true)
        {
            mTran.localPosition = Vector2.Lerp(mTran.localPosition, to, Time.deltaTime * AbsorbSpeed);
            if (Vector2.Distance(mTran.localPosition, to) <= AbsorbMinDis) break;
            yield return 0;
        }
        mTran.localPosition = to;
        Collider.radius = Radius;

        //进行连锁反应
        SlotTable.Instance.ChainBubble(Slot);
    }

    //泡泡震动动画
    public void ShakeAction(Bubble bubble) { StartCoroutine(Shake(bubble)); }
    IEnumerator Shake(Bubble bubble)
    {
        isShake = true;
        Vector2 dir = mTran.position - bubble.mTran.position;//计算方向
        float scope = ScopeBase / dir.magnitude;//计算幅度，距离越远，幅度越小
        if (scope > ShakeMax) scope = ShakeMax;//限制幅度
        Vector2 from = Slot.Position;//起始位置
        Vector2 to = from + (dir.normalized * scope);//震动位置

        //震动
        while (true)
        {
            if (!isShake) yield break;//停止动画
            mTran.localPosition = Vector2.Lerp(mTran.localPosition, to, Time.deltaTime * ShakeSpeed);
            float dis = Vector2.Distance(mTran.localPosition, to);
            if (dis <= ShakeMinDis) break;
            yield return 0;
        }

        //归位
        while (true)
        {
            if (!isShake) yield break;//停止动画
            mTran.localPosition = Vector2.Lerp(mTran.localPosition, from, Time.deltaTime * ShakeSpeed);
            float dis = Vector2.Distance(mTran.localPosition, from);
            if (dis <= ShakeMinDis) break;
            yield return 0;
        }
        mTran.localPosition = from;

        //状态回复
        yield return 0;
        isShake = false;
    }
    #endregion

    #region 行为
    //连锁消除
    public void ToChain()
    {
        //爆炸动画
        GameObject explode = PoolManager.GetObject(PrefabKey.Explode);
        explode.SetActive(true);
        explode.transform.position = mTran.position;
        explode.GetComponent<Animator>().Play("ExplodeAnim");
        PoolManager.TimingCollect(1, explode);

        //消除对象
        isShake = false;//停止震动
        mObj.SetActive(false);
        Slot = null;
    }

    //断开坠落
    public void ToFall()
    {
        isShake = false;//停止震动
        Sprite.sortingOrder = SpriteManager.Bottom;//图层垫底
        mObj.layer = Layer.BubbleFall;
        Ridid.isKinematic = false;
        Ridid.fixedAngle = false;
        Ridid.gravityScale = FallGravity;
        Ridid.AddForce(new Vector2(Random.Range(-FALL_X, FALL_X),
            Random.Range(0, FALL_Y * 2)), ForceMode2D.Impulse);
        Slot = null;
    }
    #endregion
}
