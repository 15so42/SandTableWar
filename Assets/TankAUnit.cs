using System.Collections;
using System.Collections.Generic;
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
   protected override void Awake()
   {
      base.Awake();
      stateController=new TankStateController(this);
      tankAnimCtrl = GetComponent<TankAnimCtrl>();
      navMeshAgent.angularSpeed = 120;
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
   

   protected override void RotationControl()
   {
      return;
      Vector3 horDir = navMeshAgent.desiredVelocity;
      horDir.y = 0;
      if(horDir==Vector3.zero)
         return;
      if(navMeshAgent.remainingDistance<=navMeshAgent.stoppingDistance)
         return;
      
      Quaternion q=Quaternion.LookRotation(horDir);
      transform.rotation = Quaternion.RotateTowards(transform.rotation, q, angelSpeed * Time.deltaTime);
   }

   private Vector3 lastTankEuler;
   //用以平滑同步炮台和炮管旋转,因为TankAnimCtrl非本客户端不会运行，所以需要额外写一段
   protected override void Update()
   {
      float angle = transform.eulerAngles.y - lastTankEuler.y;
      Vector3 towerEuler = tankAnimCtrl.tower.transform.eulerAngles;
      tankAnimCtrl.tower.transform.eulerAngles=new Vector3(towerEuler.x,towerEuler.y-angle,towerEuler.z);
      lastTankEuler = transform.eulerAngles;
      
      base.Update();
      //同步炮塔和炮管方向
      tankAnimCtrl.tower.transform.rotation = Quaternion.RotateTowards(tankAnimCtrl.tower.transform.rotation, towerTargetRotation, tankAnimCtrl.towerRotateSpeed * Time.deltaTime);
      tankAnimCtrl.canon.transform.rotation = Quaternion.RotateTowards( tankAnimCtrl.canon.transform.rotation, canonTargetRotation, tankAnimCtrl.canonRotateSpeed * Time.deltaTime);
      
   }
   
   
   
}
