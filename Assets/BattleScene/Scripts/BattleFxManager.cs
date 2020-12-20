using UnityEngine;

namespace BattleScene.Scripts
{
    public class BattleFxManager
    {
        public const string fxPath = "Prefab/Fx/";
        public static void SpawnFxAtPos(string fxName,Vector3 pos,Vector3 forward)
        {
            RecycleAbleObject recycleAbleObject = UnityObjectPoolManager.Allocate(fxName);//对象池
            GameObject fxGo;
            if (recycleAbleObject==null)
            {
                fxGo= GameObject.Instantiate(Resources.Load<GameObject>(fxPath + fxName));
            }
            else
            {
                fxGo = recycleAbleObject.gameObject;
            }

            fxGo.transform.position = pos;
            fxGo.transform.forward = forward;
            
        }
    }
}