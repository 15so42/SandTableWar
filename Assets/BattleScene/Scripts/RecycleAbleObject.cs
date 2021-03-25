using System;
using UnityEngine;
using UnityTimer;

namespace BattleScene.Scripts
{
    public class RecycleAbleObject: MonoBehaviour
    {
        public float recycleTime;
        public bool autoRecycle = false;

        private Timer recycleTimer;
        void OnEnable()
        {
            if (autoRecycle)
            {
                if (recycleTimer == null)
                {
                    recycleTimer=Timer.Register(recycleTime, Recycle);
                }
                else
                {
                    recycleTimer.Cancel();
                    recycleTimer=Timer.Register(recycleTime, Recycle);//这玩意和协程类似，重启时必须关掉上一个，不然到后面无限个协程一起执行
                }
            }
        }

        private void OnDestroy()
        {
            recycleTimer?.Cancel();
        }

        public virtual void ReUse()
        {
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }
        }//重用,从对象池中取出，并且要求初始化

        public virtual void Recycle()
        {
            if (gameObject.activeSelf == false)
            {
                return;//已经回收了
            }
            UnityObjectPoolManager.Recycle(GetRecycleName(),this);
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }//回收

        private string GetRecycleName()//去掉实例化生成的Clone后缀已得到对应物体名
        {
            return gameObject.name.Replace("(Clone)", "");
        }
    }
}