using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityTimer;

//tank必须炮管瞄准后才能发射
public class TankWeapon : RangedWeapon
{
    private Transform tower;
    public Transform barrel;//炮管
    [Header("坦克开炮后坐力")] public int recoil;
    // Start is called before the first frame update
    void Start()
    {
        tower = GetComponent<TankAnimCtrl>().tower;
    }
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
            Debug.Log(angle);
            if (Vector3.Angle(enemyDir, towerDir) < 10f)
            {
                atkTimer = 0;
                Attack();
            }
            
        }
    }

    public override void Attack()
    {
        base.Attack();
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        Vector3 shootDir = shootPos.transform.position - GetEnemy().transform.position;
        rigidbody.AddForce(shootDir*recoil+Vector3.up);
        Timer.Register(0.5f,() =>
           rigidbody.isKinematic = true);
        barrel.transform.DOLocalMove(new Vector3(0,0,-0.6f), 0.1f).SetLoops(2,LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
