using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// UI对象池
/// </summary>
public class UIPool
{
    static Dictionary<string, GameObject[]> PoolList = new Dictionary<string, GameObject[]>();
    const int PoolSize = 32;

    //获取游戏对象
    public static GameObject GetObject(string key, GameObject target = null)
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
                        obj = Object.Instantiate(target) as GameObject;
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
            obj = Object.Instantiate(target) as GameObject;
            pools[0] = obj;
            PoolList.Add(key, pools);
            Logger.INFO("缓存池 >> " + key);
        }
        return obj;
    }

    //将对象放入池中
    public static void AddPool(string key, GameObject obj)
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

    //定时回收
    public static void Collect(GameObject target) { target.SetActive(false); }
}
