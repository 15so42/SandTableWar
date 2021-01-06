using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

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
   protected override void Awake()
   {
      base.Awake();
      stateController=new TankStateController(this);
      tankAnimCtrl = GetComponent<TankAnimCtrl>();
      navMeshAgent.updatePosition = false;
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

   //用以平滑同步炮台和炮管旋转,因为TankAnimCtrl非本客户端不会运行，所以需要额外写一段
   protected override void Update()
   {
      base.Update();
      //同步炮塔和炮管方向
      tankAnimCtrl.tower.transform.rotation = Quaternion.RotateTowards(tankAnimCtrl.tower.transform.rotation, towerTargetRotation, tankAnimCtrl.towerRotateSpeed * Time.deltaTime);
      tankAnimCtrl.canon.transform.rotation = Quaternion.RotateTowards( tankAnimCtrl.canon.transform.rotation, canonTargetRotation, tankAnimCtrl.canonRotateSpeed * Time.deltaTime);
      
      //移动
      speed = Mathf.SmoothDamp(speed, navMeshAgent.desiredVelocity.magnitude>moveThreshold?maxMovesSpeed:0, ref refMoveSpeed, moveDampTime*Time.deltaTime);
      transform.Translate(Vector3.forward * (speed * Time.deltaTime),Space.Self);
      navMeshAgent.nextPosition = transform.position;
      
   }
   
}
