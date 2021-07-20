using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public abstract class BuffBase : ScriptableObject
{
   public bool instant;//立即执行，表示无持续时间
   
   [Header("设置为-1即为永久")]
   public float duration;//持续时间
   public int plies=1;

   public bool isDurationStackAble;//持续时间可叠加,当获得同类型buff是否刷新持续时间
   public bool isPliesStackAble;//层数可叠加，当获得同类型buff是否叠加层数
   public float period=1;
   
   //计时
   private float leftTime;//运行时间
   private float periodTimer;

   public bool isFinished;
   private void OnEnable()
   {
      leftTime = duration;
   }

   public abstract void Init<T>(T t) where T: MonoBehaviour;
   
   //实施效果
   public abstract void Apply();

   protected void Tick(float deltaTime)
   {
      periodTimer += deltaTime;
      if (periodTimer > period)
      {
         PeriodTick();
         periodTimer = 0;
      }
      if (duration > 0)
      {
         leftTime -= deltaTime;//倒计时
      }
      if (leftTime <= 0 && duration>0)
      {
         End();
      }
   }

   protected abstract void PeriodTick();
 

   public virtual void OnSameBuffAdd(BuffBase newBuff)
   {
      if (isDurationStackAble)//可叠加时长时叠加市场，否则重置剩余时间
      {
         duration += newBuff.duration;
      }
      else
      {
         leftTime = duration;
      }

      if (isPliesStackAble)//叠加层数，否则重置剩余时间
      {
         plies++;
      }
      else
      {
         leftTime = duration;
      }
   }

   public virtual void End()
   {
      isFinished = true;
   }
   
}
