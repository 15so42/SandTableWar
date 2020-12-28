
using System.Collections.Generic;
using System.Linq;
using BattleScene.Scripts;
using UnityEngine;

/// <summary>
/// 对象池类管理类
/// </summary>
public class UnityObjectPoolManager
{
    /// <summary>
    /// 管理类中有多个池，每个池对应一种物体,一般为类名和对应的gameObject，比如子弹Bullet类对应Bullet、Bullet(Clone)等Go
    /// </summary>
    public static Dictionary<string,Stack<RecycleAbleObject>> pools=new Dictionary<string, Stack<RecycleAbleObject>>();

    private static int eachPoolSize = 50;//每个单独的池子的大小
    
    
    //取得
    public static RecycleAbleObject Allocate(string poolName)
    {
        if (pools.Keys.Contains(poolName) && pools[poolName].Count > 0)
        {
            //Debug.LogError($"从{poolName}中取出一个东西，取出前数量为{pools[poolName].Count}");
            RecycleAbleObject recycleAbleObject=pools[poolName].Pop();
            recycleAbleObject.ReUse();
            //Debug.LogError($"从{poolName}中取出一个东西，取出后数量为{pools[poolName].Count}");
            return recycleAbleObject;
            
        }

        return null;//池子中没有东西或者没有对应池子
    }
    
    public static void Recycle(string poolName,RecycleAbleObject recycleAbleObject)
    {
        if (pools.Keys.Contains(poolName))
        {
            //Debug.LogError($"准备回收到{poolName}中，回收前数量为{pools[poolName].Count}");
            if (pools[poolName].Count < eachPoolSize)//控制池子大小
            {
                pools[poolName].Push(recycleAbleObject);
                //Debug.LogError($"回收到{poolName}中，回收后数量为{pools[poolName].Count}");
            }
            else
            {
                //Debug.LogError("对象池超出容量!!!!!!!!!!!!!");
                GameObject.Destroy(recycleAbleObject);
            }
        }
        else//没有对应池子，生成新池子
        {
            pools.Add(poolName,new Stack<RecycleAbleObject>());
            Recycle(poolName,recycleAbleObject);
        }
    }
    
  
}