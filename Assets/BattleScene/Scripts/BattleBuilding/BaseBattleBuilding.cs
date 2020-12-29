using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

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
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        stateController = null;//建筑单位行为较简单，不使用状态机，需要转换时在对应类中重写
        //ChangeSpawnId(spawnId);
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
    
    public void SpawnUnit()
    {
        SpawnBattleUnitConfigInfo curSpawnInfo=ConfigHelper.Instance.GetSpawnBattleUnitConfigInfoById(toSpawn.Peek());
        if (fightingManager.ConsumeResByUnitInfo(curSpawnInfo));
        {
            BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos(curSpawnInfo,spawnPos.position,fightingManager.campId);
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
        base.MouseClickHandle();
        BattleBuildingMenuDialog.ShowDialog(this,menuCommands);
    }
}



