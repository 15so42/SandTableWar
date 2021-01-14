using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

//侦察兵
public class Scout : BattleUnitBase
{
   //被动：
   //深入敌营：7秒未受到伤害后每5秒回复10%生命值
   //其疾如风：有%30的概率闪避受到的伤害
   //广域视界：能发现视野距离一半距离内的隐形单位
   public float lastDamageTime;

   private Timer recoveryTimer;
   
   private void OnEnable()
   {
      if (!photonView.IsMine)
      {
         return;
      }
      
      prop.onTakeDamage+= OnTakDamage;
      lastDamageTime = Time.time;
   }

   protected override void Update()
   {
      if (!photonView.IsMine)
         return;
      base.Update();
      if (Time.time > lastDamageTime + 7)
      {
         if (recoveryTimer == null)
         {
            StartRecovery();
         }
      }
   }

   private void StartRecovery()
   {
      recoveryTimer=Timer.Register(5, () =>
      {
        CureHp((int)(prop.maxHp*0.1f));
      },null,true);
   }

   private void OnTakDamage()
   {
      lastDamageTime = Time.time;
      //停止回复
      if (recoveryTimer == null) 
         return;
      recoveryTimer.isLooped = false;
      recoveryTimer?.Cancel();
      recoveryTimer = null;
   }

   private void OnDisable()
   {
      if (!photonView.IsMine)
      {
         return;
      }
      prop.onTakeDamage = null;
   }
}
