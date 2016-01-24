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

    public char ID;//对应泡泡标识，字符"-"代表空
    public Slot Slot;//泡泡所在槽位

    #region 吸附
    public bool isAbsorb;//是否吸附

    const int AbsorbSpeed = 20;//吸附速度
    const float AbsorbMinDis = .2f;//吸附最小距离
    const float Radius = 2.3f;//碰撞范围
    #endregion

    #region 震动
    public bool isShake;//是否震动

    const int ScopeBase = 3;//幅度基数
    const int ShakeMax = 5;//最大值
    const float ShakeDelay = .05f;//回复延迟
    const int ShakeSpeed = 15;//速度
    const float ShakeMinDis = .1f;//最小距离
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
        ID = id;
        Sprite.sprite = SpriteManager.GetInstance().GetSprite(ID.ToString());
        isAbsorb = false;
        Ridid.gravityScale = 0;
        Collider.enabled = true;
    }
    #endregion

    //碰撞处理
    void OnCollisionEnter2D(Collision2D coll)
    {
        Transform other = coll.transform;
        if (other.CompareTag(Tag.Bottom))
        {//4.碰撞地面消失
            mObj.SetActive(false);
        }

        if (isAbsorb) return;

        if (other.CompareTag(Tag.Wall))
        {//1.碰撞墙壁反弹
            CannonControl.Instance.InverseDirect(mTran);
        }
        else if (other.CompareTag(Tag.Top))
        {//2.碰撞顶部吸附
            isAbsorb = true;
            SlotTable.Instance.AbsorbTop(this);
        }
        else if (other.CompareTag(Tag.Bubble))
        {//3.碰撞泡泡吸附
            isAbsorb = true;
            Bubble bubble = other.GetComponent<Bubble>();
            SlotTable.Instance.Absorb(bubble.Slot, this);
        }
    }

    //泡泡吸附动画
    public void AbsorbAction(Slot slot) { StartCoroutine(AbsorbSlot(slot)); }
    IEnumerator AbsorbSlot(Slot slot)
    {   //吸附到指定槽位
        Slot = slot;
        Vector2 to = Slot.Position;
        Ridid.isKinematic = true;
        Slot.Bubble = this;
        //吸附动画
        while (true)
        {
            mTran.localPosition = Vector2.Lerp(mTran.localPosition, to, Time.deltaTime * AbsorbSpeed);
            float dis = Vector2.Distance(mTran.localPosition, to);
            if (dis <= AbsorbMinDis) break;
            yield return 0;
        }
        mTran.localPosition = to;
        Collider.radius = Radius;

        //检查震动是否结束
        while (true)
        {
            if (SlotTable.Instance.ShakeFinish()) break;
            yield return new WaitForSeconds(ShakeDelay);
        }

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
            mTran.localPosition = Vector2.Lerp(mTran.localPosition, to, Time.deltaTime * ShakeSpeed);
            float dis = Vector2.Distance(mTran.localPosition, to);
            if (dis <= ShakeMinDis) break;
            yield return 0;
        }

        //归位
        while (true)
        {
            mTran.localPosition = Vector2.Lerp(mTran.localPosition, from, Time.deltaTime * ShakeSpeed);
            float dis = Vector2.Distance(mTran.localPosition, from);
            if (dis <= ShakeMinDis) break;
            yield return 0;
        }
        mTran.localPosition = from;

        //状态回复
        yield return new WaitForSeconds(ShakeDelay);
        isShake = false;
    }
}
