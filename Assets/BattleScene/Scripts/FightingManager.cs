using System.Collections;
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
    
    //资源
    public BattleResMgr battleResMgr;

    public GlobalItemManager globalItemManager;
    public bool isUsingItem;
    public GameObject usingItemGo;

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
        if (PhotonNetwork.IsMasterClient)
        {
            BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(298),Vector3.zero,-1);//生成碉堡

        }
            
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
        if (isUsingItem)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 999))
            {
                globalItemManager.GetUsingItemRangeMark().transform.position = raycastHit.point;//技能范围，参考英雄联盟
            }

            if (Input.GetMouseButtonUp(0))
            {
                isUsingItem = false;
                GameObject.Destroy(globalItemManager.GetUsingItemRangeMark());
                globalItemManager.ActAtPos(raycastHit.point);
                globalItemManager.ClearUsingItem();
            }
            
        }
        
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

    /// <summary>
    /// 追踪目标
    /// </summary>
    public void ChaseTargetUnit(BattleUnitBase battleUnitBase)
    {
        foreach (var unit in selectedUnits)
        {
            unit.SetChaseTarget(battleUnitBase);
        }
    }

    public void UnselectAllUnits()
    {
        while (selectedUnits.Count > 0)
        {
            UnselectUnit(selectedUnits[0]);
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

    public void InitSelectMarkForUnit(BattleUnitBase unitBase)
    {
        Transform unitBaseTransform = unitBase.transform;
        GameObject iMark = Object.Instantiate(selectMarkInCache,
            unitBaseTransform.position + unitBase.selectMarkOffset, 
            Quaternion.Euler(new Vector3(90,0,0)), unitBaseTransform);
        unitBase.SetSelectMark(iMark);
        unitBase.HideSelectMark();
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
       //在建筑物内时通过根据建筑物血量百分比闪避伤害
       if (victim.isInBuilding)
       {
           DefenceBuilding building = victim.defenceBuilding;
           if (building)
           {
               float percentage = building.prop.GetPercentage();
               if (Random.Range(0, percentage)<=percentage)
               {
                   Attack(attcker,building,damageValue);
                   return;//玩家概率免伤
               }
           }
       }
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