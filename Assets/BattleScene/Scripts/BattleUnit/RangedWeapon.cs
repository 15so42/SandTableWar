
    using Photon.Pun;
    using UnityEngine;

    //子弹检测类型
    public enum RangedAttackDetectionType{
        Ray,//射线
        Livefire//实弹
    }
    public class RangedWeapon : Weapon
    {
        public Transform shootPos;
        public GameObject bullet;

        public float bulletDamageRate=1f;//子弹伤害倍率，主要由枪械武器的撞针力度导致不同
        
        public RangedAttackDetectionType detectionType;

        //散射角度，当检测方式为ray时，射击向量在玩家到敌人的方向进行对应度数的随机取值以模拟误差射击效果，如若射中玩家，发射子弹到对应位置并附带拖尾
        [Header("散射角度")]
        public float scatteringAngel=5;
        public override void Attack()
        {
            base.Attack();
            //todo 使用对象池
            if(detectionType == RangedAttackDetectionType.Ray)
            {
                Vector3 enemyDir = GetEnemy().transform.position- shootPos.transform.position;
                //散射算法
                Vector3 newVec = Quaternion.Euler(Random.Range(-scatteringAngel,scatteringAngel),Random.Range(-scatteringAngel,scatteringAngel),0)*enemyDir;
                PhotonView.Get(this).RPC("FireBullet",RpcTarget.All,newVec);
            }

            if (detectionType == RangedAttackDetectionType.Livefire)
            {
                PhotonView.Get(this).RPC("FireBullet",RpcTarget.All,shootPos.forward);
            }
           
            
        }

        [PunRPC]
        public void FireBullet(Vector3 shootDir)
        {
            //todo 使用对象池
            if (detectionType == RangedAttackDetectionType.Livefire)
            {
                GameObject iBullet=GameObject.Instantiate(bullet, shootPos.position, shootPos.rotation);
                iBullet.GetComponent<Bullet>().SetWeapon(this);
                iBullet.GetComponent<Bullet>().SetShooter(owner);
                iBullet.GetComponent<Rigidbody>().AddForce(shootDir*200f);
            }
            else if(detectionType == RangedAttackDetectionType.Ray)
            {
                GameObject iBullet=GameObject.Instantiate(bullet, shootPos.position, shootPos.rotation);
                iBullet.GetComponent<Bullet>().SetWeapon(this);
                iBullet.GetComponent<Bullet>().SetShooter(owner);
                iBullet.GetComponent<Rigidbody>().AddForce(shootDir*800f);
            }
           
        }

        public BattleUnitBase GetEnemy()
        {
            return owner.stateController.enemy;
        }
        
    }
