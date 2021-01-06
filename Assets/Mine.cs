using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Object = System.Object;

public class Mine : BattleUnitBase
{
   public GameObject explosionFx;
   public float radius=3;
   
   protected override void Start()
   {
      base.Start();
      stateController = null;
   }

   private void OnTriggerEnter(Collider other)
   {
      BattleUnitBase battleUnitBase = other.GetComponent<BattleUnitBase>();
      if (battleUnitBase)
      {
         if (battleUnitBase.campId != campId)
         {
            Explosion();
         }
      }
   }

   public void Explosion()
   {
      PhotonView.Get(this).RPC(nameof(ExplosionInPhoton),RpcTarget.All);
   }

   [PunRPC]
   public void ExplosionInPhoton()
   {
      GameObject.Instantiate(explosionFx, transform.position, Quaternion.Euler(-90,0,0));
      //用碰撞体来判断而不是坐标，因为有些物体的半径超过检测半径就不会受伤但是实际上看起来已经碰到了
      Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
      foreach (var collider in colliders)
      {
         BattleUnitBase battleUnitBase = collider.GetComponentInChildren<BattleUnitBase>();
         if (battleUnitBase)
         {
            int damage = fightingManager.CalDamage(250, battleUnitBase.prop.defense, DamageType.Physical);
            fightingManager.Attack(this,battleUnitBase,damage);
         }
               
      }
   }
}
