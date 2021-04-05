using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using DefaultNamespace;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class FightingManager
{
    public SceneState battleSceneState;
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
    //建筑物拖拽，用于设置出生点
    public bool isDragFromBuilding;
    public BaseBattleBuilding buildingSpawnMark;

    //通过建筑菜单生成建筑
    public bool isBuildingPreview;
    public PreviewBuilding previewBuilding;//建造预览中的建筑
    public GameObject usingItemGo;

    private GameManager gameManager;
    public void Init()
    {
        mainCamera = Camera.main;
        logicMap = Object.FindObjectOfType<LogicMap>();
        gameManager=GameManager.Instance;
        //单机
        if (gameManager.gameMode == GameMode.Campaign)
        {
            campId = 0;
        }else if (gameManager.gameMode == GameMode.PVP)
        {
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(GameManager.PLAYER_CAMP_ID, out var value))
            {
                campId = (int)value;
            };
        }
        selectMarkInCache = Resources.Load<GameObject>(SelectMarkPath);
        battleResMgr=new BattleResMgr();
    }

    private BattleUnitBase enemyBase;
    public void SpawnBase()
    {
        //单机
        if (gameManager.gameMode==GameMode.Campaign)
        {
            //友方基地
            BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.Base),logicMap.GetBasePosByPlayerId(campId),campId);
            BattleCamera.Instance.SetLookPos(logicMap.GetBasePosByPlayerId(campId));//相机位置
            BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.Bunker_M),Vector3.zero,-1);//生成碉堡
            //敌人基地
            enemyBase=BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.Base),logicMap.GetBasePosByPlayerId(campId+1),campId+1);
            
            //矿物
            for (int i = 0; i < logicMap.minerals.Count; i++)
            {
                Transform tmpMineral = logicMap.minerals[i];
                tmpMineral.gameObject.SetActive(false);
                BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.Mineral),tmpMineral.position,-1);//生成矿物
            }
            return;
        }
        
        //联网
        //基地
        BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.Base),logicMap.GetBasePosByPlayerId(campId),campId);
        BattleCamera.Instance.SetLookPos(logicMap.GetBasePosByPlayerId(campId));
        if (PhotonNetwork.IsMasterClient)
        {
            BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.Bunker_M),Vector3.zero,-1);//生成碉堡
            
            for (int i = 0; i < logicMap.minerals.Count; i++)
            {
                Transform tmpMineral = logicMap.minerals[i];
                tmpMineral.gameObject.SetActive(false);
                BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(BattleUnitId.Mineral),tmpMineral.position,-1);//生成矿物
            }
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
        if (isUsingItem || isDragFromBuilding || isBuildingPreview)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 999,LayerMask.GetMask("Ground")))
            {
                if (isUsingItem)
                {
                    globalItemManager.GetUsingItemRangeMark().transform.position = raycastHit.point;//技能范围，参考英雄联盟
                }

                if (isDragFromBuilding)
                {
                    buildingSpawnMark.GetSpawnMark().transform.position = raycastHit.point;
                }
                
                if (isBuildingPreview)
                {
                    previewBuilding.transform.position=raycastHit.point;
                    
                    //Graphics.DrawMeshNow(previewBuilding.buildingPreviewMesh,raycastHit.point,Quaternion.identity);
                    //previewBuilding.transform.position = raycastHit.point;
                }
                
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isUsingItem)
                {
                    isUsingItem = false;
                    GameObject.Destroy(globalItemManager.GetUsingItemRangeMark());
                    globalItemManager.ActAtPos(raycastHit.point);
                    globalItemManager.ClearUsingItem();
                }

                if (isDragFromBuilding)
                {
                    buildingSpawnMark.OnDragMarkEnd();
                }

                if (isBuildingPreview)
                {
                    previewBuilding.OnBuildingPreviewEnd(raycastHit.point);
                    isBuildingPreview = false;
                }
                
            }
            
        }
        
        //游戏时间控制，用于Debug
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Time.timeScale = 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            (enemyBase as BaseBattleBuilding).AddUnitToSpawnStack(BattleUnitId.Ranger);
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
        //selectedUnits[0].SetFlockLeader(selectedUnits);
        
        List<DestinationSphereData> destinationSphereData=new List<DestinationSphereData>();
        int i = 0;
        float radius=1;//每填充一个单位，随机位置半径增大1
        foreach (var unit in selectedUnits)
        {
            if(unit==null || unit.navMeshAgent==false)
                return;//单位已经被消灭
            unit.navMeshAgent.avoidancePriority = i;
            i++;

            int randomTime = 20;
            while (randomTime > 0)//次数限制
            {
                randomTime--;

                Vector3 randomPos = pos + Random.insideUnitSphere * radius;

                if (NavMesh.SamplePosition(randomPos, out var hit, radius, -1))
                {
                    var sampledPos = hit.position;//不一定能找到可移动的位置
                    if (CanAgentReach(destinationSphereData, sampledPos, unit.navMeshAgent.radius))//能防止
                    {
                        destinationSphereData.Add(new DestinationSphereData(sampledPos,radius));
                        unit.SetTargetPos(sampledPos);
                        break;
                    }
                }
                radius++;
              
            }

          
            
           

            //radius++;
           
            
            //unit.SetTargetPos(pos);
            //todo 添加特效
            // GameObject mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            // mark.transform.position = raycastHit.point;
        }

        // for (int j = 0; j < destinationSphereData.Count; j++)
        // {
        //     unit
        // }
    }
    
    //检测预测终点是否已经有其他agent占用
    public bool CanAgentReach(List<DestinationSphereData> destinationSphereData,Vector3 sampledPos,float radius)
    {
        for (int i = 0; i < destinationSphereData.Count; i++)
        {
            Vector3 targetPoint = destinationSphereData[i].point;
            float targetRadius = destinationSphereData[i].radius;
            if (Vector3.Distance(sampledPos, targetPoint) < targetRadius+ radius+0.5f)//与任意一个圆相交则错误
            {
                return false;
            }
        }

        return true;
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
            if (unitBase == null)
            {
                return;
            }
            //关闭选中特效
            unitBase.HideSelectMark();
            unitBase.OnUnSelect();
        }
    }
    
    public int CalDamage(int damage,DamageProp attackerDamageProp,BattleUnitBaseProp  victimProp, DamageType damageType)
    {
        int resultDamage = 0;
        if (damageType == DamageType.Physical)
        {
            int reducedPenetrateResistance =  victimProp.penetrateDamageResistance - attackerDamageProp.penetrateResistanceIgnoreValue ;
            reducedPenetrateResistance = reducedPenetrateResistance < 0 ? 0 : reducedPenetrateResistance;
            float penetrateDamage = damage  *
                                  (1 - (float)reducedPenetrateResistance /
                                      (reducedPenetrateResistance + 50));
            
            int reducedExplosionResistance = victimProp.explosionDamageResistance - attackerDamageProp.explosionResistanceIgnoreValue ;
            reducedExplosionResistance = reducedExplosionResistance < 0 ? 0 : reducedExplosionResistance;
            float explosionDamage = damage * 
                                    (1 - (float)reducedExplosionResistance /
                                        (reducedExplosionResistance + 50));
            resultDamage = (int)penetrateDamage + (int)explosionDamage;
        }
        //使用英雄联盟的伤害计算公式
        return resultDamage;
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
    public bool HasEnoughResToSpawnSolider(BattleUnitId spawnId)
    {
        //todo 完善该方法
        SpawnBattleUnitConfigInfo curSpawnInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(spawnId);
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

/////////////////////////////////用于多单位设置终点////////////////////////////////
public class DestinationSphereData
{
    public DestinationSphereData(Vector3 pos, float radius)
    {
        this.point = pos;
        this.radius = radius;
    }
    public Vector3 point;
    public float radius;
}