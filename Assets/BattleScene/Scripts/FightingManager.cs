﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEngine;

public class FightingManager
{
    public BattleSceneState battleSceneState;
    public LogicMap logicMap;
    public List<BattleUnitBase> selectedUnits = new List<BattleUnitBase>(); //选中的单位

    private Camera mainCamera;
    public int campId;

    //选中特效
    private const string SelectMarkPath = "Fx/SelectMark";
    private GameObject selectMarkInCache;
    
    //按键控制
    public bool isHoldShift;
    public bool isHoldCtrl;
    
    
    //资源,临时，之后抽象出具体类
    public BattleResMgr battleResMgr;
    public int coin = 20;

    public void Init()
    {
        mainCamera = Camera.main;
        logicMap = Object.FindObjectOfType<LogicMap>();
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameManager.PLAYER_CAMP_ID, out var value))
        {
            campId = (int)value;
        };
        selectMarkInCache = Resources.Load<GameObject>(SelectMarkPath);
        battleResMgr=new BattleResMgr();
    }

    public void SpawnBase()
    {
        BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(299),logicMap.GetBasePosByPlayerId(campId),campId);
        BattleCamera.Instance.SetLookPos(logicMap.GetBasePosByPlayerId(campId));
    }
    

    public void Update()
    {
        //todo 考虑按键控制抽象成单独类
        //0代表鼠标左键，1代表鼠标右键
        if (Input.GetMouseButtonDown(0))
        {
            MouseClickHandle(0);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            MouseClickHandle(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isHoldShift = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isHoldShift = false;
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isHoldCtrl = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isHoldCtrl = false;
        }
        
        battleResMgr.Update();
        
        
    }

    public void MouseClickHandle(int mouseBtn)
    {
        if (UITool.IsPointerOverUIObject(Input.mousePosition))
        {
            return;//防止UI穿透
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, 999))
        {
            if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                if (mouseBtn == 1)//点击鼠标右键时
                {
                    MoveToSpecificPos(raycastHit.point);
                }else if (mouseBtn == 0)
                {
                    UnselectAllUnits();
                }
            }
        }
    }

    #region 鼠标点击的命令

    /// <summary>
    /// 所有选中的目标移动到指定位置
    /// </summary>
    /// <param name="pos"></param>
    public void MoveToSpecificPos(Vector3 pos)
    {
        foreach (var unit in selectedUnits)
        {
            unit.SetTargetPos(pos);
            //todo 添加特效
            // GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // mark.transform.position = raycastHit.point;
        }
    }

    public void UnselectAllUnits()
    {
        for (int i=0;i<selectedUnits.Count;i++)
        {
            UnselectUnit(selectedUnits[i]);
        }
    }
    
    
    

    #endregion

    public void SelectUnit(BattleUnitBase unitBase)
    {
        if (!selectedUnits.Contains(unitBase))
        {
            selectedUnits.Add(unitBase);
            Transform unitBaseTransform = unitBase.transform;
            //选中特效
            if (unitBase.isFirstSelected)
            {
                
                GameObject iMark = Object.Instantiate(selectMarkInCache,
                    unitBaseTransform.position + unitBase.selectMarkOffset, 
                    Quaternion.Euler(new Vector3(90,0,0)), unitBaseTransform);
                unitBase.SetSelectMark(iMark);
            }
            unitBase.OnSelect();
        }
    }

    public void UnselectUnit(BattleUnitBase unitBase)
    {
        if (selectedUnits.Contains(unitBase))
        {
            selectedUnits.Remove(unitBase);
            //关闭选中特效
            unitBase.HideSelectMark();
            unitBase.OnUnSelect();
        }
    }
    
    public int CalDamage(int damage, int defense, DamageType damageType)
    {
        //使用英雄联盟的伤害计算公式
        return damage *(1 - (defense / (defense + 100)));
    }

    public void Attack(BattleUnitBase attcker,BattleUnitBase victim,int damageValue)
    {
       // Debug.Log($"单位{attcker.gameObject.name}对单位{victim.gameObject.name}找成了{damageValue}点伤害");
       victim.ReduceHp(damageValue);
    }
    
    /// <summary>
    /// 是否含有足够的资源生成指定单位
    /// </summary>
    /// <returns></returns>
    public bool HasEnoughResToSpawnSolider(int spawnId)
    {
        //todo 完善该方法
        SpawnBattleUnitConfigInfo curSpawnInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(spawnId);
        int needPopulation = curSpawnInfo.needPopulation;
        int needCoin = curSpawnInfo.needCoin;
        int needMineral = curSpawnInfo.needMineral;
        int needFood = curSpawnInfo.needFood;
        //先判断是否所有都满足再消耗
        if (battleResMgr.HasEnoughRes(BattleResType.population, needPopulation) &&
            battleResMgr.HasEnoughRes(BattleResType.coin, needCoin) &&
            battleResMgr.HasEnoughRes(BattleResType.mineral, needMineral) &&
            battleResMgr.HasEnoughRes(BattleResType.food, needFood))
        {
            return true;
        }

        return false;
    }

    public bool ConsumeResByUnitInfo(SpawnBattleUnitConfigInfo spawnInfo)
    {
        return battleResMgr.ConsumeResByUnitInfo(spawnInfo);
    }
    
}