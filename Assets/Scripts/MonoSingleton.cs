using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    // T 代表子类型 通过 where T : MonoSingleton<T> 来体现
    //按需加载
    private static T instance;

    //只能读取get
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //在场景中根据类型来查找引用
                //只执行一次
                instance = FindObjectOfType<T>();
                //场景中没这个类型==游戏对象未挂载脚本
                if (instance == null)
                {
                    //创建一个脚本对象（立即执行Awake）
                    new GameObject("Singleton of " + typeof(T)).AddComponent<T>();
                }
            }

            return instance;
        }
    }

    protected  void Awake()
    {
        instance = this as T;
    }
}