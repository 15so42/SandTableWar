using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class BattleUnitBase : MonoBehaviour
{
    public StateController stateController;
    
    [HideInInspector]public NavMeshAgent NavMeshAgent { get;private set;}

    protected FightingManager fightingManager;
    public PhotonView photonView;

    public int campId;//陣營Id,用於區分敵我
    //單位都是用武器攻擊敵人，因此抽象出武器類
    [HideInInspector]public Weapon weapon;
    public BattleUnitBaseProp prop;//单位基础属性
   
    
    //血条UI，通过动态加载到对应画布
    [Header("血条UI")]
    protected BaseHpUi hpUi;
    protected Transform hpInfoParent;
    protected Camera mainCam;
    public Vector3 hpUiOffset=new Vector3(0,10,0);
    
    //选中特效，选中特效由FightingManager动态生成并设置成单位的子物体，一般单位不控制选中特效，除非需要额外增加选中特效效果，比如旋转
    private GameObject selectMark;
    [Header("选中特效大小")]//后期可能还需控制选中特效类型，目前先不管
    public float selectMarkSize = 1;
    public Vector3 selectMarkOffset=Vector3.zero;

    [HideInInspector]public bool isFirstSelected;//第一次被选中

    [Header("技能点")] public GlobalItemType globalItemType=GlobalItemType.None;
    public int amountBySecond=0; 
    private float lastAddTime=0;
    
    //静态全局单位列表
    public static List<BattleUnitBase> selfUnits=new List<BattleUnitBase>();
    public static List<BattleUnitBase> enemyUnits=new List<BattleUnitBase>();
    /// <summary>
    /// 建筑类使用时
    /// </summary>
    protected virtual void Awake()
    {
        prop=new BattleUnitBaseProp();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        photonView = GetComponent<PhotonView>();
        weapon = GetComponent<Weapon>();//部分建筑类也需要有weapon，部分建筑可以攻击，不会攻击不需要添加weapon
        if (weapon != null)
        {
            weapon.SetOwner(this);
        }

        isFirstSelected = true;
        stateController=new StateController(this);
        if (photonView.IsMine)
        {
            selfUnits.Add(this);
        }
        else
        {
            enemyUnits.Add(this);
        }
    }

    // Start is called before the first frame update
    /// <summary>
    /// 建筑类继承时先使用base.Start();
    /// </summary>
    protected virtual void Start()
    {
        
        fightingManager = GameManager.Instance.GetFightingManager();
        //生成血条
        mainCam = Camera.main;
        hpInfoParent = UITool.FindUIGameObject("HpInfo").transform;
        var position = transform.position;
        hpUi = Instantiate(Resources.Load<BaseHpUi>("Prefab/UI/BaseHpUi"), mainCam.WorldToScreenPoint(position),
            Quaternion.identity,hpInfoParent);
        hpUi.owner = this;
        hpUi.Init();
        //设置初始目标地点
        stateController.targetPos = position;
        stateController.lastTargetPos = stateController.targetPos;
        lastAddTime = 0;//技能点计时器
    }

    // Update is called once per frame
    /// <summary>
    /// 建筑类继承时使用Base.Update()
    /// </summary>
    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            stateController?.Update();
            if (Time.time - lastAddTime >= 1)
            {
                fightingManager.globalItemManager.AddPoint(globalItemType,amountBySecond);
                lastAddTime = Time.time;
            }
        }
        hpUi.transform.position = mainCam.WorldToScreenPoint(transform.position) + hpUiOffset;
        
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            selfUnits.Remove(this);
        }
        else
        {
            enemyUnits.Remove(this);
        }
    }

    /// <summary>
    /// 設置移動位置,和navmesh的setDestion類似
    /// </summary>
    /// <param name="pos"></param>
    public void SetTargetPos(Vector3 pos)
    {
        stateController?.SetTargetPos(pos);
    }
    
    public void SetSelectMark(GameObject mark)
    {
        selectMark = mark;
        selectMark.transform.localScale = Vector3.one * selectMarkSize;
        ShowSelectMark();
    }

    //第一次选中时需要设置，之后选中标志通过active来控制以减少性能消耗
    public void ShowSelectMark()
    {
        selectMark.SetActive(true);
    }

    public void HideSelectMark()
    {
        selectMark.SetActive(false);
    }
    
    #region 鼠标控制
    protected void OnMouseDown()
    {
        //throw new NotImplementedException();
    }

    protected void OnMouseDrag()
    {
        //throw new NotImplementedException();
    }

    protected void OnMouseUp()
    {
        //throw new NotImplementedException();
    }

    private  void OnMouseUpAsButton()
    {
        if (photonView.IsMine == false)
        {
            return;
        }
        MouseClickHandle();
    }

    protected virtual void MouseClickHandle()
    {
        if (UITool.IsPointerOverUIObject(Input.mousePosition))
        {
            return;//防止UI穿透
        }
        if (fightingManager.isHoldShift)//加选
        {
            fightingManager.SelectUnit(this);
        }
        else if (fightingManager.isHoldCtrl)//减选
        {
            fightingManager.UnselectUnit(this);
        }
        else//单独选择此单位
        {
            fightingManager.UnselectAllUnits();
            fightingManager.SelectUnit(this);
        }
        
    }
    #endregion
    
    public bool IsMine()
    {
        return photonView.IsMine;
    }

    [PunRPC]
    public void RecycleBullet(Bullet bullet)
    {
        bullet.Recycle();
    }

    #region 伤害管理

    public void ReduceHp(int value)
    {
        int leftHp = prop.ReduceHp(value);
        if (leftHp < 0)
        {
            Die();
        }
        UpdateHpUIInPhoton();
    }
    
    #endregion

    private void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    public virtual void OnSelect()
    {
        if (isFirstSelected)
        {
            isFirstSelected = false;
        }

        if (selectMark.gameObject)
        {
            selectMark.gameObject.SetActive(true);
        }
        
    }

    public virtual void OnUnSelect()
    {
        if (selectMark.gameObject)
        {
            selectMark.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void UpdateHpUI(int hp)
    {
        prop.hp = hp;
        hpUi.UpdateHpUi();
    }

    public void UpdateHpUIInPhoton()
    {
        PhotonView.Get(this).RPC(nameof(UpdateHpUI),RpcTarget.All,prop.hp);
    }

    [PunRPC]
    public void SetCampId(int value)
    {
        this.campId = value;
    }

    public void SetCampInPhoton(int value)
    {
        PhotonView.Get(this).RPC(nameof(SetCampId),RpcTarget.All,value);
    }
    
    

}
