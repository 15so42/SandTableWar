using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tactical.Tasks
{
    [TaskCategory("MyRTS/Tank")]
    public class SyncTankAnim : MyRtsAction
    {
        public SharedTransform turret;//炮塔
        public SharedTransform barrel;//炮管
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
                if (tacticalAgent.HasArrived() == false)
                {
                    targetTurretLookPos = tacticalAgent.GetDestinationPos();
                }
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
}