using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBorder : MonoBehaviour
{
    private FightingManager fightingManager;
    public BaseBattleBuilding building;
    
    //配置边界显示
    [Header("Border Object:")]
    [SerializeField]
    private bool spawnObj = true; //spawn the border object?
    [SerializeField]
    private GameObject obj; //Use an object that is only visible on the terrain to avoid drawing borders outside the terrain.
    public GameObject Obj { set { obj = value; } get { return obj; } }
    public int Order { private set; get; } //the sorting order of this border, if border A has been activated before border B then border A has higher order than border B.
    //the order is used to determine which has priority over a common area of the map

    [SerializeField]
    private float height = 20.0f; //The height of the border object here
    [Range(0.0f, 1.0f), SerializeField]
    private float colorTransparency = 1.0f; //transparency of the border's object color
    [SerializeField]
    private float size = 10.0f; //The size of the border around this building.
    public float Size { private set { size = value; } get { return size; } }
    [SerializeField]
    private float sizeMultiplier = 2.0f; //To control the relation of the border obj's actual size and the border's map. Using different textures for the border objects will require using 

    
    [System.Serializable]
    public class BuildingInBorder
    {
        [SerializeField]
        public BattleUnitId buildingId ; //prefab of the building to be placed inside the border
        
        
        [SerializeField]
        private FactionTypeInfo factionType = null; //Leave empty if you want this building to be placed by all factions
        public string GetFactionCode () { return factionType.Key; }

        private int currAmount; //current amount of the building type inside this border
        public void UpdateCurrAmount (bool inc) { currAmount += (inc == true) ? 1 : -1; }
        [SerializeField]
        private int maxAmount = 10; //maximum allowed amount of this building type inside this border
        public bool IsMaxAmountReached () { return currAmount >= maxAmount; }
    }
    
    
    private List<BaseBattleBuilding> buildingsInRange = new List<BaseBattleBuilding>(); //a list of the spawned buildings inside the territory defined by this border
    public IEnumerable<BaseBattleBuilding> GetBuildingsInRange () { return buildingsInRange; }
    [Header("配置范围内可放置的建筑"), SerializeField]
    private List<BuildingInBorder> buildingsInBorder = new List<BuildingInBorder>();
    
    
    public void Init(FightingManager fightingManager, BaseBattleBuilding building)
    {
      

        this.fightingManager = fightingManager;
        this.building = building; //get the building that is controlling this border component
        buildingsInRange.Add(this.building); //add source buildings to buildings in range list

        

        if (spawnObj) //if it's allowed to spawn the border object
        {
            obj = (GameObject)Instantiate(obj, new Vector3(transform.position.x, height, transform.position.z), Quaternion.identity); //create the border obj
            obj.transform.localScale = new Vector3(Size * sizeMultiplier, obj.transform.localScale.y, Size * sizeMultiplier); //set the correct size for the border obj
            obj.transform.SetParent(transform, true); //make sure it's a child object of the building main object

            Color FactionColor =  fightingManager.GetFaction(this.building.factionId).factionColor; //set its color to the faction that it belongs to
            obj.GetComponent<MeshRenderer>().material.color = new Color(FactionColor.r, FactionColor.g, FactionColor.b, colorTransparency); //set the color transparency

            obj.GetComponent<MeshRenderer>().sortingOrder = Order; //set the border object's sorting order according to the previosuly placed borders
        }

        EventCenter.Broadcast(EnumEventType.OnBorderActivated,this);

        //subscribe to following events:
        //CustomEvents.ResourceAdded += OnResourceAdded;
        //CustomEvents.ResourceDestroyed += OnResourceDestroyed;

        //BorderResourceRemoved += OnBorderResourceRemoved;

       
        
    }

    private void OnDestroy()
    { 
        EventCenter.Broadcast(EnumEventType.OnBorderDeActivated,this);
        

        //Destroy the border object if it has been created
        if (spawnObj == true)
            Destroy(obj);
    }
    
    //register a new building in this border
    public void RegisterBuilding(BaseBattleBuilding newBuilding)
    {
        buildingsInRange.Add(newBuilding); //add the new building to the list
        foreach (BuildingInBorder bir in buildingsInBorder) //go through all buildings in border slots
            if (bir.buildingId == newBuilding.configId) //if the code matches
                bir.UpdateCurrAmount(true); //increase the current amount
    }

    //unregister an old building from this border
    public void UnRegisterBuilding(BaseBattleBuilding oldBuilding)
    {
        buildingsInRange.Remove(oldBuilding); //remove the building from the list
        foreach (BuildingInBorder bir in buildingsInBorder) //go through all buildings in border slots
            if (bir.buildingId == oldBuilding.configId) //if the code matches
                bir.UpdateCurrAmount(false); //decrease the current amount
    }

    public bool AllowBuildingInBorder(BattleUnitId unitId)
    {
        foreach (BuildingInBorder bir in buildingsInBorder) //go through Ball buildings in border slots
            if (bir.buildingId == unitId) //if the code matches
                return !bir.IsMaxAmountReached(); //allow if the current amount still hasn't reached the max amount

        return true; //if the building type doesn't have a defined slot in the buildings in border list, then it can be definitely accepted.
    }
}
