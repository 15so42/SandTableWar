using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WorkerUnit : BattleUnitBase
{
    public int mineOutputRate = 1;
    public BattleUnitBase mineTarget;
    [Header("MineMachine")] public GameObject MineMachineModel;
    public int setMineMachineTime = 10;
    private GameObject iMineMachine;
    private Sequence sequence;
   protected void Awake()
   {
       base.Awake();
       weapon = null;
       
   }

   public void SetMineTarget(BattleUnitBase mineTarget)
   {
       if (mineTarget == null)
       {
           this.mineTarget = null;
           return;
       }
       //当前挖矿目标
       MineralUnit thisMineralUnit= this.mineTarget as MineralUnit;
       MineralUnit mineralUnit=mineTarget as MineralUnit;
       
       //当前有挖矿目标时，取消当前挖矿目标
       if (this.mineTarget != null)
       {
           thisMineralUnit.HasWorkerWorking = false;
           thisMineralUnit.OnUnSelect();
       }
       if (mineralUnit.HasMineMachine)
       {
           TipsDialog.ShowDialog("该矿石已经有矿机了，无需操作");
           return;
       }
       if (mineralUnit.HasWorkerWorking == false)
       {
           mineralUnit.HasWorkerWorking = true;
           this.mineTarget = mineTarget;
       }
       else
       {
           this.mineTarget=FindOtherClosetMineral(mineTarget.transform.position);
           if(this.mineTarget==null)
               return;
           (this.mineTarget as MineralUnit).HasWorkerWorking = true;
       }

       if (this.mineTarget != null)
       {
           SetTargetPos(this.mineTarget.transform.position);
       }
       
       
   }

   public void SetMineMachine()
   {
       iMineMachine = BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.MineMachine),
           mineTarget.transform.position,GameManager.Instance.GetSelfId()).gameObject;
          // GameObject.Instantiate(MineMachineModel, mineTarget.transform.position, Quaternion.identity);
        iMineMachine.GetComponent<MineMachine>().buildTime = setMineMachineTime;
       sequence = DOTween.Sequence();
       //sequence.Join(iMineMachine.transform.DOShakeScale(2f));
       iMineMachine.transform.localScale=Vector3.zero;
       sequence.Join(iMineMachine.transform.DOScale(Vector3.one, setMineMachineTime/2f));
       sequence.Append(iMineMachine.transform.DOJump(iMineMachine.transform.position + Vector3.up * 2.5f , 0.3f, 3, setMineMachineTime));
        
       sequence.OnComplete(()=>
       {
           OnSetMineMachineComplete(iMineMachine);
       });
   }

   private void OnSetMineMachineComplete(GameObject mineMachine)
   {
       MineralUnit curMineral=mineTarget as MineralUnit;
       curMineral.HasWorkerWorking = false;
       curMineral.HasMineMachine = true;
       SetMineTarget(null);
       GetComponent<Animator>().SetBool("SetMineMachine",false);
       iMineMachine.GetComponent<MineMachine>().StartMine();
   }
   

   //毁坏矿机
   public void InterruptMineMachineSetUp()
   {
       sequence.Kill();
       if (iMineMachine)
       {
           iMineMachine.GetComponent<BattleUnitBase>().Die();
       }
       MineralUnit curMineral=mineTarget as MineralUnit;
       if (curMineral != null)//移动时已经提前打断挖矿，但是遇敌没有，所以遇敌时还有挖矿目标，需要处理
       {
           curMineral.HasWorkerWorking = false;
           curMineral.HasMineMachine = false;
           curMineral.OnUnSelect();
           SetMineTarget(null);
       }
      
       GetComponent<Animator>().SetBool("SetMineMachine",false);
   }
   
   public override void SetTargetPos(Vector3 pos,bool showMark=true)
   {
       //在有挖矿目标的时候设置其他目的地，则取消挖矿目标
       if (mineTarget!=null && mineTarget.transform.position!=pos)
       {
           (mineTarget as MineralUnit).HasWorkerWorking = false;
           mineTarget.OnUnSelect();
           SetMineTarget(null);    
       }
       base.SetTargetPos(pos);
   }

   private BattleUnitBase FindOtherClosetMineral(Vector3 pos)
   {
       Collider[] colliders = Physics.OverlapSphere(pos, 30);
       for (int i = 0; i < colliders.Length; i++)
       {
           BattleUnitBase battleUnitBase = colliders[i].GetComponent<BattleUnitBase>();
           if (battleUnitBase && battleUnitBase.configId == BattleUnitId.Mineral && battleUnitBase.IsInFog()==false &&
               (battleUnitBase as MineralUnit).HasWorkerWorking==false && (battleUnitBase as MineralUnit).HasMineMachine == false)
           {
               return battleUnitBase;
           }
       }

       return null;
   }

   public override void SetChaseTarget(BattleUnitBase battleUnitBase)
   {
       if (battleUnitBase.configId == BattleUnitId.Mineral)
       {
           SetMineTarget(battleUnitBase);
       }
   }

  
}
