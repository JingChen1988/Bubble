using UnityEngine;
/// <summary>
/// 图集管理器
/// </summary>
public class SpriteManager : MonoBehaviour
{
    static SpriteManager Instance;

    public Sprite[] SpriteList;//精灵集合

    #region 图层
    public const int Bottom = -1;
    public const int Bubble = 2;
    public const int Top = 4;
    #endregion

    public Sprite GetSprite(string name)
    {
        Sprite sprite = null;
        for (int i = 0, len = SpriteList.Length; i < len; i++)
        {
            if (SpriteList[i])
            {
                Sprite sp = SpriteList[i];
                if (name == sp.name)
                {
                    sprite = sp;
                    break;
                }
            }
        }
        return sprite;
    }

    //获取管理器实例
    public static SpriteManager GetInstance()
    {
        Instance = Instance ?? PrefabList.SpriteManager.GetComponent<SpriteManager>();
        return Instance;
    }
}
