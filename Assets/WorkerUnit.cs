using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityTimer;

[RequireComponent(typeof(ResourceCollector))]
public class WorkerUnit : BattleUnitBase
{
    public ResourceInfo resourceTarget;
    [Header("MineMachine")] 
    public GameObject MineMachineModel;
    public int setMineMachineTime = 10;
  
    private Sequence SerMineMachineSequence;

    private ResourceCollector resourceCollector;

    //临时变量
    private GameObject iMineMachine;
   protected void Awake()
   {
       base.Awake();
       weapon = null;
       resourceCollector = GetComponent<ResourceCollector>();
       resourceCollector.RegisterSetTargetAction(SetTargetResource);
      
   }

   
   public void SetTargetResource(ResourceInfo target)
   {
       if (target == null)
       {
           resourceTarget = null;
           UpdateIdleStatus(true);
           return;
       }
      


       //当前有挖矿目标时，取消当前挖矿目标
       if (resourceTarget != null)
       {
           //当前挖矿目标
           BattleUnitBase preResourceUnit= this.resourceTarget.GetComponent<BattleUnitBase>();
           ResourceInfo preResourceInfo = preResourceUnit.GetComponent<ResourceInfo>();
           
           preResourceInfo.workerManager.Remove(this.resourceCollector);
           preResourceUnit.OnUnSelect();
       }
       if (target.IsEmpty)
       {
           TipsDialog.ShowDialog("目标资源已空或处于锁定状态");
           UpdateIdleStatus(true);
           return;
       }
       if (target.workerManager.CanAddWorker())
       {
           target.workerManager.Add(resourceCollector);
           this.resourceTarget = target;
       }
       else
       {
           this.resourceTarget = GetFactionManager().FindOtherNearestMineral(target.resourceTypeInfo.resourceType,target.transform.position);
           resourceCollector.target = resourceTarget;
           if (this.resourceTarget == null)
           {
               UpdateIdleStatus(true);
               return;
           }
           
           target.workerManager.Add(resourceCollector);
       }

       if (resourceTarget != null)
       {
           SetTargetPos(this.resourceTarget.transform.position);
           UpdateIdleStatus(false);
       }
   }
   

   public void SetMineMachine()
   {
       iMineMachine = BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.MineMachine),
           resourceTarget.transform.position,factionId).gameObject;
          // GameObject.Instantiate(MineMachineModel, mineTarget.transform.position, Quaternion.identity);
        iMineMachine.GetComponent<MineMachine>().buildTime = setMineMachineTime;
       SerMineMachineSequence = DOTween.Sequence();
       //sequence.Join(iMineMachine.transform.DOShakeScale(2f));
       iMineMachine.transform.localScale=Vector3.zero;
       SerMineMachineSequence.Join(iMineMachine.transform.DOScale(Vector3.one, setMineMachineTime/2f));
       SerMineMachineSequence.Append(iMineMachine.transform.DOJump(iMineMachine.transform.position + Vector3.up * 2.5f , 0.3f, 3, setMineMachineTime));
        
       SerMineMachineSequence.OnComplete(()=>
       {
           OnSetMineMachineComplete(iMineMachine);
       });
   }

   private void OnSetMineMachineComplete(GameObject mineMachine)
   {
       MineralUnit curMineral = resourceTarget.GetComponent<MineralUnit>();
     
       resourceTarget.workerManager.Remove(resourceCollector);
       curMineral.HasMineMachine = true;
       curMineral.MineMachine = mineMachine.GetComponent<MineMachine>();
       curMineral.GetComponent<ResourceInfo>().isEmpty = true;
       SetTargetResource(null);
       GetComponent<Animator>().SetBool("SetMineMachine",false);
       Timer.Register(2, () =>
       {
           iMineMachine.GetComponent<MineMachine>().StartMine();
           UpdateIdleStatus(true);
       });//播放起立动画后才算完成

   }
   

   //毁坏矿机
   public void InterruptMineMachineSetUp()
   {
       SerMineMachineSequence.Kill();
       if (iMineMachine)
       {
           iMineMachine.GetComponent<BattleUnitBase>().Die();
       }
       MineralUnit curMineral=resourceTarget.GetComponent<MineralUnit>();
       if (curMineral != null)//移动时已经提前打断挖矿，但是遇敌没有，所以遇敌时还有挖矿目标，需要处理
       {
           resourceTarget.workerManager.Remove(resourceCollector);
           curMineral.HasMineMachine = false;
           curMineral.OnUnSelect();
           SetTargetResource(null);
       }
      
       GetComponent<Animator>().SetBool("SetMineMachine",false);
   }

   public void InterruptCollectWood()
   {
       
   }
   
   public override void SetTargetPos(Vector3 pos,bool showMark=true)
   {
       //在有挖矿目标的时候设置其他目的地，则取消挖矿目标
       if (resourceTarget!=null && resourceTarget.transform.position!=pos)
       {
           resourceTarget.workerManager.Remove(resourceCollector);
           resourceTarget.GetComponent<MineralUnit>().OnUnSelect();
           SetTargetResource(null);    
       }
       base.SetTargetPos(pos);
   }

   private ResourceInfo FindOtherClosetMineral(ResourceType resourceType,Vector3 pos)
   {
       return GetFactionManager().FindOtherNearestMineral(resourceTarget.resourceTypeInfo.resourceType,pos);
       
   }

   public override void SetChaseTarget(BattleUnitBase battleUnitBase)
   {
       if (battleUnitBase.configId == BattleUnitId.Mineral)
       {
           SetTargetResource(battleUnitBase.GetComponent<ResourceInfo>());
       }
   }

  
  
}
