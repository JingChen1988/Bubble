using UnityEngine;
/// <summary>
/// 资源管理器
/// </summary>
public class ResourceManager : MonoBehaviour
{
    #region 初始化参数
    static bool IsInitSetting;//系统设置初始化
    #endregion

    #region 资源初始化
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        if (!IsInitSetting)
        {
            GameObject[] other = Resources.LoadAll<GameObject>("Prefabs");
            PrefabList.Bubble = other[0];
            PrefabList.Circle = other[1];
            PrefabList.Explode = other[2];
            PrefabList.SpriteManager = other[3];

            Resources.UnloadUnusedAssets();

            IsInitSetting = true;
            Data.LoadData();//读取玩家数据
            Internation.LoadLanguage();//加载语言设置
            Logger.INFO("加载游戏配置");
        }
    }
    #endregion

    //获取资源对象
    public static GameObject GetResource(byte key)
    {
        GameObject obj = null;
        obj = obj ?? GetObject(key);
        return obj;
    }

    public static GameObject GetResource(string key)
    {
        GameObject obj = null;
        obj = obj ?? GetObject(key);
        return obj;
    }

    //获取资源对象
    static GameObject GetObject(byte key)
    {
        GameObject obj = null;
        switch (key)
        {
            case PrefabKey.Bubble: obj = PrefabList.Bubble; break;
            case PrefabKey.Explode: obj = PrefabList.Explode; break;
        }
        return obj;
    }

    static GameObject GetObject(string key)
    {
        GameObject obj = null;
        //switch (key)
        //{

        //}
        return obj;
    }
}

/// <summary>
/// 预制对象缓存
/// </summary>
public class PrefabList
{
    public static GameObject SpriteManager;
    public static GameObject Bubble;
    public static GameObject Explode;
    public static GameObject Circle;

    public class UI
    {
        public static GameObject LevelIndex;
    }
}
/// <summary>
/// 预制对象关键字
/// </summary>
public class PrefabKey
{
    public const byte Bubble = 0;
    public const byte Explode = 1;

    public class UI
    {
        public const byte LevelIndex = 200;
    }

}

/// <summary>
/// 音频列表
/// </summary>
public class AudioList
{
    public static AudioClip BallFall;
}