using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicalSolider : BattleUnitBase
{
    public int curePoint;
    public int lastTime;
    
    protected override void Awake()
    {
        base.Awake();
        lastTime = (int)Time.time;
        stateController=new MedicalStateController(this);
    }

    protected override void Update()
    {
        base.Update();
        if (Time.time >= lastTime + 1 && curePoint<10)
        {
            curePoint++;
            lastTime = (int) Time.time;
        }
        
    }

    private Vector3 refCureDire;
    protected override void RotationControl()
    {
        base.RotationControl();
        
        MedicalStateController medicalStateController = stateController as MedicalStateController;
        if(medicalStateController==null)
            return;
        if(medicalStateController.currentState.stateName=="治疗")
        {
            if(medicalStateController.cureTarget==null)
                return;
            Vector3 dir = medicalStateController.cureTarget.transform.position - transform.position;
            dir.y = 0;
            transform.forward = Vector3.SmoothDamp(transform.forward, dir, ref refCureDire, Time.deltaTime * rotateDamp);
        }
    }
}
