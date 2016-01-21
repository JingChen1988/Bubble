using UnityEngine;
using System.Collections;

public class SpriteManager : MonoBehaviour
{
    static SpriteManager Instance;
    //精灵集合
    public Sprite[] SpriteList;

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

    public static SpriteManager GetInstance()
    {
        if (Instance == null) Instance = PrefabList.SpriteManager.GetComponent<SpriteManager>();
        return Instance;
    }
}
