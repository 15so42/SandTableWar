using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

//子弹类型，子弹伤害计算通过武器类型和子弹类型计算，
//相同的枪射击不同子弹造成伤害不同，如果子弹有特殊属性也会造成不同伤害，如穿甲弹、高爆弹、火焰弹还会点燃敌人
//相同子弹被不同枪械设计造成的伤害也不同，取决于枪械的设计力量，射程等因素
public enum BulletType
{
    B_9mm,
    B_45mm,
    B_76mm,
    Tank_Tiger_Bullet
}
public class Bullet : RecycleAbleObject
{
    public BulletType bulletType;
    //射击者,传入射击者，计算伤害时考虑射击者buff，如射击者的增加子弹伤害的buff
    private BattleUnitBase shooter;

    //武器
    private Weapon weapon;
    public int baseDamage = 20;

    public List<HitFxConfig> hitFxConfigs;

    [Header("子弹碰撞完后生成的特效，比如坦克炮弹生成爆炸烟雾特效")]
    public BattleFxType onTriggerEndFxType;

    public DamageProp damageProp;

    private void Awake()
    {
        //初始化击中特效配置
        hitFxConfigs=new List<HitFxConfig>();
        hitFxConfigs.Add(new HitFxConfig(VictimMaterial.Human,BattleFxType.Blood_1));
        hitFxConfigs.Add(new HitFxConfig(VictimMaterial.Metal,BattleFxType.Blood_1));
        hitFxConfigs.Add(new HitFxConfig(VictimMaterial.Concrete,BattleFxType.Blood_1));
        //todo 应该还需要overrideHitFxConfigs的，但是今天不想写了，以后需要时再来写
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (shooter.IsMine())
        {
            return;
        }

        BattleUnitBase otherUnit = other.gameObject.GetComponent<BattleUnitBase>();
        
        //撞到障碍物上
        if (otherUnit == null)
        {
           OnTriggerObstacle(other.gameObject);
        }
        else
        {
            if (otherUnit == shooter || otherUnit.campId== shooter.campId)//避免友军伤害
            {
                return;
            }
            OnTriggerUnit(otherUnit);
        }

        //因为有时候是通过射线来判断的，所以onTriggerEnd不能写在这里
        OnTriggerEnd(transform.position);
       
    }

    public virtual void OnTriggerEnd(Vector3 pos)
    {
        if (onTriggerEndFxType != BattleFxType.None)
        {
            string fxName = ConfigHelper.Instance.GetFxPfbByBattleFxType(onTriggerEndFxType).name;
            BattleFxManager.Instance.SpawnFxAtPosInPhoton(fxName, pos, transform.forward);
        }
        
    }

    public virtual void OnTriggerObstacle(GameObject go,RaycastHit hitInfo=default)
    {
        //Rpc销毁子弹
        //PhotonView.Get(shooter).RPC("RecycleBullet",RpcTarget.All,this);
        if (hitInfo.collider != null)
        {
            //todo 障碍物碰撞材料未完成
            //string fxName = GetFxNameByHitConfig(targetUnitBase.victimMaterial);
            BattleFxManager.Instance.SpawnFxAtPosInPhoton("FX_DirtSplatter_Lash",hitInfo.point,hitInfo.normal);
        }
        
    }

    public virtual void OnTriggerUnit(BattleUnitBase targetUnitBase,RaycastHit hitInfo=default)
    {
        FightingManager fightingManager = GameManager.Instance.GetFightingManager();
        //计算伤害使用凶器进行计算，如使用子弹计算，但是记录是记录攻击者单位,凶器都需要拥有
        int damageValue = CalcuateDamageValue(targetUnitBase);
        //伤害机制还未确定，暂时使用攻击者，受害者，伤害量进行记录，不确定是否需要伤害记录功能，先留着吧
        fightingManager.Attack(shooter, targetUnitBase,damageValue);
        
        //Rpc销毁子弹
        //PhotonView.Get(shooter).RPC("RecycleBullet",RpcTarget.All,this);
        if (hitInfo.collider != null)
        {
            string fxName = GetFxNameByHitConfig(targetUnitBase.victimMaterial);
            BattleFxManager.Instance.SpawnFxAtPosInPhoton(fxName, hitInfo.point, hitInfo.normal);
        }
        
    }

    [PunRPC]
    public void SpawnFxInPhoton()
    {
        
    }

    public virtual string GetFxNameByHitConfig(VictimMaterial victimMaterial)
    {
        BattleFxType targetFxType = hitFxConfigs.Find(x => x.victimMaterial == victimMaterial).battleFxType;
        string fxName = ConfigHelper.Instance.GetFxPfbByBattleFxType(targetFxType).name;
        return fxName;
    }

        public override void ReUse()
    {
        base.ReUse();
        //需要的变量在重用前都会被重新设置而被覆盖为新值，所以不用管
        ////throw new System.NotImplementedException();
    }

    public override void Recycle()
    {
        base.Recycle();
        //throw new System.NotImplementedException();
    }

    public virtual int CalcuateDamageValue(BattleUnitBase victim)
    {
        //传入伤害值和防御值和伤害类型，伤害类型待定
        float damage = baseDamage * ((RangedWeapon) weapon).bulletDamageRate;
        return GameManager.Instance.GetFightingManager().CalDamage((int)damage, damageProp,victim.prop, DamageType.Physical);
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }

    public void SetShooter(BattleUnitBase unitBase)
    {
        shooter = unitBase;
    }
}



//击中特效配置
public class HitFxConfig
{
    public HitFxConfig(VictimMaterial victimMaterial, BattleFxType battleFxType)
    {
        this.victimMaterial = victimMaterial;
        this.battleFxType = battleFxType;
    }
    public VictimMaterial victimMaterial;
    public BattleFxType battleFxType;
}

