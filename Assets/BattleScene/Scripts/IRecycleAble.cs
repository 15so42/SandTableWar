using UnityEngine;

namespace BattleScene.Scripts
{
    public abstract class RecycleAbleObject: MonoBehaviour
    {
        public virtual void ReUse()
        {
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }
        }//重用,从对象池中取出，并且要求初始化

        public virtual void Recycle()
        {
            UnityObjectPoolManager.Recycle(nameof(this.GetType),this);
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }//回收
    }
}