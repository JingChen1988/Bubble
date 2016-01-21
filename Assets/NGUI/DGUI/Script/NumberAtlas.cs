using UnityEngine;
/// <summary>
/// 数字图集组件
/// </summary>
public class NumberAtlas : MonoBehaviour
{
    /// <summary>
    /// 数字停靠
    /// </summary>
    public enum AnchorNumber
    {
        Left = 0, Center = 1, Right = 2
    }

    Transform mTran;
    UISprite[] NumberSprits;
    int Number = -1;//显示0
    string key;//前缀
    public bool AllShow;//全部显示

    public AnchorNumber Anchor;//停靠属性
    int X;//初始X轴
    float Y;//初始Y轴
    int Center;//中心X轴

    void Start()
    {
        if (mTran == null) Init();
    }

    //初始化
    void Init()
    {
        mTran = transform;
        NumberSprits = new UISprite[mTran.childCount];
        for (int i = 0; i < NumberSprits.Length; i++)
            NumberSprits[i] = mTran.GetChild(i).GetComponent<UISprite>();

        key = mTran.GetChild(0).GetComponent<UISprite>().spriteName;
        key = key.Substring(0, key.IndexOf("_") + 1);

        X = (int)mTran.localPosition.x;
        Y = mTran.localPosition.y;
        Center = (int)(NumberSprits[NumberSprits.Length - 1].transform.localPosition.x / 2);
    }

    //显示数字
    public void SetNumber(int number)
    {
        if (mTran == null) Init();
        if (Number != number)
        {
            string text = number.ToString();
            for (int i = NumberSprits.Length - 1, j = text.Length - 1; i >= 0; i--, j--)
            {
                UISprite sprite = NumberSprits[i];
                GameObject obj = sprite.gameObject;
                if (j >= 0)
                {
                    sprite.spriteName = key + text.Substring(j, 1);
                    obj.SetActive(true);
                }
                else if (AllShow)
                {
                    sprite.spriteName = key + "0";
                    obj.SetActive(true);
                }
                else
                    obj.SetActive(false);
            }
            AnchorHandle();
        }
    }

    //停靠处理（默认靠右）
    void AnchorHandle()
    {
        //居中
        if (Anchor == AnchorNumber.Center)
        {
            float rightX = NumberSprits[NumberSprits.Length - 1].transform.localPosition.x;
            float leftX = 0;
            for (int i = NumberSprits.Length - 1; i >= 0; i--)
            {
                if (NumberSprits[i].gameObject.activeSelf)
                    leftX = NumberSprits[i].transform.localPosition.x;
                else
                    break;
            }

            float centerX = (rightX - leftX) / 2 + leftX;
            float offset = centerX - Center;
            mTran.localPosition = new Vector3(X - offset, Y, 0);
        }
        //靠左
        else if (Anchor == AnchorNumber.Left)
        {
            //float rightX = NumberSprits[NumberSprits.Length - 1].transform.localPosition.x;
            float leftX = 0;
            for (int i = NumberSprits.Length - 1; i >= 0; i--)
            {
                if (NumberSprits[i].gameObject.activeSelf)
                    leftX = NumberSprits[i].transform.localPosition.x;
                else
                    break;
            }

            mTran.localPosition = new Vector3(X - leftX, Y, 0);
        }
        //靠右
        else if (Anchor == AnchorNumber.Right)
        {
            mTran.localPosition = new Vector3(X, Y, 0);
        }
    }
}
