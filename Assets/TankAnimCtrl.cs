using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAnimCtrl : BattleUnitAnimCtrl
{
    [Header("轮胎")]
    public Transform[] wheels;

    [Header("炮台")] public Transform tower;
    public float towerRotateSpeed=30f;
    [Header("炮管")] public Transform canon;
    public float canonRotateSpeed;
    
    protected override void OnBattleState()
    {
        //base.OnBattleState();
        Vector3 enemyPos = stateController.enemy.transform.position;
        Vector3 towerVec = enemyPos - tower.transform.position;
        towerVec.y = 0;//炮塔只能水平旋转
        Quaternion q = Quaternion.LookRotation(towerVec);
        tower.transform.rotation = Quaternion.RotateTowards(tower.transform.rotation, q, towerRotateSpeed * Time.deltaTime);
        Quaternion q1 = Quaternion.LookRotation(enemyPos-canon.transform.position);
        canon.transform.rotation = Quaternion.RotateTowards(canon.transform.rotation, q, canonRotateSpeed * Time.deltaTime);
    }

    protected override void OnIdleOrMoveState()
    {
        //base.OnIdleOrMoveState();
        //炮塔匀速旋转
        Vector3 towerVec = stateController.targetPos - tower.transform.position;
        towerVec.y = 0;//炮塔只能水平旋转
        Quaternion q = Quaternion.LookRotation(towerVec);
        tower.transform.rotation = Quaternion.RotateTowards(tower.transform.rotation, q, towerRotateSpeed * Time.deltaTime);
        Quaternion q1 = Quaternion.LookRotation(stateController.targetPos-canon.transform.position);
        canon.transform.rotation = Quaternion.RotateTowards(canon.transform.rotation, q, canonRotateSpeed * Time.deltaTime);

    }

    public override void AttackAnim()
    {
        //no Anim
    }
}
