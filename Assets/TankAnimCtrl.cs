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

    private Vector3 towerOffset;

    //tank 炮台和车身分离，不然车身旋转时炮台会被带动选中，看着有点怪，炮台和车身的方向应该独立
    protected override void Start()
    {
        base.Start();
        towerOffset = tower.transform.position - transform.position;
    }

    protected override void Update()
    {
        base.Update();
        tower.transform.position = transform.position + towerOffset;
    }

    public override void AttackAnim()
    {
        //no Anim
    }

    public override void DieAnim()
    {
        //no anim;
    }
}
