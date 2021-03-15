using System;
using System.Collections.Generic;
using BattleScene.Scripts;
using EPOOutline;
using FoW;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityTimer;
using BehaviorDesigner.Runtime;
using BehaviorDesigner;
using BehaviorDesigner.Runtime.Tactical;
using Object = System.Object;

[RequireComponent(typeof(FogOfWarEvents))]
public class BattleUnitBase : MonoBehaviour,IDamageable,IAttackAgent
{
    //[HideInInspector]public StateController stateController;
    public BehaviorTree behaviorDesigner;
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
    //重写寻路组件旋转控制
    [HideInInspector]public bool overrideRotationCtrl=true;
    [HideInInspector]public bool overrideMoveCtrl=false;
    [Header("===移动===")]
    public bool moveByAnim;

    public float moveSpeedMultiplier=1;

    [Header("兵种")]
    public BattleUnitId configId;

    private Outlinable victimOutline;
    private Timer victimFxTimer;//受击特效计时器
    //静态全局单位列表
    public static List<BattleUnitBase> selfUnits=new List<BattleUnitBase>();
    public static List<BattleUnitBase> enemyUnits=new List<BattleUnitBase>();
    //进入房间
    [HideInInspector]public bool isGoingBuilding;
    [HideInInspector]public bool isInBuilding;
    [HideInInspector] public DefenceBuilding defenceBuilding;

    //动画
    [HideInInspector]public BattleUnitAnimCtrl animCtrl;
    //地雷隐形
    public bool visibleOnPhoton;
    
    //初始目标点
    [HideInInspector] public Vector3 spawnTargetPos;

    protected FogOfWarUnit fogOfWarUnit;
    protected FogOfWar fogOfWar;
    protected FogOfWarEvents fogOfWarEvents;
    private Animator animator;
    private Rigidbody rigidbody;
    
    //*********************行为树常量区*********************
    private const string BD_estinationPos="DestinationPos";
    private const string BD_LastDestinationPos="LastDestinationPos";
    private const string BD_EnemyBattleUnit="EnemyBattleUnit";
    private const string BD_EnemyGameObject="EnemyGameObject";
    //****************************************************
    
    //********************真正迷雾控制相关变量****************
    private PhotonAnimatorView photonAnimatorView;//进入雾中后不同步
    [Header("===战争迷雾===")]
    public Renderer[] renderers;//进入战争迷雾后关闭相关的渲染
    private bool isInFog = true;
    
    #region 逻辑控制
    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent)
        {
            navMeshAgent.updateRotation = !overrideRotationCtrl;
            navMeshAgent.updatePosition = !overrideMoveCtrl;
        }

        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        weapon = GetComponent<Weapon>();//部分建筑类也需要有weapon，部分建筑可以攻击，不会攻击不需要添加weapon
        if (weapon != null)
        {
            weapon.SetOwner(this);
        }

        animCtrl = GetComponent<BattleUnitAnimCtrl>();
        isFirstSelected = true;

        //*************战争迷雾**********
        behaviorDesigner = GetComponent<BehaviorTree>();
        fogOfWar = Camera.main.GetComponent<FogOfWar>();
        fogOfWarEvents = GetComponent<FogOfWarEvents>();
        fogOfWarEvents.onFogEnter.AddListener(OnFogEnter);
        fogOfWarEvents.onFogExit.AddListener(OnFogExit);
        photonAnimatorView = GetComponent<PhotonAnimatorView>();
        //因为设置campId是在awake后执行的，而且因为在网络中传输可能会有延迟，所以建筑一般先关闭迷雾，知道收到设置campId的消息后再根据情况决定是否打开迷雾。
        fogOfWarUnit = GetComponent<FogOfWarUnit>();
        fogOfWarUnit.enabled = false;
    }

    //晚于Awake执行
    protected virtual void SetFogOfWarTeam()
    {
        //fogOfWarUnit = GetComponent<FogOfWarUnit>();
        if(fogOfWarUnit){
            fogOfWarUnit.enabled = true;
            fogOfWarUnit.circleRadius = prop.viewDistance;
            fogOfWarUnit.team = campId;
        }

    }

    //这个方法和直观理解不太一样，Enter标识能看见，Exit表示看不见
    public void OnFogEnter()
    {
        ShowRenderers(true);
        if(photonAnimatorView)
            photonAnimatorView.enabled = true;
        isInFog = false;
        // if(isInBuilding==false)
        //     hpUi.gameObject.SetActive(true);
    }

    public void OnFogExit()
    {
        ShowRenderers(false);
        if(photonAnimatorView)
            photonAnimatorView.enabled = false;
        isInFog = true;
        //hpUi.gameObject.SetActive(false);
    }

    public bool IsInFog()
    {
        return isInFog;
    }

    private void ShowRenderers(bool status)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = status;
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
       
        if (behaviorDesigner)
        {
            behaviorDesigner.SetVariableValue( "DestinationPos",transform.position);
            behaviorDesigner.EnableBehavior();
        }
       
        
        lastAddTime = 0;//技能点计时器
        victimOutline = GetComponentInChildren<Outlinable>();
        
        if (photonView.IsMine)
        {
            if (GameManager.Instance.gameMode == GameMode.Campaign)
            {
                if (EnemyIdentifier.Instance.GetDiplomaticRelation(campId) == DiplomaticRelation.Self)
                {
                    selfUnits.Add(this);
                }
                else
                {
                    enemyUnits.Add(this);
                    fightingManager.InitSelectMarkForUnit(this);
                }
            }
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
            if (GameManager.Instance.gameMode == GameMode.Campaign)
            {
                if (EnemyIdentifier.Instance.GetDiplomaticRelation(campId) == DiplomaticRelation.Self)
                {
                    if (Time.time - lastAddTime >= 1)
                    {
                        fightingManager.globalItemManager.AddPoint(globalItemType,amountBySecond);
                        lastAddTime = Time.time;
                    }
                }
                
            }
            if (navMeshAgent != null)
            {
                RotationControl();
                MovementControl();
            }
        }
        hpUi.transform.position = mainCam.WorldToScreenPoint(transform.position) + hpUiOffset;

    }

    /// <summary>
    /// 开启火关闭转向
    /// </summary>
    /// <param name="update"></param>
    public void UpdateRotation(bool update)
    {
        
        if (overrideRotationCtrl)
        {
            overrideRotationCtrl = update;
        }
        else
        {
            navMeshAgent.updateRotation = update;
        }
    }

    private Vector3 refDir;
    protected virtual void RotationControl()
    {
        if (overrideRotationCtrl == false)
            return;
        Vector3 horDir = navMeshAgent.desiredVelocity;
        horDir.y = 0;
        transform.forward = Vector3.SmoothDamp(transform.forward, horDir, ref refDir, Time.deltaTime * rotateDamp);
    }

    private void OnAnimatorMove()
    {
        if (animator)
        {
            if (moveByAnim)
            {
                refAnimDeltaPos = animator.deltaPosition;
            }
        }
           
    }
    
    private Vector3 refAnimDeltaPos;
    protected virtual void MovementControl()
    {
        if (overrideMoveCtrl == false)
            return;
        if (moveByAnim)
        {
            transform.Translate((refAnimDeltaPos*moveSpeedMultiplier)/Time.deltaTime,Space.World);
            //通过刚体移动
            // Vector3 v=(refAnimDeltaPos*moveSpeedMultiplier)/Time.deltaTime;
            // v.y = rigidbody.velocity.y;
            // rigidbody.velocity = v;
        }
        else
        {
            Vector3 horDir = navMeshAgent.desiredVelocity;
            transform.Translate(Vector3.forward * (horDir.magnitude * Time.deltaTime),Space.Self);
        }
       
        navMeshAgent.nextPosition = transform.position;
    }

    
   

    #endregion

    #region 销毁处理
    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            if (GameManager.Instance.gameMode == GameMode.Campaign)
            {
                if(EnemyIdentifier.Instance.GetDiplomaticRelation(campId)==DiplomaticRelation.Self)
                    selfUnits.Remove(this);
                else
                    enemyUnits.Remove(this);
            }
        }
        else
        {
            enemyUnits.Remove(this);
        }

        if (hpUi)
        {
            Destroy(hpUi.gameObject);
        }
        
    }
    #endregion

    #region AI命令
    
    /// <summary>
    /// 设置追踪目标
    /// </summary>
    /// <param name="chaseTarget"></param>
    public virtual void SetChaseTarget(BattleUnitBase chaseTarget)
    {
        if (chaseTarget.configId != BattleUnitId.Mineral && chaseTarget!=this)
        {
            behaviorDesigner.SetVariableValue(BD_EnemyBattleUnit,chaseTarget);
            behaviorDesigner.SetVariableValue(BD_EnemyGameObject,chaseTarget.gameObject);
        }
        //stateController?.SetChaseTarget(chaseTarget);
    }
    
    /// <summary>
    /// 設置移動位置,和navmesh的setDestion類似
    /// </summary>
    /// <param name="pos"></param>
    public virtual void SetTargetPos(Vector3 pos)
    {
        if (behaviorDesigner)
        {
            //behaviorDesigner.SetVariableValue( "DestinationPos",pos);
            behaviorDesigner.SendEvent("SetDestinationPos",pos);
            Debug.Log(pos);
        }
        
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
        selectMark.GetComponent<MeshRenderer>().material.SetColor(ColorString,Color.green);
        selectMark.GetComponent<MeshRenderer>().material.SetColor(EmissionString,Color.green);
        selectMark.SetActive(true);
    }

    public void ShowNeutralSelectMark()
    {
        if (selectMark == null)
        {
            fightingManager.InitSelectMarkForUnit(this);
        }
        selectMark.GetComponent<MeshRenderer>().material.SetColor(ColorString,Color.yellow);
        selectMark.GetComponent<MeshRenderer>().material.SetColor(EmissionString,Color.yellow);
        selectMark.SetActive(true);
    }
    public void ShowEnemySelectMark()
    {
        if (selectMark == null)
        {
            fightingManager.InitSelectMarkForUnit(this);
        }
        selectMark.GetComponent<MeshRenderer>().material.SetColor(ColorString,Color.red);
        selectMark.GetComponent<MeshRenderer>().material.SetColor(EmissionString,Color.red);
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
    /// 治疗特效
    /// </summary>
    public void PlayCureFx()
    {
        var transform1 = transform;
        BattleFxManager.Instance.SpawnFxAtPosInPhoton("CureFx",transform1.position,Vector3.forward);
    }
    /// <summary>
    /// 受击特效
    /// </summary>
    public void PlayVictimFx()
    {
        if (victimOutline)
        {
            if (victimFxTimer==null || victimFxTimer.isCompleted)
            {
                victimOutline.FrontParameters.Enabled = true;
                
                victimFxTimer = Timer.Register(0.05f,
                    () => victimOutline.FrontParameters.Enabled = false);
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
        fightingManager.ChaseTargetUnit(this);
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
        UpdateHpUIInPhoton();
    }
    
    public void CureHp(int value)
    {
        int leftHp = prop.CureHp(value);
        UpdateHpUIInPhoton();
    }
    
    private void Die()
    {
        Destroy(hpUi);
        PhotonNetwork.Destroy(gameObject);
        animCtrl.DieAnim();
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
        if (prop.hp<hp)//治疗
        {
            PlayCureFx();
        }
        else
        {
            PlayVictimFx();
        }
        prop.hp = hp;
        if (hp <= 0 && photonView.IsMine)
        {
            Die();
        }
        hpUi.UpdateHpUi();
        
    }

    public void UpdateHpUIInPhoton()
    {
        UpdateHpUI(prop.hp);
        PhotonView.Get(this).RPC(nameof(UpdateHpUI),RpcTarget.Others,prop.hp);
    }

    [PunRPC]
    public void SetCampId(int value)
    {
        this.campId = value;
        SetFogOfWarTeam();
    }

    public void SetCampInPhoton(int value)
    {
        SetCampId(value);
        PhotonView.Get(this).RPC(nameof(SetCampId),RpcTarget.Others,value);
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
    private static readonly int EmissionString = Shader.PropertyToID("_EmissionColor");

    public void GoInDefenceBuilding(DefenceBuilding defenceBuilding)
    {
        isGoingBuilding = true;
        targetDefenceBuilding = defenceBuilding;
        SetTargetPos(defenceBuilding.GetEntrance());
    }

    public void OnEnterBuilding(DefenceBuilding building)
    {
        isInBuilding = true;
        behaviorDesigner.SetVariableValue("isInBuilding",true);
        defenceBuilding = building;
    }

    public void OutBuilding()
    {
        behaviorDesigner.SetVariableValue("isInBuilding",false);
        isInBuilding = false;
    }
    #endregion


    public void Damage(int amount)
    {
        ReduceHp(amount);
    }

    public bool IsAlive()
    {
        return prop.hp > 0;
    }
    

    #region 攻击行为树模块
    public float AttackDistance()
    {
        return prop.attackDistance;
    }

    public bool CanAttack()
    {
        //为了使用战术行为树，此处需要适配对应的方法以攻击敌人
        return true;
    }

    public float AttackAngle()
    {
        return 5;
    }
    
    //使用战术包行为树的Attack接口
    public void Attack(Vector3 targetPosition)
    {
        weapon.WeaponUpdate();
    }
    #endregion 
}
