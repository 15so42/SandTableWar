using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("MyRTS/AutomaticBattery")]
public class SyncAutomaticBatteryAnim : Action
{
    public SharedTransform turret;//炮塔
    public SharedBattleUnit enemyUnit;
    [Header("炮塔转速")] public int turretRotateSpeed=45;
    
    public override TaskStatus OnUpdate()
    {
        var turretTrans = turret.Value.transform;
            
        Vector3 targetTurretLookPos = Vector3.zero;
        if (enemyUnit.Value != null)
            targetTurretLookPos = enemyUnit.Value.GetVictimPos();
        else
        {
            targetTurretLookPos = Vector3.forward;

        }
           
        //旋转炮管
        Vector3 turretVec = targetTurretLookPos - turretTrans.position;
        turretVec.y = 0;//炮塔只能水平旋转
        if (turretVec == Vector3.zero)//没有明确指向时只想tank前方
        {
            turretVec = transform.forward;
        }
          
        Quaternion q = Quaternion.LookRotation(turretVec);
        turretTrans.rotation = Quaternion.RotateTowards(turretTrans.rotation, q, turretRotateSpeed * Time.deltaTime);
        
        return TaskStatus.Running;
    }
}
