using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 对象池管理器
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    static Dictionary<byte, GameObject[]> PoolList = new Dictionary<byte, GameObject[]>();

    const int PoolSize = 128;//池对象数量

    void Awake() { Instance = this; }

    //获取游戏对象
    public static GameObject GetObject(byte key)
    {
        GameObject obj = null;

        //缓存池中有对象
        if (PoolList.ContainsKey(key))
        {
            GameObject[] pools = PoolList[key];
            //获取空闲对象
            for (int i = 0; i < PoolSize; i++)
            {
                GameObject temp = pools[i];
                if (temp != null && !temp.activeSelf)
                {
                    obj = temp;
                    Logger.INFO("激活 >> " + key);
                    break;
                }
            }

            //实例化新对象
            if (obj == null)
            {
                for (int i = 0; i < PoolSize; i++)
                {
                    GameObject temp = pools[i];
                    if (temp == null)
                    {
                        obj = Object.Instantiate(ResourceManager.GetResource(key)) as GameObject;
                        obj.name = obj.name + i;
                        pools[i] = obj;
                        Logger.INFO("实例 >> " + key);
                        break;
                    }
                }
            }
        }
        //缓存池中无对象，则实例化新对象，放入池中
        else
        {
            GameObject[] pools = new GameObject[PoolSize];
            obj = Object.Instantiate(ResourceManager.GetResource(key)) as GameObject;
            pools[0] = obj;
            PoolList.Add(key, pools);
            Logger.INFO("缓存池 >> " + key);
        }
        return obj;
    }

    //将对象放入池中
    public static void AddPool(byte key, GameObject obj)
    {
        if (PoolList.ContainsKey(key))
        {
            GameObject[] pools = PoolList[key];
            //获取空闲对象
            for (int i = 0; i < PoolSize; i++)
            {
                GameObject temp = pools[i];
                if (temp != null)
                {
                    pools[i] = obj;
                    Logger.INFO("追加 >> " + key);
                    break;
                }
            }
        }
        else
        {
            GameObject[] pools = new GameObject[PoolSize];
            pools[0] = obj;
            PoolList.Add(key, pools);
            Logger.INFO("缓存池 >> " + key);
        }
    }

    //获取所有激活对象
    public static List<GameObject> GetActiveObjects(byte key)
    {
        List<GameObject> objs = new List<GameObject>();
        //缓存池中有对象
        if (PoolList.ContainsKey(key))
        {
            GameObject[] pools = PoolList[key];
            //获取激活对象
            for (int i = 0; i < PoolSize; i++)
            {
                GameObject temp = pools[i];
                if (temp != null && temp.activeSelf)
                    objs.Add(temp);
            }
        }
        return objs;
    }

    //获取所有激活对象
    public static List<GameObject> GetActiveObjects(byte[] keys)
    {
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0, len = keys.Length; i < len; i++)
            objs.AddRange(GetActiveObjects(keys[i]));
        return objs;
    }

    //回收所有对象
    public static void CollectAllObject()
    {
        foreach (KeyValuePair<byte, GameObject[]> pools in PoolList)
        {
            GameObject[] objs = pools.Value;
            for (int i = 0; i < PoolSize; i++)
            {
                GameObject temp = objs[i];
                if (temp != null && temp.activeSelf) temp.SetActive(false);
            }
        }
    }

    //清空池对象
    public static void ClearAllObject() { PoolList.Clear(); }

    #region 定时函数
    //启动定时回收
    public static void TimingCollect(float time, GameObject target) { PoolManager.Instance.StartCoroutine(CollectObject(time, target)); }
    //启动定时销毁
    public static void TimingDestory(float time, GameObject target) { PoolManager.Instance.StartCoroutine(DestoryObject(time, target)); }

    //定时回收
    static IEnumerator CollectObject(float time, GameObject target)
    {
        yield return new WaitForSeconds(time);
        target.SetActive(false);
    }

    //定时销毁
    static IEnumerator DestoryObject(float time, GameObject target)
    {
        yield return new WaitForSeconds(time);
        Destroy(target);
    }
    #endregion
}
