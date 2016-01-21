using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// UI动画播放器
/// </summary>
public class UIAnimation : MonoBehaviour
{
    GameObject mObj;
    UISprite Sprite;

    bool play;//是否播放
    string SpriteName;//图集名称
    string Prefix;//图集前缀
    float mDelta;
    int index;//图集索引
    float during;//定时播放

    List<string> SpriteNames = new List<string>();//图集名称集合
    int SpriteCount;

    public bool Loop;//是否循环播放
    public bool Invisible;//是否自动隐藏
    public bool PingPong;//是否来回播放
    public int FPS = 20;//播放帧数
    public float Rate = 3;//循环频率
    bool Forward;//播放方向

    void Start()
    {
        mObj = gameObject;
        Sprite = GetComponent<UISprite>();
        SpriteName = Sprite.spriteName;
        Prefix = SpriteName.Substring(0, SpriteName.IndexOf('_') + 1);

        //获取动画名称列表
        List<UISpriteData> sprites = Sprite.atlas.spriteList;
        for (int i = 0, imax = sprites.Count; i < imax; ++i)
        {
            UISpriteData sprite = sprites[i];
            if (string.IsNullOrEmpty(Prefix) || sprite.name.StartsWith(Prefix))
                SpriteNames.Add(sprite.name);
        }
        SpriteNames.Sort();
        SpriteCount = SpriteNames.Count;

        if (Invisible) Sprite.alpha = 0;
    }

    //播放UI动画
    public void Play()
    {
        play = true;
        Forward = true;
        if (Invisible)
        {
            mObj.SetActive(true);
            Sprite.alpha = 1;
        }
    }

    void Update()
    {
        //播放
        if (play && SpriteCount > 1)//&& Application.isPlaying
        {
            mDelta += RealTime.deltaTime;
            float rate = 1f / FPS;

            if (rate < mDelta)
            {
                mDelta = (rate > 0f) ? mDelta - rate : 0f;
                if (Forward)//正向播放
                {
                    if (++index >= SpriteCount)
                    {
                        if (PingPong)
                        {
                            index = SpriteCount - 1;
                            Forward = false;
                        }
                        else
                        {
                            index = 0;
                            play = false;
                        }
                    }
                }
                else if (PingPong && !Forward)//反向播放
                {
                    if (--index <= 0)
                    {
                        Forward = true;
                        play = false;
                    }
                }
                //切换图片
                if (play) Sprite.spriteName = SpriteNames[index];
            }
        }

        //定时调用
        if (Loop)
        {
            during -= RealTime.deltaTime;
            if (during < 0)
            {
                during = Rate;
                Play();
            }
        }
        StopHandle();
    }

    //停止处理
    void StopHandle()
    {
        if (!play && mObj.activeSelf)
        {
            Sprite.spriteName = SpriteName;
            if (Invisible && mObj.activeSelf)
            {
                Sprite.alpha = 0;
                mObj.SetActive(false);
            }
        }
    }
}
