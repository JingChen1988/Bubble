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
    public bool isAbsorb;//是否吸附
    public Slot Slot;//泡泡所在槽位

    const int AbsorbSpeed = 20;//吸附速度
    const float AbsorbMinDis = .2f;//吸附最小距离
    const float Radius = 2.3f;//碰撞范围

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
        if (isAbsorb) return;
        Transform other = coll.transform;

        Debug.Log(name + ">>" + Ridid.velocity.ToString("f8"));
        Debug.DrawRay(coll.contacts[0].point, Ridid.velocity, Color.red, 60);

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
            Slot slot = other.GetComponent<Bubble>().Slot;
            SlotTable.Instance.Absorb(slot, this);
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
        while (true)
        {
            mTran.localPosition = Vector2.Lerp(mTran.localPosition, to, Time.deltaTime * AbsorbSpeed);
            float dis = Vector2.Distance(mTran.localPosition, to);
            if (dis <= AbsorbMinDis) break;
            yield return 0;
        }
        mTran.localPosition = to;
        Collider.radius = Radius;

        //进行连锁反应
        SlotTable.Instance.ChainBubble(Slot);
    }
}
