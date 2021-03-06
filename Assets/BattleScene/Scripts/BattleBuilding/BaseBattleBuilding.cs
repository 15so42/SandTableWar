﻿using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    [Header("建造tween动画")] public bool isBuilding=true;
    [Header("建造动画模型")]public GameObject animModel;
    public float height=5f;
    public int buildTime=5;
    protected override void Awake()
    {
        if (isBuilding)
        {
            animModel.transform.position -= Vector3.up * height;

            var sequence = DOTween.Sequence();
            sequence.Join(animModel.transform.DOShakeScale(.5f, .5f, buildTime/2));
            for (int i = 0; i < buildTime; i++)
            {
                // if (buildTime % 2 == 0 && i%2==0 || buildTime%2==1 && i%2==1 || i==0) 
                // {
                    sequence.Append(animModel.transform.DOJump(animModel.transform.position + Vector3.up * i * (height/buildTime), 0.5f, 1, 0.2f));
                    sequence.AppendInterval(0.8f);

                //}
            }
            sequence.OnComplete(() =>
            {
                isBuilding = false;
                PlayBuildCompleteFx();
                Awake();
                Start();
            });
            return;
        }
        base.Awake();
    }

    void PlayBuildCompleteFx()
    {
        
        GetComponent<MeshRenderer>().material.DOColor(new Color(1, 1, 1, 1), "_EmissionColor", 0.3f)
            .SetLoops(2, LoopType.Yoyo);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        if (isBuilding)
        {
            return;
        }
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
        if (isBuilding)
        {
            return;
        }
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
        if (isBuilding)
        {
            return;
        }
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



