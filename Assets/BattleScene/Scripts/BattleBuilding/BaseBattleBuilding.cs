using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityTimer;

public class BaseBattleBuilding : BattleUnitBase
{
    private float timer = 0;
    private float interval = 0;
    
    //public int spawnId=1;//作为建筑一般是产出士兵等活动单位，士兵等活动单位通过id确定

    //private SpawnBattleUnitConfigInfo curSpawnInfo;

    public Transform spawnPos;
    public Stack<int> toSpawn=new Stack<int>();
    [Header("建筑菜单，使用字符串表示对应菜单")]
    public string[] menuCommands;

    [Header("出生点标志")] public GameObject spawnMarkPfb;
    public GameObject spawnMark;
    private BattleBuildingMenuDialog buildingMenuDialog;

    private Timer spawnMarkFadeTimer;

    [Header("建造时碰撞检测")] //
    public bool beforeBuilding;//从建造菜单拖出来时生成模型的是否可建造轮廓来标识位置是否可建造，在拖拽完成后正式生成建筑
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        stateController = null;//建筑单位行为较简单，不使用状态机，需要转换时在对应类中重写
        //ChangeSpawnId(spawnId);
        if (spawnMarkPfb != null)
        {
            spawnMark=GameObject.Instantiate(spawnMarkPfb);
            spawnMark.SetActive(false);
            spawnMark.transform.position = spawnPos.position;
        }
    }

    public GameObject GetSpawnMark()
    {
        return spawnMark;
    }

    public void OnDragMarkEnd()
    {
        if (spawnMarkFadeTimer == null|| spawnMarkFadeTimer.isCompleted==false)
        {
            spawnMarkFadeTimer=Timer.Register(3,()=>
            {
                spawnMark.SetActive(false);
            });
        }
        if (spawnMark.GetComponent<CollisionDetection>().CanPlace()==false)
        {
            spawnMark.transform.position=spawnPos.position;
        }

        fightingManager.isDragFromBuilding = false;
        fightingManager.buildingSpawnMark = null;
    }

    public void OnBuildingMarkEnd()
    {
        
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
        base.Update();
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
        SpawnBattleUnitConfigInfo curSpawnInfo=ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(toSpawn.Peek());
        if (fightingManager.ConsumeResByUnitInfo(curSpawnInfo));
        {
            BattleUnitBase spawnedUnit=BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(curSpawnInfo,spawnPos.position,fightingManager.campId);
            spawnedUnit.spawnTargetPos = spawnMark.transform.position;
        }
        toSpawn.Pop();
        //下一个单位
        if (toSpawn.Count > 0)
        {
            interval = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(toSpawn.Peek()).spawnTime;
        }

        timer = 0;

    }

    public void AddUnitToSpawnStack(int id)
    {
        
        if (toSpawn.Count > 0)
        {
            toSpawn.Push(id);
        }
        else
        {
            toSpawn.Push(id);
            interval = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(toSpawn.Peek()).spawnTime;
            timer = 0;
        }
    }

    protected override void MouseClickHandle()
    {
        if (UITool.IsPointerOverUIObject(Input.mousePosition))
        {
            return;//防止UI穿透
        }

        DiplomaticRelation relation = EnemyIdentifier.Instance.GetDiplomaticRelation(campId);
        if ( relation== DiplomaticRelation.Neutral|| relation==DiplomaticRelation.Enemy || relation == DiplomaticRelation.Ally )
        {
            return;
        }
        base.MouseClickHandle();
        buildingMenuDialog = BattleBuildingMenuDialog.ShowDialog(this,menuCommands) as BattleBuildingMenuDialog;
    }

    public float GetSpawnRatio()
    {
        return timer / interval;
    }

    private void OnMouseDown()
    {
        //没有重生点的建筑不适用拖拽修改建筑目标点
        if (spawnPos == null || photonView.IsMine==false)
        {
            return;
        }
        spawnMarkFadeTimer?.Cancel();
        spawnMark.SetActive(true);
        fightingManager.isDragFromBuilding = true;
        fightingManager.buildingSpawnMark = this;
    }
    
    
    
}



