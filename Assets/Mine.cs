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
      GameObject.Instantiate(explosionFx, transform.position, Quaternion.identity);
      foreach (var enemyUnit in enemyUnits)
      {
         if (Vector3.Distance(enemyUnit.transform.position, transform.position) < radius)
         {
            int damage = fightingManager.CalDamage(250, enemyUnit.prop.defense, DamageType.Physical);
            fightingManager.Attack(this,enemyUnit,damage);
         }
      }
   }
}
