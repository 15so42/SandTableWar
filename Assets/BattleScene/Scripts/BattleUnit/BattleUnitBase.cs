using System;
using System.Collections.Generic;
using EPOOutline;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityTimer;

public class BattleUnitBase : MonoBehaviour
{
    [HideInInspector]public StateController stateController;
    [HideInInspector] public NavMeshAgent navMeshAgent;

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
    public Vector3 hpUiOffset=new Vector3(0,20,0);
    
    //选中特效，选中特效由FightingManager动态生成并设置成单位的子物体，一般单位不控制选中特效，除非需要额外增加选中特效效果，比如旋转
    private GameObject selectMark;
    [Header("选中特效大小")]//后期可能还需控制选中特效类型，目前先不管
    public float selectMarkSize = 1;
    public Vector3 selectMarkOffset=Vector3.zero;

    [HideInInspector]public bool isFirstSelected;//第一次被选中

    [Header("MouseOverOutline")] public Outlinable mouseOverOutline;

    [Header("技能点")] public GlobalItemType globalItemType=GlobalItemType.None;
    public int amountBySecond=0; 
    private float lastAddTime=0;

    [Header("受击点,别人攻击此目标时瞄准的位置")] public Transform victimPos;
    [Header("受击偏移，如果没有设置victimPos则使用这个")] public float victimOffset=1.5f;
    [Header("旋转阻尼")] public float rotateDamp = 10;

    public int configId;//配置id，即兵种id，1表示Solider1，2表示Tank_A等等

    private Outlinable outlinable;
    private Timer victimFxTimer;//受击特效计时器
    //静态全局单位列表
    public static List<BattleUnitBase> selfUnits=new List<BattleUnitBase>();
    public static List<BattleUnitBase> enemyUnits=new List<BattleUnitBase>();
    //进入房间
    [HideInInspector]public bool isGoingBuilding;
    [HideInInspector]public bool isInBuilding;

    #region 逻辑控制
    protected virtual void Awake()
    {
        prop=new BattleUnitBaseProp();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if(navMeshAgent) 
            navMeshAgent.angularSpeed = 0;//禁用nav的旋转
        photonView = GetComponent<PhotonView>();
        weapon = GetComponent<Weapon>();//部分建筑类也需要有weapon，部分建筑可以攻击，不会攻击不需要添加weapon
        if (weapon != null)
        {
            weapon.SetOwner(this);
        }

        isFirstSelected = true;
        stateController=new StateController(this);
       
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
        outlinable = GetComponentInChildren<Outlinable>();
        if (photonView.IsMine)
        {
            selfUnits.Add(this);
        }
        else
        {
            enemyUnits.Add(this);
            fightingManager.InitSelectMarkForUnit(this);
        }
        if (mouseOverOutline)
        {
            mouseOverOutline.OutlineParameters.Enabled = false;
        }
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
            if (navMeshAgent != null)
            {
                RotationControl();
            }
            
        }
        hpUi.transform.position = mainCam.WorldToScreenPoint(transform.position) + hpUiOffset;
        
    }

    private Vector3 refDir;
    protected virtual void RotationControl()
    {
        Vector3 horDir = navMeshAgent.desiredVelocity;
        horDir.y = 0;
        transform.forward = Vector3.SmoothDamp(transform.forward, horDir, ref refDir, Time.deltaTime * rotateDamp);
    }
    #endregion

    #region 销毁处理
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
    #endregion

    #region AI命令
    /// <summary>
    /// 設置移動位置,和navmesh的setDestion類似
    /// </summary>
    /// <param name="pos"></param>
    public void SetTargetPos(Vector3 pos)
    {
        stateController?.SetTargetPos(pos);
    }
    #endregion
    
    #region 表现
    public void SetSelectMark(GameObject mark)
    {
        selectMark = mark;
        selectMark.transform.localScale = Vector3.one * selectMarkSize;
        ShowSelectMark();
    }

    //第一次选中时需要设置，之后选中标志通过active来控制以减少性能消耗
    public void ShowSelectMark()
    {
        selectMark.GetComponent<MeshRenderer>().sharedMaterial.SetColor(ColorString,Color.green);
        selectMark.GetComponent<MeshRenderer>().sharedMaterial.SetColor(EmissionString,Color.green);
        selectMark.SetActive(true);
    }

    public void ShowNeutralSelectMark()
    {
        if (selectMark == null)
        {
            fightingManager.InitSelectMarkForUnit(this);
        }
        selectMark.GetComponent<MeshRenderer>().sharedMaterial.SetColor(ColorString,Color.yellow);
        selectMark.GetComponent<MeshRenderer>().sharedMaterial.SetColor(EmissionString,Color.yellow);
        selectMark.SetActive(true);
    }
    public void ShowEnemySelectMark()
    {
        if (selectMark == null)
        {
            fightingManager.InitSelectMarkForUnit(this);
        }
        selectMark.GetComponent<MeshRenderer>().sharedMaterial.SetColor(ColorString,Color.red);
        selectMark.GetComponent<MeshRenderer>().sharedMaterial.SetColor(EmissionString,Color.red);
        selectMark.SetActive(true);
    }

    public void HideSelectMark()
    {
        selectMark.SetActive(false);
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

    /// <summary>
    /// 受击特效
    /// </summary>
    public void PlayVictimFx()
    {
        if (outlinable)
        {
            if (victimFxTimer==null || victimFxTimer.isCompleted)
            {
                outlinable.FrontParameters.Enabled = true;
                
                victimFxTimer = Timer.Register(0.05f,
                    () => outlinable.FrontParameters.Enabled = false);
            }
        }
    }
    #endregion

    #region 鼠标控制

    protected void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnRightMouseDown();
        }

        if (Input.GetMouseButtonUp(1))
        {
            OnRightMouseUp();
        }
    }

    //鼠标进入时，用于更改鼠标形状及标识单位，提示可执行操作等
    private void OnMouseEnter()
    {
        DiplomaticRelation relation = EnemyIdentifier.Instance.GetDiplomaticRelation(campId);
        if (relation == DiplomaticRelation.Enemy)
        {
            MouseShapeManager.Instance.SetMouseState(MouseState.OnEnemyUnit);
        }

        if (mouseOverOutline)
        {
            Color outlineColor=Color.yellow;
            if (relation == DiplomaticRelation.Self)
                outlineColor = Color.green;
            if (relation == DiplomaticRelation.Enemy)
                outlineColor = Color.red;
            mouseOverOutline.OutlineParameters.Color = outlineColor;
            mouseOverOutline.OutlineParameters.Enabled = true;
        }
    }

    //鼠标离开时
    private void OnMouseExit()
    {
        MouseShapeManager.Instance.SetMouseState(MouseState.Default);
        if(mouseOverOutline)
            mouseOverOutline.OutlineParameters.Enabled = false;
    }

    protected virtual void OnRightMouseDown()
    {
        
    }

    protected virtual void OnRightMouseUp()
    {
        DiplomaticRelation relation = EnemyIdentifier.Instance.GetDiplomaticRelation(campId);
        if (relation == DiplomaticRelation.Neutral)
        {
            ShowNeutralSelectMark();
        }
        else
        {
            if (relation == DiplomaticRelation.Ally)
            {
                ShowNeutralSelectMark();
            }

            if (relation == DiplomaticRelation.Enemy)
            {
                ShowEnemySelectMark();
            }
        }
        fightingManager.MoveToSpecificPos(transform.position);
    }
    

    //左键，右键
    private  void OnMouseUpAsButton()
    {
        MouseClickHandle();
    }

    protected virtual void MouseClickHandle()
    {
        if (UITool.IsPointerOverUIObject(Input.mousePosition))
        {
            return;//防止UI穿透
        }

        DiplomaticRelation relation = EnemyIdentifier.Instance.GetDiplomaticRelation(campId);
        if (relation == DiplomaticRelation.Self)
        {
            if (fightingManager.isHoldShift) //加选
            {
                fightingManager.SelectUnit(this);
            }
            else if (fightingManager.isHoldCtrl) //减选
            {
                fightingManager.UnselectUnit(this);
            }
            else //单独选择此单位
            {
                fightingManager.UnselectAllUnits();
                fightingManager.SelectUnit(this);
            }
        }
        
    }
    #endregion

    #region 辅助函数
    public bool IsMine()
    {
        return photonView.IsMine;
    }
    
    [PunRPC]
    public void RecycleBullet(Bullet bullet)
    {
        bullet.Recycle();
    }
    
    public void ReduceHp(int value)
    {
        int leftHp = prop.ReduceHp(value);
        if (leftHp < 0)
        {
            Die();
        }
        UpdateHpUIInPhoton();
    }
    
    private void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }
    
    public Vector3 GetVictimPos()
    {
        if (victimPos)
        {
            return victimPos.transform.position;
        }
        else
        {
            return transform.position + Vector3.up * victimOffset;
        }
    }

    #endregion
    
    #region 同步属性
    
    [PunRPC]
    public void UpdateHpUI(int hp)
    {
        prop.hp = hp;
        hpUi.UpdateHpUi();
        PlayVictimFx();
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
    #endregion

    #region 进入房间防守功能

    public void BeforeInDefenceBuilding()
    {
        navMeshAgent.isStopped = true;
        //GetComponent<MeshRenderer>().enabled = false;
    }

    private DefenceBuilding targetDefenceBuilding;
    private static readonly int ColorString = Shader.PropertyToID("_Color");
    private static readonly int EmissionString = Shader.PropertyToID("_Emission");

    public void GoInDefenceBuilding(DefenceBuilding defenceBuilding)
    {
        isGoingBuilding = true;
        targetDefenceBuilding = defenceBuilding;
        SetTargetPos(defenceBuilding.GetEntrance());
    }

    public void OnEnterBuilding()
    {
        isInBuilding = true;
    }

    public void OutBuilding()
    {
        isInBuilding = false;
    }
    #endregion
    

}
