using UnityEngine;
using System.Collections;
/// <summary>
/// 抬头数字显示
/// </summary>
public class HUD : MonoBehaviour
{
    protected Transform mTran;
    public static Camera GameCamera;
    public static Camera UICamera;
    GameObject[] HUDModels;

    #region 周期
    public float RiseTime = 2f;//上升时间
    public float JumpTime = 2f;//跳跃时间
    public float ErasureTime = 5f;//擦除时间
    public float ScaleTime = 2f;//缩放时间
    #endregion

    #region 动画曲线
    //速度曲线
    public AnimationCurve SpeedC = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 60f), new Keyframe(2f, 100f) });
    //缩放-Rise
    public AnimationCurve ScaleRise = new AnimationCurve(new Keyframe[] { new Keyframe(0f, .2f), new Keyframe(.2f, 1.2f), new Keyframe(.3f, 1f), new Keyframe(2f, .9f) });
    //缩放-Jump
    public AnimationCurve ScaleJump = new AnimationCurve(new Keyframe[] { new Keyframe(0f, .5f), new Keyframe(.2f, 1f), new Keyframe(.3f, 1f), new Keyframe(2f, .7f) });
    //透明度曲线
    public AnimationCurve AlphaC = new AnimationCurve(new Keyframe[] { new Keyframe(.1f, 1f), new Keyframe(2, 0f) });
    //X轴曲线
    public AnimationCurve XOffsetC = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(.4f, 100f), new Keyframe(2f, 300f) });
    //Y轴曲线
    public AnimationCurve YOffsetC = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(.3f, 100f), new Keyframe(.4f, 100f), new Keyframe(2f, -250f) });
    //擦除曲线
    AnimationCurve ErasureOffset;
    //缩放-Rise
    AnimationCurve ScaleCurve;
    #endregion

    void Start()
    {
        mTran = transform;
        GameCamera = Camera.main;
        UICamera = GameObject.Find("MainUI/Camera").GetComponent<Camera>();

        //初始化HUD对象
        int count = mTran.childCount;
        HUDModels = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            HUDModels[i] = mTran.GetChild(i).gameObject;
            HUDModels[i].SetActive(false);
        }

        //初始化曲线
        ErasureOffset = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(ErasureTime * .2f, 1f), new Keyframe(ErasureTime * .8f, 1f), new Keyframe(ErasureTime, 0f) });
        ScaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(ScaleTime * .1f, 1f), new Keyframe(ScaleTime * .9f, 1f), new Keyframe(ScaleTime, 0f) });
    }

    #region 核心函数
    Transform DisplayUP(Transform Target, HUDType type, string key, string text, bool isFollow, Vector2 offset)
    {
        //获取HUD对象
        GameObject UDObj = GetHUDObj(key);

        //初始化UD组件
        Transform UITran = UIPool.GetObject(key, UDObj).transform;
        Transform ContextTran = UITran.GetChild(0);
        UIWidget UIComponet = ContextTran.GetComponent<UIWidget>();

        //设置对应关联
        UITran.parent = mTran;
        UITran.localScale = Vector3.one;
        ContextTran.localPosition = Vector3.zero;
        UITran.gameObject.SetActive(true);
        //初始化位置
        if (offset == Vector2.zero) Follow(UITran, Target);
        else Follow(UITran, Target, offset);

        //开启HUD特效
        switch (type)
        {
            case HUDType.Rise:
                StartCoroutine(Rise(Target, UITran, ContextTran, UIComponet, text, isFollow, offset)); break;
            case HUDType.Jump:
                StartCoroutine(Jump(Target, UITran, ContextTran, UIComponet, text, isFollow)); break;
            case HUDType.Scale:
                StartCoroutine(Scale(Target, UITran, ContextTran, UIComponet, text, isFollow, offset)); break;
            case HUDType.Erasure:
                StartCoroutine(Erasure(Target, UITran, ContextTran, UIComponet, text, isFollow)); break;
        }
        return UITran;
    }

    //容器在UI上跟随目标位置
    public static void Follow(Transform UITran, Transform pivot)
    {
        Follow(UITran, pivot, Vector2.zero);
    }
    public static void Follow(Transform UITran, Transform pivot, Vector2 offset)
    {
        Vector3 pos = GameCamera.WorldToViewportPoint(pivot.position);
        Vector3 uiPos = UICamera.ViewportToWorldPoint(pos);
        UITran.position = uiPos;
        pos = UITran.localPosition;
        pos.x = Mathf.FloorToInt(pos.x);
        pos.y = Mathf.FloorToInt(pos.y);
        pos.z = 0f;
        UITran.localPosition = pos + new Vector3(offset.x, offset.y, 0);
    }

    //获取HUD对象
    GameObject GetHUDObj(string key)
    {
        GameObject HUDObj = null;
        for (int i = 0, len = HUDModels.Length; i < len; i++)
        {
            if (HUDModels[i].name == key)
            {
                HUDObj = HUDModels[i];
                break;
            }
        }
        return HUDObj;
    }
    #endregion

    #region HUD特效
    //上升(文本&图片)
    IEnumerator Rise(Transform Target, Transform UITran, Transform ContextTran, UIWidget UIComponet, string text, bool isFollow, Vector2 offset)
    {
        if (UIComponet is UILabel) ((UILabel)UIComponet).text = text;
        if (UIComponet is UISprite) ((UISprite)UIComponet).spriteName = text;

        float time = RiseTime;
        float ShowTime = 0;
        while (ShowTime < time)
        {
            //跟随
            if (isFollow)
            {
                if (offset == Vector2.zero) Follow(UITran, Target);
                else Follow(UITran, Target, offset);
            }

            ShowTime += Time.deltaTime;
            float scale = ScaleRise.Evaluate(ShowTime);
            ContextTran.transform.localScale = new Vector3(scale, scale, scale);
            float alpha = AlphaC.Evaluate(ShowTime);
            UIComponet.alpha = alpha;

            float UpSpeed = SpeedC.Evaluate(ShowTime);
            float move = UpSpeed * Time.deltaTime;
            ContextTran.transform.localPosition += Vector3.up * move;

            yield return new WaitForEndOfFrame();
        }
        UIPool.Collect(UITran.gameObject);
    }

    //跳跃(文本&图片)
    IEnumerator Jump(Transform Target, Transform UITran, Transform ContextTran, UIWidget UIComponet, string text, bool isFollow)
    {
        if (UIComponet is UILabel) ((UILabel)UIComponet).text = text;
        if (UIComponet is UISprite) ((UISprite)UIComponet).spriteName = text;

        float XRandom = Random.Range(8, 13) * (Random.Range(0, 2) == 0 ? .1f : -.1f);
        float YRandom = Random.Range(10, 15) * .1f;

        float time = JumpTime;
        float ShowTime = 0;
        while (ShowTime < time)
        {
            if (isFollow) Follow(UITran, Target);
            ShowTime += Time.deltaTime;

            float scale = ScaleJump.Evaluate(ShowTime);
            ContextTran.transform.localScale = new Vector3(scale, scale, scale);
            float alpha = AlphaC.Evaluate(ShowTime);
            UIComponet.alpha = alpha;

            float XOffset = XOffsetC.Evaluate(ShowTime) * XRandom;
            float YOffset = YOffsetC.Evaluate(ShowTime) * YRandom;
            Vector3 lp = ContextTran.transform.localPosition;
            ContextTran.transform.localPosition = new Vector3(XOffset, YOffset, lp.z);

            yield return new WaitForEndOfFrame();
        }
        UIPool.Collect(UITran.gameObject);
    }

    //缩放(文本&图片)
    IEnumerator Scale(Transform Target, Transform UITran, Transform ContextTran, UIWidget UIComponet, string text, bool isFollow, Vector2 offset)
    {
        if (text != null)
        {
            if (UIComponet is UILabel) ((UILabel)UIComponet).text = text;
            if (UIComponet is UISprite) ((UISprite)UIComponet).spriteName = text;
        }

        float time = ScaleTime;
        float ShowTime = 0;
        while (ShowTime < time)
        {
            //跟随
            if (isFollow)
            {
                if (offset == Vector2.zero) Follow(UITran, Target);
                else Follow(UITran, Target, offset);
            }

            ShowTime += Time.deltaTime;

            float scale = ScaleCurve.Evaluate(ShowTime);
            ContextTran.localScale = new Vector3(scale, scale, scale);

            yield return new WaitForEndOfFrame();
        }
        UIPool.Collect(UITran.gameObject);
    }

    //擦除(图片)
    IEnumerator Erasure(Transform Target, Transform UITran, Transform ContextTran, UIWidget UIComponet, string text, bool isFollow)
    {
        if (UIComponet is UISprite)
        {
            UISprite sprite = ContextTran.GetComponent<UISprite>();
            sprite.spriteName = text;

            float YAxis = ContextTran.localPosition.y;
            float Height = UIComponet.width;

            float time = ErasureTime;
            float ShowTime = 0;
            while (ShowTime < time)
            {
                if (isFollow) Follow(UITran, Target);
                ShowTime += Time.deltaTime;

                float offset = ErasureOffset.Evaluate(ShowTime);
                float Y = YAxis - Height * (1 - offset);
                sprite.fillAmount = offset;
                ContextTran.localPosition = new Vector3(ContextTran.localPosition.x, Y, 0);

                yield return new WaitForEndOfFrame();
            }
        }
        UIPool.Collect(UITran.gameObject);
    }
    #endregion

    #region 展示HUD
    public Transform Display(Transform Target, HUDType hudType, string key, bool isFollow = false) { return DisplayUP(Target, hudType, key, null, isFollow, Vector2.zero); }
    public Transform Display(Transform Target, HUDType hudType, string key, string text, bool isFollow = false) { return DisplayUP(Target, hudType, key, text, isFollow, Vector2.zero); }
    public Transform Display(Transform Target, HUDType hudType, string key, string text, Vector2 offset, bool isFollow = false) { return DisplayUP(Target, hudType, key, text, isFollow, offset); }
    #endregion
}
