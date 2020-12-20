
    using Photon.Pun;
    using UnityEngine;
    using UnityTimer;

    //子弹检测类型
    public enum RangedAttackDetectionType{
        RayWithTrail,//子弹瞬间到达，但是会留下烟雾弹道，多见于狙击枪
        RayWithWithLightTracer,//曳光弹，多用于机枪和冲锋枪，肉眼看来只能看到在空中出现一瞬间的长条状发光子弹而非全程
        Livefire//实弹
    }
    public class RangedWeapon : Weapon
    {
        public Transform shootPos;
        public GameObject bullet;

        public float bulletDamageRate=1f;//子弹伤害倍率，主要由枪械武器的撞针力度导致不同
        
        public RangedAttackDetectionType detectionType;

        //散射角度，当检测方式为ray时，射击向量在玩家到敌人的方向进行对应度数的随机取值以模拟误差射击效果，如若射中玩家或者障碍物，发射子弹到对应位置并附带拖尾
        [Header("散射角度")]
        public float scatteringAngel=5;
        public override void Attack()
        {
            base.Attack();
            //todo 使用对象池
            if(detectionType == RangedAttackDetectionType.RayWithTrail||detectionType == RangedAttackDetectionType.RayWithWithLightTracer)
            {
                Vector3 enemyDir = GetEnemy().transform.position- shootPos.transform.position;
                //散射算法
                Vector3 newVec = Quaternion.Euler(Random.Range(-scatteringAngel,scatteringAngel),Random.Range(-scatteringAngel,scatteringAngel),0)*enemyDir;
                //PhotonView.Get(this).RPC("FireBullet",RpcTarget.All,newVec);
                
                RaycastHit hitInfo;
                if (Physics.Raycast(shootPos.transform.position,newVec, out hitInfo,40))//todo 考虑设置枪械攻击距离
                {
                    //因为需要使用子弹计算伤害，所以先在本地生成子弹，再在其他终端生成子弹，并通过本地子弹扣除其他终端血量
                    PhotonView.Get(this).RPC("FireBullet",RpcTarget.Others,hitInfo.point);
                    Bullet iBullet=FireBullet(hitInfo.point);
                    //扣除击中目标血量
                    BattleUnitBase hitUnit = hitInfo.transform.GetComponent<BattleUnitBase>();
                    if (hitUnit)
                    {
                        iBullet.OnTriggerUnit(hitUnit);
                    }
                }
            }
            // if(detectionType == RangedAttackDetectionType.RayWithWithLightTracer)
            // {
            //     Vector3 enemyDir = GetEnemy().transform.position- shootPos.transform.position;
            //     //散射算法
            //     Vector3 newVec = Quaternion.Euler(Random.Range(-scatteringAngel,scatteringAngel),Random.Range(-scatteringAngel,scatteringAngel),0)*enemyDir;
            //     //PhotonView.Get(this).RPC("FireBullet",RpcTarget.All,newVec);
            //     
            //     RaycastHit hitInfo;
            //     if (Physics.Raycast(shootPos.transform.position,newVec, out hitInfo,40))//todo 考虑设置枪械攻击距离
            //     {
            //         //因为需要使用子弹计算伤害，所以先在本地生成子弹，再在其他终端生成子弹，并通过本地子弹扣除其他终端血量
            //         PhotonView.Get(this).RPC("FireBullet",RpcTarget.Others,hitInfo.point);
            //         Bullet iBullet=FireBullet(hitInfo.point);
            //         //扣除击中目标血量
            //         BattleUnitBase hitUnit = hitInfo.transform.GetComponent<BattleUnitBase>();
            //         if (hitUnit)
            //         {
            //             iBullet.OnTriggerUnit(hitUnit);
            //         }
            //     }
            // }

            if (detectionType == RangedAttackDetectionType.Livefire)
            {
                PhotonView.Get(this).RPC("FireBullet",RpcTarget.All,shootPos.forward);
            }
           
            
        }

        //当射击模式为ray时，传入射线最终目标位置，为实弹时传入发射向量，有特殊情况重写此方法
        [PunRPC]
        public virtual Bullet FireBullet(Vector3 param)
        {
            //todo 使用对象池
            if (detectionType == RangedAttackDetectionType.Livefire)
            {
                GameObject iBullet=GameObject.Instantiate(bullet, shootPos.position, shootPos.rotation);
                iBullet.GetComponent<Bullet>().SetWeapon(this);
                iBullet.GetComponent<Bullet>().SetShooter(owner);
                iBullet.GetComponent<Rigidbody>().AddForce(param*200f);
                return iBullet.GetComponent<Bullet>();
            }
            //param表示射线射击到的位置，通过直接把子弹生成后并设置到对应位置来表示一次带拖尾的射击
            //通过射线检测的方式如果射击到敌人直接调用rpc扣血，不需要实体子弹判断
            else if(detectionType == RangedAttackDetectionType.RayWithTrail)
            {
                GameObject iBullet=GameObject.Instantiate(bullet, shootPos.position, shootPos.rotation);
                iBullet.GetComponent<Bullet>().SetWeapon(this);
                iBullet.GetComponent<Bullet>().SetShooter(owner);
                //iBullet.GetComponent<Rigidbody>().AddForce(param*800f);
                //iBullet.transform.position = param;
                Timer.Register(0.05f, () =>
                    iBullet.transform.position = param);
                return iBullet.GetComponent<Bullet>();
            }
            //param表示向量
            else if(detectionType == RangedAttackDetectionType.RayWithWithLightTracer)
            {
                GameObject iBullet=GameObject.Instantiate(bullet, shootPos.position, shootPos.rotation);
                iBullet.GetComponent<Bullet>().SetWeapon(this);
                iBullet.GetComponent<Bullet>().SetShooter(owner);
                //iBullet.GetComponent<Rigidbody>().AddForce(param*800f);
                //iBullet.transform.position = param;
                Timer.Register(0.05f, () =>
                    iBullet.transform.position = param);
                return iBullet.GetComponent<Bullet>();
            }

            return null;
        }

        public BattleUnitBase GetEnemy()
        {
            return owner.stateController.enemy;
        }
        
    }
