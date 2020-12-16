using System;
using System.Collections;
using System.Collections.Generic;
using BattleScene.Scripts;
using Photon.Pun;
using UnityEngine;

//子弹类型，子弹伤害计算通过武器类型和子弹类型计算，
//相同的枪射击不同子弹造成伤害不同，如果子弹有特殊属性也会造成不同伤害，如穿甲弹、高爆弹、火焰弹还会点燃敌人
//相同子弹被不同枪械设计造成的伤害也不同，取决于枪械的设计力量，射程等因素
public enum BulletType
{
    B_9mm,
    B_45mm,
    B_76mm
}
public class Bullet : MonoBehaviour,IRecycleAble
{
    public BulletType bulletType;
    //射击者,传入射击者，计算伤害时考虑射击者buff，如射击者的增加子弹伤害的buff
    private BattleUnitBase shooter;

    //武器
    private Weapon weapon;
    public int baseDamage = 20;
    
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
            if (otherUnit == shooter)
            {
                return;
            }
            OnTriggerUnit(otherUnit);
        }
    }

    protected virtual void OnTriggerObstacle(GameObject go)
    {
        //Rpc销毁子弹
        //PhotonView.Get(shooter).RPC("RecycleBullet",RpcTarget.All,this);
        Recycle();
    }

    protected virtual void OnTriggerUnit(BattleUnitBase unitBase)
    {
        FightingManager fightingManager = GameManager.Instance.GetFightingManager();
        //计算伤害使用凶器进行计算，如使用子弹计算，但是记录是记录攻击者单位,凶器都需要拥有
        int damageValue = CalcuateDamageValue(unitBase);
        //伤害机制还未确定，暂时使用攻击者，受害者，伤害量进行记录，不确定是否需要伤害记录功能，先留着吧
        fightingManager.Attack(shooter, unitBase,damageValue);
        
        //Rpc销毁子弹
        //PhotonView.Get(shooter).RPC("RecycleBullet",RpcTarget.All,this);
        Recycle();
    }

    public void ReUse()
    {
        //throw new System.NotImplementedException();
    }

    public void Recycle()
    {
        //子弹回收一般由RPC控制，因此执行本地逻辑即可
        Destroy(this);
        //throw new System.NotImplementedException();
    }

    public virtual int CalcuateDamageValue(BattleUnitBase victim)
    {
        //传入伤害值和防御值和伤害类型，伤害类型待定
        float damage = baseDamage * ((RangedWeapon) weapon).bulletDamageRate;
        return GameManager.Instance.GetFightingManager().CalDamage((int)damage, victim.prop.defense, DamageType.Physical);
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
