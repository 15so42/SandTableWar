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

    private Vector3 lastTankEuler;
    protected override void Update()
    {
        base.Update();
        tower.transform.position = transform.position + towerOffset;
        //删除tank本身旋转时带动炮塔的旋转依保证炮塔方向正确
        float angle = transform.eulerAngles.y - lastTankEuler.y;
        Vector3 towerEuler = tower.transform.eulerAngles;
        tower.transform.eulerAngles=new Vector3(towerEuler.x,towerEuler.y-angle,towerEuler.z);
        lastTankEuler = transform.eulerAngles;

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
