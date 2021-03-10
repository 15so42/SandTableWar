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
           
            Vector3 targetTurretLookPos;
            var turretTrans = turret.Value.transform;
            if (enemyUnit.Value != null)
                targetTurretLookPos = enemyUnit.Value.GetVictimPos();
            else
            {
                if (tacticalAgent.HasArrived() == false)
                {
                    targetTurretLookPos = tacticalAgent.GetDestinationPos();
                }
                else
                {
                    targetTurretLookPos = turretTrans.position + turretTrans.forward * 20f;
                }
            }
           
            //旋转炮管
            Vector3 turretVec = targetTurretLookPos - turretTrans.position;
            turretVec.y = 0;//炮塔只能水平旋转
            Quaternion q = Quaternion.LookRotation(turretVec);
            turretTrans.rotation = Quaternion.RotateTowards(turretTrans.rotation, q, turretRotateSpeed * Time.deltaTime);
            return TaskStatus.Running;
        }
    }
}