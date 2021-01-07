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
    
    /// <summary>
    /// 战斗中
    /// </summary>
    protected override void OnBattleState()
    {
        //base.OnBattleState();
        Vector3 enemyPos = stateController.enemy.GetVictimPos();
        Vector3 towerVec = enemyPos - tower.transform.position;
        towerVec.y = 0;//炮塔只能水平旋转
        Quaternion q = Quaternion.LookRotation(towerVec);
        tower.transform.rotation = Quaternion.RotateTowards(tower.transform.rotation, q, towerRotateSpeed * Time.deltaTime);


        //Vector3 canonVec = enemyPos - canon.transform.position;
        //canonVec.x = canon.transform.forward.x;
        //Quaternion q1 = Quaternion.LookRotation(canonVec);
        //canon.transform.rotation = Quaternion.RotateTowards(canon.transform.rotation, q1, canonRotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 正常移动中
    /// </summary>
    protected override void OnIdleOrMoveState()
    {
        //base.OnIdleOrMoveState();
        //炮塔匀速旋转
        Vector3 towerVec = stateController.targetPos - tower.transform.position;
        towerVec.y = 0;//炮塔只能水平旋转
        Quaternion q = Quaternion.LookRotation(towerVec);
        tower.transform.rotation = Quaternion.RotateTowards(tower.transform.rotation, q, towerRotateSpeed * Time.deltaTime);
        
        canon.transform.rotation = Quaternion.RotateTowards(canon.transform.rotation, tower.transform.rotation, canonRotateSpeed * Time.deltaTime);

    }

    public override void AttackAnim()
    {
        //no Anim
    }
}
