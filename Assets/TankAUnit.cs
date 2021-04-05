using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using DG.Tweening;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityTimer;

public class TankAUnit : BattleUnitBase,IPhotonViewCallback,IPunObservable
{
   private TankAnimCtrl tankAnimCtrl;
   private Quaternion towerTargetRotation;
   private Quaternion canonTargetRotation;
   public float maxMovesSpeed = 3.5f;
   public float moveDampTime = 50;
   private float refMoveSpeed;
   private float speed;
   public float moveThreshold = 0.6f;
   public float angelSpeed=60;

   public Transform[] damageSmokes;
   protected override void Awake()
   {
      base.Awake();
      tankAnimCtrl = GetComponent<TankAnimCtrl>();
      navMeshAgent.angularSpeed = 120;
      OnHpChanged.AddListener(UpdateDamageAppearance);
      viewDistanceAfterDie = prop.viewDistance;
   }


   public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
   {
      if (stream.IsWriting)
      {
         stream.SendNext(tankAnimCtrl.tower.transform.rotation);
         stream.SendNext(tankAnimCtrl.canon.transform.rotation);
      }
      else
      {
         towerTargetRotation = (Quaternion)stream.ReceiveNext();
         canonTargetRotation = (Quaternion)stream.ReceiveNext();
      }
     
   }
   

   private float viewDistanceAfterDie;
   //用以平滑同步炮台和炮管旋转,因为TankAnimCtrl非本客户端不会运行，所以需要额外写一段
   protected override void Update()
   {
      
    
      base.Update();
      //同步炮塔和炮管方向
      // tankAnimCtrl.tower.transform.rotation = Quaternion.RotateTowards(tankAnimCtrl.tower.transform.rotation, towerTargetRotation, tankAnimCtrl.towerRotateSpeed * Time.deltaTime);
      // tankAnimCtrl.canon.transform.rotation = Quaternion.RotateTowards( tankAnimCtrl.canon.transform.rotation, canonTargetRotation, tankAnimCtrl.canonRotateSpeed * Time.deltaTime);
      //

   }
   

   private void UpdateDamageAppearance()
   {
      float hpPercentage = prop.GetPercentage();
      for (int i = 0; i < damageSmokes.Length; i++)
      {
         if (hpPercentage < (1f / damageSmokes.Length) * i)
         {
            damageSmokes[i].gameObject.SetActive(true);
         }
         else
         {
            damageSmokes[i].gameObject.SetActive(false);
         }
      }

      if (hpPercentage < 0.1)
      {
         for (int i = 0; i < renderers.Length; i++)
         {
            renderers[i].material.DOColor(Color.white* hpPercentage*3, 1f);
         }
      }
     
   }

   public override void Die()
   {
      
      BattleFxManager.Instance.SpawnFxAtPosInPhotonByFxType(BattleFxType.MetalHitLarge,transform.position,Vector3.up); 
      base.Die();

   }
}
