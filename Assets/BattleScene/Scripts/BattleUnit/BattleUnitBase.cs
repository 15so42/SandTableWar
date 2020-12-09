using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class BattleUnitBase : MonoBehaviour
{
    [HideInInspector]public StateController stateController;
    
    [HideInInspector]public NavMeshAgent NavMeshAgent { get;private set;}

    protected FightingManager fightingManager;
    protected PhotonView photonView;

    public int campId;//陣營Id,用於區分敵我
    //單位都是用武器攻擊敵人，因此抽象出武器類
    [HideInInspector]public Weapon weapon;
    public BattleUnitBaseProp prop;//单位基础属性
    public GameObject selectMark;
    
    //血条UI，通过动态加载到对应画布
    [Header("血条UI")]
    protected BaseHpUi hpUi;
    protected Transform hpInfoParent;
    protected Camera mainCam;
    public Vector3 hpUiOffset=new Vector3(0,10,0);
    
    /// <summary>
    /// 建筑类使用时
    /// </summary>
    protected virtual void Awake()
    {
        prop=new BattleUnitBaseProp();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        photonView = GetComponent<PhotonView>();
        weapon = GetComponent<Weapon>();//建筑类也需要有weapon，部分建筑可以攻击，不会攻击的使用不会攻击的weapon类即可
        weapon.SetOwner(this);
    }

    // Start is called before the first frame update
    /// <summary>
    /// 建筑类继承时先使用base.Start();
    /// </summary>
    protected virtual void Start()
    {
        OnUnSelect();
        stateController=new StateController(this);
        fightingManager = GameManager.Instance.GetFightingManager();
        //生成血条
        mainCam = Camera.main;
        hpInfoParent = UITool.FindUIGameObject("HpInfo").transform;
        hpUi = Instantiate(Resources.Load<BaseHpUi>("Prefab/UI/BaseHpUi"), mainCam.WorldToScreenPoint(transform.position),
            Quaternion.identity,hpInfoParent);
        hpUi.owner = this;

    }

    // Update is called once per frame
    /// <summary>
    /// 建筑类继承时使用Base.Update()
    /// </summary>
    protected virtual void Update()
    {
        stateController?.Update();
        hpUi.transform.position = mainCam.WorldToScreenPoint(transform.position) + hpUiOffset;
    }

    /// <summary>
    /// 設置移動位置,和navmesh的setDestion類似
    /// </summary>
    /// <param name="pos"></param>
    public void SetTargetPos(Vector3 pos)
    {
        stateController.SetTargetPos(pos);
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

    protected virtual void OnMouseUpAsButton()
    {
        if (photonView.IsMine == false)
        {
            return;
        }
        
        fightingManager.SelectUnit(this);
    }
    #endregion

    public void SetCampId(int value)
    {
        campId = value;
    }

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

    public void OnSelect()
    {
        selectMark.gameObject.SetActive(true);
    }

    public void OnUnSelect()
    {
        selectMark.gameObject.SetActive(false);
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
}
