using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public enum GlobalItemType
{
    None,
    Mine,//地雷
    RPG,//火箭筒
}
public class GlobalItemManager : MonoBehaviour
{
    public Transform container;

    private const string ItemPrefabPath = "Prefab/UI/GlobalItem";

    private GameObject cachedItem;
    
    public List<GlobalItem> items=new List<GlobalItem>();
    
    private List<GlobalItemConfig> configs=new List<GlobalItemConfig>();
    
    public Dictionary<GlobalItemType,int> counter=new Dictionary<GlobalItemType, int>();
    // Start is called before the first frame update
    public void Init()
    {
        cachedItem = Resources.Load<GameObject>(ItemPrefabPath);
        configs=new List<GlobalItemConfig>()
        {
            new GlobalItemConfig(GlobalItemType.Mine,100,"Mine","地雷","Mine"),
            new GlobalItemConfig(GlobalItemType.RPG,100,"RPG","RPG","Rpg")
        };
        
        //初始化计数器
        foreach (var config in configs)
        {
            counter.Add(config.itemType,0);
        }

        GameManager.Instance.GetFightingManager().globalItemManager = this;
    }

    // Update is called once per frame
    public void UpdateManager()//不用update和start而是用Init和UpdateManager是为了方便控制主UI界面控制各部分Ui的执行顺序
    {
        for (int i = 0; i < items.Count; i++)
        {
            if(items[i]==null)
                continue;
            items[i].UpdateItem();
        }
    }

    private void SpawnNewItem(GlobalItemType type)
    {
        //实例化
        GameObject iItem=Instantiate(cachedItem, container);
        GlobalItem globalItem = iItem.GetComponent<GlobalItem>();
        globalItem.Init(this,type);
        items.Add(globalItem);
    }

    public void AddPoint(GlobalItemType type,int point)
    {
        if(!counter.ContainsKey(type))
            return;
        counter[type] += point;
       
        //避免重复生成
        bool alreayHasItem = false;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemType == type)
            {
                alreayHasItem = true;
            }
        }
        if(alreayHasItem)
            return;
        SpawnNewItem(type);
        
    }

    public void ActAtPos(Vector3 pos)
    {
        GlobalItemType itemType = items.Find(x => x.isUsing).itemType;
        if (itemType == GlobalItemType.Mine)
        {
            GameObject iMine=PhotonNetwork.Instantiate("BattleUnit/Mine",pos,Quaternion.identity);
            //BattleUnitBaseFactory.Instance.SpawnBattleUnitAtPos()
            iMine.GetComponent<BattleUnitBase>().SetCampInPhoton(GameManager.Instance.GetFightingManager().myFactionId);
        }
    }

    public void ClearUsingItem()
    {
        items.Find(x => x.isUsing == true).isUsing = false;
    }

    public GlobalItemConfig GetConfigByType(GlobalItemType type)
    {
        return configs.Find(x => x.itemType == type);
    }
    public int GetNeedPoint(GlobalItemType type)
    {
        return configs.Find(x => x.itemType == type).needPoint;
    }

    public int GetPoint(GlobalItemType type)
    {
        return counter[type];
    }
    
    //技能范围指示器
    public GameObject GetUsingItemRangeMark()
    {
       return items.Find(x => x.isUsing == true).GetUsingItemRangeMark();
    }
}

//后期考虑换成Excel表
public class GlobalItemConfig
{
    public GlobalItemType itemType;
    public int needPoint;//可使用技能需要的点数，点数达到后出现在全局节能列表里并可以使用
    public string iconPath;
    public string name;
    public string rangeMarkPath;

    public GlobalItemConfig(GlobalItemType type, int needPoint,string iconPath,string name,string rangeMarkPath)
    {
        this.itemType = type;
        this.needPoint = needPoint;
        this.iconPath = iconPath;
        this.name = name;
        this.rangeMarkPath = rangeMarkPath;
    }
}


