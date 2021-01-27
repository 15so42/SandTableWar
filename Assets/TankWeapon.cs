using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//tank必须炮管瞄准后才能发射
public class TankWeapon : RangedWeapon
{
    private Transform tower;
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
            BattleUnitBase enemy = owner.stateController.enemy;
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
