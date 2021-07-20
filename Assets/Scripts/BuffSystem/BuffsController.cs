using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsController : MonoBehaviour
{
    [SerializeField] private List<BuffBase> buffs=new List<BuffBase>();

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
