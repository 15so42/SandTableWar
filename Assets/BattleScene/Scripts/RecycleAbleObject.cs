using System;
using UnityEngine;
using UnityTimer;

namespace BattleScene.Scripts
{
    public class RecycleAbleObject: MonoBehaviour
    {
        public float recycleTime;
        public bool autoRecycle = false;
        void OnEnable()
        {
            if (autoRecycle)
            {
                Timer.Register(recycleTime, Recycle);
            }
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