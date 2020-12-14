using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BaseBattleBuilding : BattleUnitBase
{
    //临时存储id和对应的动态加载的名称
    public Dictionary<int,string> soliderIdTable=new Dictionary<int, string>()
    {
        {1,"Solider1"}
    };

    private float timer = 0;
    private float interval = 0;
    
    public int spawnId=1;//作为建筑一般是产出士兵等活动单位，士兵等活动单位通过id确定

    private SpawnBattleUnitConfigInfo curSpawnInfo;

    public Transform spawnPos;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        base.Start();
        stateController = null;//建筑单位行为较简单，不使用状态机，需要转换时在对应类中重写
        ChangeSpawnId(spawnId);
    }
    
    //更换生成的单位
    public void ChangeSpawnId(int id)
    {
        spawnId = id;
        curSpawnInfo = ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(id);
        interval = curSpawnInfo.spawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if(photonView.IsMine==false)
            return;
        //判断资源足够时才计时
        if (fightingManager.HasEnoughResToSpawnSolider(spawnId))
        {
            timer += Time.deltaTime;
            if (timer > interval)
            {
                timer = 0;
                SpawnUnit();
            }
        }
        
    }


    public void SpawnUnit()
    {
        if (fightingManager.ConsumeResByUnitInfo(curSpawnInfo))
        {
            BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(curSpawnInfo,spawnPos.position,fightingManager.campId);
        }
        
    }
}



