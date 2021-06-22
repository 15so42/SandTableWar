using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FoW;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityTimer;

public class BaseBattleBuilding : BattleUnitBase
{
    private float timer = 0;
    private float interval = 0;
    
    //public int spawnId=1;//作为建筑一般是产出士兵等活动单位，士兵等活动单位通过id确定

    //private SpawnBattleUnitConfigInfo curSpawnInfo;

    public Transform spawnPos;
    public Stack<BattleUnitId> toSpawn=new Stack<BattleUnitId>();
    [Header("建筑菜单，使用字符串表示对应菜单")]
    public BuildingMenuCommand[] menuCommands;

    [Header("出生点标志")] public GameObject spawnMarkPfb;
    public GameObject spawnMark;
    private BattleBuildingMenuDialog buildingMenuDialog;

    private Timer spawnMarkFadeTimer;
    private bool isBuilding=true;
    
    [Header("建造动画模型")]public GameObject animModel;
    [Header("建造完成闪光MeshRender效果")] public MeshRenderer[] meshRenderers;
    [Header("建造预览模型配置")] public MeshFilter[] meshFilters;
    
    
    public float height=5f;
    public int buildTime=5;
    public float buildingModelOffset;
    private NavMeshObstacle navMeshObstacle;
    public BuildingBorder borderComp;
    public BuildingBorder buildingCenter;

    protected Sequence buildSequence;

    private bool isOpenMenu;//是否打开了菜单
    private LineRenderer spawnPosLine;//显示
    protected override void Awake()
    {
        base.Awake();

        navMeshObstacle = GetComponent<NavMeshObstacle>();
        if(navMeshObstacle)
            navMeshObstacle.enabled = false;
        //边界
        borderComp = GetComponent<BuildingBorder>();
       
       
        animModel.transform.localPosition -= Vector3.up * height;

        if (buildTime == 0)
        {
            OnBuildSuccess();
        }
        else
        {
           
            buildSequence = DOTween.Sequence();
            if (height > 0)
            {
                buildSequence.Join(animModel.transform.DOShakeScale(.5f, .5f, buildTime/2));
                buildSequence.Append(animModel.transform.DOLocalJump(animModel.transform.localPosition + Vector3.up * (height + buildingModelOffset), 0.3f, buildTime, buildTime));
            }

            buildSequence.OnComplete(OnBuildSuccess);
        }

        //fogOfWarUnit.enabled = true;

    }
    public virtual void OnBuildSuccess()
    {
        isBuilding = false;
        PlayBuildCompleteFx();
        //fogOfWarUnit.enabled = true;
        if (IsInFog() == false && hpUi != null && needShowHpUi)
        {
            //hpUi.transform.position = mainCam.WorldToScreenPoint(transform.position) + hpUiOffset;//防止血条ui瞬移
            spawnMark.transform.position = spawnPos.transform.position;    
            //hpUi.gameObject.SetActive(true);
            hpUi.Show(true);
        }

        if (navMeshObstacle)
        {
            navMeshObstacle.enabled = true;
            navMeshObstacle.carving = true;
        }
        if (borderComp)
        {
            borderComp.Init(fightingManager,this);
        }

    }
    
    public float GetRadius()
    {
        return borderComp.Size;
    }

    protected void PlayBuildCompleteFx()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            for (int j = 0; j < meshRenderers[i].materials.Length; j++)
            {
                Material material = meshRenderers[i].materials[j];
                material.EnableKeyword("_EMISSION");
                material.DOColor(new Color(1, 1, 1, 1), "_EmissionColor", 0.3f)
                    .SetLoops(2, LoopType.Yoyo);
            }
           
        }
    }

    protected override void InitFactionEntityType()
    {
        battleUnitType = BattleUnitType.Building;
    }

    public override void Die()
    {
        EventCenter.Broadcast(EnumEventType.BuildingDestroyed,this);
        
        //Check if it's the capital building, it's not getting upgraded and the faction defeated condition is set to capital destructionss
        if (configId==BattleUnitId.Base && fightingManager.GetDefeatCondition() == DefeatConditions.destoryBase)
            fightingManager.OnFactionDefeated(factionId);
        
        if (buildingCenter != null) //If the building is not a center then we'll check if it occupies a place in the defined buildings for its center:
            buildingCenter.UnRegisterBuilding(this);
        base.Die();
    }

    public override  void OnFogExit()
    {
        ShowRenderers(true);
        if(photonAnimatorView)
            photonAnimatorView.enabled = true;
        isInFog = false;
        if (isBuilding == false && needShowHpUi)
        {
            //hpUi.gameObject.SetActive(true);
            hpUi.Show(true);
        }
        DiplomaticRelation diplomaticRelation = EnemyIdentifier.Instance.GetMyDiplomaticRelation(factionId);
        if (diplomaticRelation == DiplomaticRelation.Enemy)
        {
            enemyUnitsInMyView.Add(this);
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // if(IsInFog() || isBuilding)
        //     //hpUi.gameObject.SetActive(false);
        //     hpUi.Show(false);
       
        
        if (spawnMarkPfb != null)
        {
            spawnMark=GameObject.Instantiate(spawnMarkPfb);
            spawnMark.SetActive(false);
            spawnMark.transform.position = spawnPos.position;
        }

        spawnPosLine = LineFactory.Instance.GetLineByLineMode(LineMode.Dotted);
        spawnPosLine.enabled = false;
    }

    public GameObject GetSpawnMark()
    {
        return spawnMark;
    }

    public Vector3 GetSpawnPos()
    {
        NavMeshHit navMeshHit;
        if(NavMesh.SamplePosition(spawnPos.transform.position, out navMeshHit,20,-1))
        {
            return navMeshHit.position;
        }
        return spawnPos.position;
    }

    public void OnDragMarkEnd()
    {
        if (spawnMarkFadeTimer == null|| spawnMarkFadeTimer.isCompleted==false || spawnMarkFadeTimer.isDone)
        {
            spawnMarkFadeTimer?.Cancel();
            spawnMarkFadeTimer=Timer.Register(3,()=>
            {
                spawnMark.SetActive(false);
            });
        }
        if (spawnMark.GetComponent<CollisionDetection>().CanPlace()==false)
        {
            spawnMark.transform.position=spawnPos.position;
        }
        GameManager.Instance.GetFightingManager().EnableSelectUnitByRect(true);
        fightingManager.isDragFromBuilding = false;
        fightingManager.buildingWhichIsSetSpawnPos = null;
    }
    
    //更换生成的单位
    private void ChangeSpawnId(int id)
    {
        //spawnId = id;
        //curSpawnInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(id);
        //interval = curSpawnInfo.spawnTime;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isBuilding)
        {
            return;
        }
        base.Update();
        // if (!IsInFog())
        // {
        //     hpUi.gameObject.SetActive(true);
        // }
        if(photonView.IsMine==false)
            return;
        if (toSpawn.Count > 0)
        {
            //判断资源足够时才计时
            if (fightingManager.HasEnoughResToSpawnSolider(toSpawn.Peek()))
            {
                timer += Time.deltaTime;
                if (timer > interval)
                {
                    timer = 0;
                    SpawnUnit();
                }
            }
        }

        if (spawnMark)
        {
            spawnPosLine.enabled = spawnMark.activeSelf;
            if (spawnMark.activeSelf)
            {
                var spawnPosition = spawnMark.transform.position;
                var transPosition = transform.position;
                spawnPosLine.SetPositions(new Vector3[]{transPosition + Vector3.up,spawnPosition + Vector3.up});
                float distance = Vector3.Distance(transPosition, spawnPosition);
                Material material=spawnPosLine.material;
                material.SetTextureScale("_MainTex", new Vector2(distance/4,1));
                
                material.SetTextureOffset("_MainTex", material.mainTextureOffset-new Vector2(1,0)*Time.deltaTime);
            }
        }
        
      
    }

    public override void OnUnSelect()
    {
        base.OnUnSelect();
        if (buildingMenuDialog != null)
        {
            buildingMenuDialog.Close();
        }
    }

    public void SpawnUnit()
    {
        SpawnBattleUnitConfigInfo curSpawnInfo=ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(toSpawn.Peek());
        if (fightingManager.ConsumeResByUnitInfo(curSpawnInfo));
        {
            BattleUnitBase spawnedUnit=BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(curSpawnInfo,spawnPos.position,factionId);
            spawnedUnit.spawnTargetPos = spawnMark.transform.position;
            spawnedUnit.creatorBuilding = this;
        }
        toSpawn.Pop();
        //下一个单位
        if (toSpawn.Count > 0)
        {
            interval = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(toSpawn.Peek()).spawnTime;
        }

        timer = 0;

    }

    public void AddUnitToSpawnStack(BattleUnitId id)
    {
        
        if (toSpawn.Count > 0)
        {
            toSpawn.Push(id);
        }
        else
        {
            toSpawn.Push(id);
            if (GameManager.Instance.gameMode == GameMode.Campaign)
            {
                interval = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(toSpawn.Peek()).spawnTime/3;
            }else
                interval = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoByUnitId(toSpawn.Peek()).spawnTime;
            timer = 0;
        }
    }

    protected override void MouseClickHandle()
    {  
        if (isBuilding || menuCommands.Length == 0)
        {
            return;
        }
        if (UITool.IsPointerOverUIObject(Input.mousePosition))
        {
            return;//防止UI穿透
        }

        DiplomaticRelation relation = EnemyIdentifier.Instance.GetMyDiplomaticRelation(factionId);
        if ( relation== DiplomaticRelation.Neutral|| relation==DiplomaticRelation.Enemy || relation == DiplomaticRelation.Ally )
        {
            return;
        }
        base.MouseClickHandle();

        //打开菜单时关闭建筑血条，关闭菜单时血条回复为打开前的状态
        bool beforeOpenDialog = hpUi.GetShowingStatus();
        void OnDialogClose()
        {
            //hpUi.gameObject.SetActive(beforeOpenDialog);
            hpUi.Show(beforeOpenDialog);
            isOpenMenu = false;
            if (GameManager.Instance.GetFightingManager().isDragFromBuilding==false)
            {
                OnDragMarkEnd();
            }
           
        }
        //hpUi.gameObject.SetActive(false);
        hpUi.Show(false);
        spawnMarkFadeTimer?.Cancel();
        if (spawnMark)
        {
            spawnMark.SetActive(true);
        }
        isOpenMenu = true;
        buildingMenuDialog = BattleBuildingMenuDialog.ShowDialog(this,menuCommands,OnDialogClose) as BattleBuildingMenuDialog;
        
        
    }

    public float GetSpawnRatio()
    {
        return timer / interval;
    }

    private void OnMouseDown()
    {
        if (isBuilding)
        {
            return;
        }
        
    }

    public void StartSetSpawnPos()
    {
        //没有重生点的建筑不适用拖拽修改建筑目标点
        if (spawnPos == null || photonView.IsMine==false)
        {
            return;
        }
        spawnMarkFadeTimer?.Cancel();
        spawnMark.SetActive(true);
        fightingManager.isDragFromBuilding = true;
        fightingManager.buildingWhichIsSetSpawnPos = this;
    }
    
    
}

public class BuildingPreviewMeshConfig
{
    public MeshFilter meshFilter;
}



