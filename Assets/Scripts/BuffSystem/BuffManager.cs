using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoSingleton<BuffManager>
{
    public void AddBuff<T>(T buff, BuffsController target) where T: BuffBase
    {
        
        target.AddBuff(ScriptableObject.CreateInstance<T>());
    }
}
