using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoSingleton<BuffManager>
{
    public void AddBuff(BuffBase buff, BuffsController target)
    {
        
        target.AddBuff(ScriptableObject.CreateInstance<>() );
    }
}
