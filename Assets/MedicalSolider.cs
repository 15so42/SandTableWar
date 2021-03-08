using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalSolider : BattleUnitBase
{
    public int curePoint;
    public int lastTime;
    public BattleUnitBase cureTarget;
    
    protected override void Awake()
    {
        base.Awake();
        lastTime = (int)Time.time;
    }

    protected override void Update()
    {
        base.Update();
        if (Time.time >= lastTime + 1 && curePoint<10)
        {
            curePoint++;
            SetBDCurePoint();
            lastTime = (int) Time.time;
        }
        
    }

    //设置行为树的治疗点数
    private void SetBDCurePoint()
    {
        behaviorDesigner.SetVariableValue("CurePoint",curePoint);
    }
    public void SetCureTarget(BattleUnitBase cureTarget)
    {
        this.cureTarget = cureTarget;
    }

    public void CureTargetUnit()
    {
        cureTarget.CureHp(10);
        curePoint = 0;
        SetBDCurePoint();
        SetCureTarget(null);
    }

    private Vector3 refCureDire;
    protected override void RotationControl()
    {
        
        base.RotationControl();
        //转向可能改为Action
        
        // MedicalStateController medicalStateController = stateController as MedicalStateController;
        // if(medicalStateController==null)
        //     return;
        // if(medicalStateController.currentState.stateName=="治疗")
        // {
        //     if(medicalStateController.cureTarget==null)
        //         return;
        //     Vector3 dir = medicalStateController.cureTarget.transform.position - transform.position;
        //     dir.y = 0;
        //     transform.forward = Vector3.SmoothDamp(transform.forward, dir, ref refCureDire, Time.deltaTime * rotateDamp);
        // }
    }
}
