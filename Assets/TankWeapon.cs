using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityTimer;

//tank必须炮管瞄准后才能发射
public class TankWeapon : RangedWeapon
{
    public Transform tower;
    public Transform barrel;//炮管
    [Header("坦克开炮后坐力")] public int recoil;
    // Start is called before the first frame update
   
    public override void WeaponUpdate()
    {
        atkTimer += Time.deltaTime;
        if (atkTimer > lastAtkTime + (float) 1 / atkRate)
        {
            BattleUnitBase enemy = GetEnemy();
            Vector3 enemyDir = enemy.transform.position-tower.transform.position;
            enemyDir.y = 0;
            Vector3 towerDir = tower.transform.forward;
            towerDir.y = 0;
            float angle = Vector3.Angle(enemyDir, towerDir);
           
            if (angle < 5f)
            {
                atkTimer = 0;
                Attack();
            }
            
        }
    }

    public override void Attack()
    {
        base.Attack();
        //坦克后坐力，效果不太好，关了
        // Rigidbody rigidbody = GetComponent<Rigidbody>();
        // rigidbody.isKinematic = false;
        // Vector3 shootDir = (shootPos.transform.position - GetEnemy().transform.position).normalized;
        // Quaternion rotate = Quaternion.Euler(0, 90, 0);//因为扭矩是绕向量旋转，所以需要先将向量绕y周旋转90
        // shootDir = rotate * shootDir;
        // rigidbody.AddTorque(shootDir*recoil,ForceMode.Impulse);
        // Timer.Register(3f, () =>
        // {
        //     if (rigidbody)
        //     {
        //         rigidbody.isKinematic = true;
        //     }
        // });
        barrel.transform.DOLocalMove(new Vector3(0,0,-0.6f), 0.1f).SetLoops(2,LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
