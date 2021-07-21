using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsController : MonoBehaviour
{
    [SerializeField] private List<BuffBase> buffs=new List<BuffBase>();

    public void AddBuff(BuffBase newBuff)
    {
        //判断是否有同类型buff
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].buffType == newBuff.buffType)//同类型
            {
                buffs[i].OnSameBuffAdd(newBuff);
            }
        }
        buffs.Add(newBuff);
    }
    
    public void Update()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            BuffBase buffBase = buffs[i];
            if (buffBase.isFinished || buffBase == null)
            {
                buffs.Remove(buffBase);
                i--;
            }
        }
    }
    
}
